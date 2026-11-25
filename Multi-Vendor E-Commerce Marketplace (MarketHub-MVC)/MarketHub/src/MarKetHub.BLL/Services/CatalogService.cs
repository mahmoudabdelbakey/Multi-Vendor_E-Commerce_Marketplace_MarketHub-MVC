using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;

    public CatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Search, filter, sort and paginate active products
    // from approved vendors only.
    public async Task<PagedResult<ProductListItemDto>> SearchAsync(
        ProductQuery query,
        CancellationToken ct = default)
    {
        query.Normalize();

        var products = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive &&
                        p.Vendor.Status == VendorStatus.Approved);

        // Search by product name.
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            products = products.Where(p =>
                p.Name.Contains(query.Search));
        }

        // Filter by category.
        if (query.CategoryId.HasValue)
        {
            products = products.Where(p =>
                p.CategoryId == query.CategoryId.Value);
        }

        // Filter by minimum price.
        if (query.MinPrice.HasValue)
        {
            products = products.Where(p =>
                p.Price >= query.MinPrice.Value);
        }

        // Filter by maximum price.
        if (query.MaxPrice.HasValue)
        {
            products = products.Where(p =>
                p.Price <= query.MaxPrice.Value);
        }

        // Sorting.
        products = query.Sort switch
        {
            "price_asc" => products.OrderBy(p => p.Price).ThenBy(p => p.Id),
            "price_desc" => products.OrderByDescending(p => p.Price),
            "name" => products.OrderBy(p => p.Name),
            _ => products.OrderByDescending(p => p.CreatedAt)
        };

        // Total number of matching products.
        var totalCount = await products.CountAsync(ct);

        // Pagination and projection.
        var items = await products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductListItemDto(
                p.Id,
                p.Name,
                p.Price,
                p.StockQuantity,
                p.Category.Name,
                p.Vendor.StoreName,
                p.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);

        return new PagedResult<ProductListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    // Get product details for active products
    // from approved vendors only.
    public async Task<ProductDetailsDto?> GetDetailsAsync(
        int productId,
        CancellationToken ct = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == productId &&
                        p.IsActive &&
                        p.Vendor.Status == VendorStatus.Approved)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity,
                p.CategoryId,
                CategoryName = p.Category.Name,
                p.VendorProfileId,
                StoreName = p.Vendor.StoreName,
                Images = p.Images
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto(
                        i.Id,
                        i.ImagePath,
                        i.IsPrimary))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (product == null)
            return null;

        // Retrieve reviews separately using the Reviews DbSet.
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(
                r.Id,
                r.Customer.FullName,
                r.Rating,
                r.Comment,
                r.CreatedAt))
            .ToListAsync(ct);

        var averageRating = reviews.Count == 0
            ? 0
            : reviews.Average(r => (double)r.Rating);

        return new ProductDetailsDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CategoryId,
            product.CategoryName,
            product.VendorProfileId,
            product.StoreName,
            product.Images,
            averageRating,
            reviews.Count,
            reviews);
    }

    // Retrieve active categories.
    public async Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync(
        CancellationToken ct = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.IsActive,
                c.Products.Count(p =>
                    p.IsActive &&
                    p.Vendor.Status == VendorStatus.Approved)
            ))
            .ToListAsync(ct);
    }

    // Retrieve featured products from approved vendors only.
    public async Task<IReadOnlyList<ProductListItemDto>> GetFeaturedAsync(
        int take = 8,
        CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 48);

        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive &&
                        p.Vendor.Status == VendorStatus.Approved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .Select(p => new ProductListItemDto(
                p.Id,
                p.Name,
                p.Price,
                p.StockQuantity,
                p.Category.Name,
                p.Vendor.StoreName,
                p.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);
    }
}