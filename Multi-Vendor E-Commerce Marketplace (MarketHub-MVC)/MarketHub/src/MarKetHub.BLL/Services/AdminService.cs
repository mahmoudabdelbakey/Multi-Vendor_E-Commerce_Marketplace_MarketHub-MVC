using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _db;

    public AdminService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var totalVendors = await _db.VendorProfiles.CountAsync(ct);
        var pendingVendors = await _db.VendorProfiles.CountAsync(v => v.Status == VendorStatus.Pending, ct);
        var approvedVendors = await _db.VendorProfiles.CountAsync(v => v.Status == VendorStatus.Approved, ct);
        var suspendedVendors = await _db.VendorProfiles.CountAsync(v => v.Status == VendorStatus.Suspended, ct);

        var totalProducts = await _db.Products.CountAsync(ct);
        var activeProducts = await _db.Products.CountAsync(p => p.IsActive, ct);

        var registeredUsers = await _db.Users.CountAsync(ct);

        var totalOrders = await _db.Orders.CountAsync(ct);
        var paidOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed, ct);

        var grossRevenue = await _db.Orders
            .Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount, ct);

        var totalCommission = await _db.CommissionRecords
            .Where(c => c.Status == CommissionStatus.Recorded)
            .SumAsync(c => c.CommissionAmount, ct);

        var topVendors = await _db.VendorProfiles
            .Where(v => v.Status == VendorStatus.Approved)
            .Select(v => new TopVendorDto(
                v.StoreName,
                v.Products.SelectMany(p => p.OrderItems)
                    .Where(oi => oi.Order.Status == OrderStatus.Paid || oi.Order.Status == OrderStatus.Completed)
                    .Sum(oi => oi.LineTotal),
                v.CommissionRecords.Where(c => c.Status == CommissionStatus.Recorded).Sum(c => c.CommissionAmount)
            ))
            .OrderByDescending(x => x.GrossSales)
            .Take(5)
            .ToListAsync(ct);

        var last7Days = await _db.Orders
            .Where(o => o.PaidAt != null && o.PaidAt >= DateTime.UtcNow.AddDays(-7))
            .GroupBy(o => o.PaidAt!.Value.Date)
            .Select(g => new DailyRevenueDto(g.Key, g.Sum(o => o.TotalAmount)))
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        return new AdminDashboardDto(
            totalVendors, pendingVendors, approvedVendors, suspendedVendors,
            totalProducts, activeProducts, registeredUsers,
            totalOrders, paidOrders, grossRevenue, totalCommission,
            topVendors, last7Days);
    }

    public async Task<PagedResult<AdminProductDto>> ListProductsAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Products
            .Include(p => p.Vendor)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Vendor.StoreName.Contains(search));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new AdminProductDto(
                p.Id,
                p.Name,
                p.Vendor.StoreName,
                p.Category.Name,
                p.Price,
                p.StockQuantity,
                p.IsActive))
            .ToListAsync(ct);

        return new PagedResult<AdminProductDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Result> SetProductActiveAsync(int productId, bool isActive, CancellationToken ct = default)
    {
        var product = await _db.Products.FindAsync(new object[] { productId }, ct);

        if (product == null)
            return Result.NotFoundResult("Product not found.");

        product.IsActive = isActive;
        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
