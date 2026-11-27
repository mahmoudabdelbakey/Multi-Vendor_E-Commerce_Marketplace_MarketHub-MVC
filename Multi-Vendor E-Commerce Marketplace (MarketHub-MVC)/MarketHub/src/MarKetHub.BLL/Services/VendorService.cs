using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

/// <summary>
/// TASK-28. The vendor's own side: apply for a store, read the application status, edit the store profile,
/// see the dashboard numbers. A new application is always Pending; only a Super Admin (TASK-29) changes the status.
/// </summary>
public class VendorService : IVendorService
{
    private const string NameTaken = "This store name is already taken.";

    private readonly ApplicationDbContext _db;

    public VendorService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<int>> ApplyAsync(string userId, VendorApplicationInput input, CancellationToken ct = default)
    {
        var name = (input.StoreName ?? string.Empty).Trim();
        var description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();

        var error = ValidateStore(name, description);
        if (error is not null)
        {
            return Result<int>.Fail(error);
        }

        if (!await _db.Users.AnyAsync(u => u.Id == userId, ct))
        {
            return Result<int>.NotFoundResult("User not found.");
        }

        // One application per user (the UNIQUE index on UserId is the last line of defence).
        if (await _db.VendorProfiles.AnyAsync(v => v.UserId == userId, ct))
        {
            return Result<int>.Fail("You have already submitted a store application.");
        }

        if (!await IsStoreNameAvailableAsync(name, null, ct))
        {
            return Result<int>.Fail(NameTaken);
        }

        var profile = new VendorProfile
        {
            UserId = userId,
            StoreName = name,
            Description = description,
            Status = VendorStatus.Pending
        };

        _db.VendorProfiles.Add(profile);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
        {
            // Two people took the same name (or the same user clicked twice) at the same moment.
            return Result<int>.Fail(NameTaken);
        }

        return Result<int>.Ok(profile.Id, "Your application was submitted. An administrator will review it.");
    }

    public async Task<bool> IsStoreNameAvailableAsync(string storeName, int? exceptVendorId = null, CancellationToken ct = default)
    {
        var name = (storeName ?? string.Empty).Trim();
        return !await _db.VendorProfiles.AnyAsync(v => v.StoreName == name && v.Id != exceptVendorId, ct);
    }

    public async Task<VendorDto?> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        await _db.VendorProfiles.AsNoTracking()
            .Where(v => v.UserId == userId)
            .Select(v => new VendorDto(
                v.Id, v.UserId, v.StoreName, v.Description, v.LogoPath,
                v.Status, v.RejectionReason, v.CreatedAt, v.ReviewedAt,
                v.User.FullName, v.User.Email ?? string.Empty))
            .FirstOrDefaultAsync(ct);

    public async Task<int?> GetApprovedVendorIdAsync(string userId, CancellationToken ct = default)
    {
        var id = await _db.VendorProfiles.AsNoTracking()
            .Where(v => v.UserId == userId && v.Status == VendorStatus.Approved)
            .Select(v => v.Id)
            .FirstOrDefaultAsync(ct);

        return id == 0 ? null : id;
    }

    public async Task<Result> UpdateProfileAsync(int vendorId, StoreProfileInput input, CancellationToken ct = default)
    {
        var name = (input.StoreName ?? string.Empty).Trim();
        var description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();

        var error = ValidateStore(name, description);
        if (error is not null)
        {
            return Result.Fail(error);
        }

        var vendor = await _db.VendorProfiles.FirstOrDefaultAsync(v => v.Id == vendorId, ct);
        if (vendor is null)
        {
            return Result.NotFoundResult();
        }

        if (!await IsStoreNameAvailableAsync(name, vendorId, ct))
        {
            return Result.Fail(NameTaken);
        }

        vendor.StoreName = name;
        vendor.Description = description;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
        {
            return Result.Fail(NameTaken);
        }

        return Result.Ok("Store profile updated.");
    }

    public async Task<VendorDashboardDto> GetDashboardAsync(int vendorId, CancellationToken ct = default)
    {
        // Every number is filtered by vendorId: a vendor can never see the figures of another vendor.
        var paidItems = _db.OrderItems.AsNoTracking().Where(i =>
            i.VendorProfileId == vendorId
            && (i.Order.Status == OrderStatus.Paid || i.Order.Status == OrderStatus.Completed)
            && i.FulfillmentStatus != FulfillmentStatus.Cancelled);

        var gross = await paidItems.SumAsync(i => (decimal?)i.LineTotal, ct) ?? 0m;

        var records = _db.CommissionRecords.AsNoTracking()
            .Where(r => r.VendorProfileId == vendorId && r.Status == CommissionStatus.Recorded);
        var commission = await records.SumAsync(r => (decimal?)r.CommissionAmount, ct) ?? 0m;
        var net = await records.SumAsync(r => (decimal?)r.VendorNetAmount, ct) ?? 0m;

        var products = _db.Products.AsNoTracking().Where(p => p.VendorProfileId == vendorId);
        var productCount = await products.CountAsync(ct);
        var activeProducts = await products.CountAsync(p => p.IsActive, ct);

        var pending = await paidItems.CountAsync(i => i.FulfillmentStatus == FulfillmentStatus.Pending, ct);
        var shipped = await paidItems.CountAsync(i => i.FulfillmentStatus == FulfillmentStatus.Shipped, ct);
        var delivered = await paidItems.CountAsync(i => i.FulfillmentStatus == FulfillmentStatus.Delivered, ct);

        var recent = await paidItems
            .OrderByDescending(i => i.Order.CreatedAt).ThenByDescending(i => i.Id)
            .Take(10)
            .Select(i => new VendorOrderItemDto(
                i.Id, i.OrderId, i.Order.OrderNumber, i.Order.CreatedAt,
                i.ProductId, i.ProductNameSnapshot, i.Quantity, i.UnitPrice, i.LineTotal,
                i.FulfillmentStatus,
                i.Order.ShippingFullName, i.Order.ShippingPhone, i.Order.ShippingAddress,
                i.Order.ShippingCity, i.Order.ShippingCountry,
                i.CommissionRecord != null ? i.CommissionRecord.CommissionAmount : (decimal?)null,
                i.CommissionRecord != null ? i.CommissionRecord.VendorNetAmount : (decimal?)null))
            .ToListAsync(ct);

        return new VendorDashboardDto(
            gross, commission, net, productCount, activeProducts, pending, shipped, delivered, recent);
    }

    /// <summary>Same limits as the database columns (StoreName 100, Description 1000).</summary>
    private static string? ValidateStore(string name, string? description)
    {
        if (name.Length < 3 || name.Length > 100)
        {
            return "Store name must be between 3 and 100 characters.";
        }

        if (description is { Length: > 1000 })
        {
            return "The description cannot be longer than 1000 characters.";
        }

        return null;
    }
}
