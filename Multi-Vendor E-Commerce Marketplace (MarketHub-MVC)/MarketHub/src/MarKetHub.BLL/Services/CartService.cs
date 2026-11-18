using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketHub.BLL.Services;

public sealed class CartService : ICartService
{
    private const int MaxQuantityPerLine = 99;
    private const string SignInMessage = "Please sign in to manage your cart.";

    private readonly ApplicationDbContext _db;
    private readonly ILogger<CartService> _logger;

    public CartService(ApplicationDbContext db, ILogger<CartService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CartDto> GetCartAsync(string customerId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return new CartDto();
        }

        var rows = await _db.CartItems
            .AsNoTracking()
            .Where(line => line.CustomerId == customerId)
            .OrderBy(line => line.AddedAt)
            .ThenBy(line => line.Id)
            .Select(line => new
            {
                line.Id,
                line.Quantity,
                ProductId = line.Product.Id,
                line.Product.Name,
                line.Product.Price,
                line.Product.StockQuantity,
                line.Product.IsActive,
                StoreName = line.Product.Vendor.StoreName,
                VendorStatus = line.Product.Vendor.Status,
                ImagePath = line.Product.Images
                    .OrderByDescending(image => image.IsPrimary)
                    .ThenBy(image => image.SortOrder)
                    .Select(image => image.ImagePath)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var items = rows.Select(row =>
        {
            var reason = GetUnavailableReason(row.IsActive, row.VendorStatus);

            if (reason is null && row.Quantity > row.StockQuantity)
            {
                reason = GetStockMessage(row.StockQuantity);
            }
            else if (reason is null && row.Quantity > MaxQuantityPerLine)
            {
                reason = $"Quantity cannot exceed {MaxQuantityPerLine} per product.";
            }

            return new CartItemDto(
                row.Id,
                row.ProductId,
                row.Name,
                row.StoreName,
                row.Price,
                row.Quantity,
                row.StockQuantity,
                reason is null,
                reason,
                row.ImagePath);
        }).ToList();

        return new CartDto { Items = items };
    }

    public async Task<int> CountAsync(string customerId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return 0;
        }

        return await _db.CartItems
            .Where(line => line.CustomerId == customerId)
            .SumAsync(line => (int?)line.Quantity, ct) ?? 0;
    }

    public async Task<Result> AddAsync(
        string customerId, int productId, int quantity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Result.Fail(SignInMessage);
        }

        var quantityError = ValidateQuantity(quantity);
        if (quantityError is not null)
        {
            return Result.Fail(quantityError);
        }

        var product = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new { p.IsActive, VendorStatus = p.Vendor.Status, p.StockQuantity })
            .FirstOrDefaultAsync(ct);

        if (product is null || GetUnavailableReason(product.IsActive, product.VendorStatus) is not null)
        {
            return Result.NotFoundResult("Product is not available.");
        }

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var line = await _db.CartItems
                .FirstOrDefaultAsync(l => l.CustomerId == customerId && l.ProductId == productId, ct);

            var newQuantity = (line?.Quantity ?? 0) + quantity;

            if (newQuantity > MaxQuantityPerLine)
            {
                return Result.Fail($"You can add up to {MaxQuantityPerLine} of this product.");
            }

            if (newQuantity > product.StockQuantity)
            {
                return Result.Fail(GetStockMessage(product.StockQuantity));
            }

            if (line is null)
            {
                _db.CartItems.Add(new CartItem
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                });
            }
            else
            {
                line.Quantity = newQuantity;
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                return Result.Ok("Added to your cart.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Adding product {ProductId} to the cart failed (attempt {Attempt})",
                    productId, attempt + 1);

                _db.ChangeTracker.Clear();
            }
        }

        return Result.Fail("We couldn't update your cart. Please try again.");
    }

    public async Task<Result> UpdateQuantityAsync(
        string customerId, int cartItemId, int quantity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Result.Fail(SignInMessage);
        }

        var quantityError = ValidateQuantity(quantity);
        if (quantityError is not null)
        {
            return Result.Fail(quantityError);
        }

        var line = await _db.CartItems
            .Include(l => l.Product)
                .ThenInclude(p => p.Vendor)
            .FirstOrDefaultAsync(l => l.Id == cartItemId && l.CustomerId == customerId, ct);

        if (line is null)
        {
            return Result.NotFoundResult("Cart item not found.");
        }

        if (GetUnavailableReason(line.Product.IsActive, line.Product.Vendor.Status) is not null)
        {
            return Result.Fail("This product is no longer available. Remove it from your cart.");
        }

        if (quantity > line.Product.StockQuantity)
        {
            return Result.Fail(GetStockMessage(line.Product.StockQuantity));
        }

        line.Quantity = quantity;
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Cart quantity updated.");
    }

    public async Task<Result> RemoveAsync(string customerId, int cartItemId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Result.Fail(SignInMessage);
        }

        var line = await _db.CartItems
            .FirstOrDefaultAsync(l => l.Id == cartItemId && l.CustomerId == customerId, ct);

        if (line is null)
        {
            return Result.NotFoundResult("Cart item not found.");
        }

        _db.CartItems.Remove(line);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Item removed from your cart.");
    }

    private static string? GetUnavailableReason(bool isActive, VendorStatus vendorStatus)
    {
        if (!isActive)
        {
            return "This product is no longer available.";
        }

        if (vendorStatus != VendorStatus.Approved)
        {
            return "This store is not currently available.";
        }

        return null;
    }

    private static string? ValidateQuantity(int quantity)
    {
        if (quantity < 1)
        {
            return "Quantity must be at least 1.";
        }

        if (quantity > MaxQuantityPerLine)
        {
            return $"Quantity cannot exceed {MaxQuantityPerLine} per product.";
        }

        return null;
    }

    private static string GetStockMessage(int stockQuantity) =>
        stockQuantity <= 0
            ? "This product is out of stock."
            : $"Only {stockQuantity} item(s) are currently in stock.";
}

