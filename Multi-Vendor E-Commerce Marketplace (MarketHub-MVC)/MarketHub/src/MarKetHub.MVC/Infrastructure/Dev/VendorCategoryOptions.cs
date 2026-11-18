using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.MVC.Infrastructure.Dev;

/// <summary>
/// TEMPORARY STUB (Member C, TASK-19). The product form needs the list of active categories, but ICatalogService (B) and
/// ICategoryService (E) are not implemented yet. This tiny interface is OURS on purpose: if we registered ICatalogService
/// here we would silently replace B's real implementation (VendorProductsFeature runs after CatalogFeature).
///
/// TASK-48 (remove stubs): delete this file and the registration in VendorProductsFeature, and in ProductsController call
/// ICatalogService.GetActiveCategoriesAsync instead (same CategoryDto, so it is a one-line change).
/// </summary>
public interface IVendorCategoryOptions
{
    Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken ct = default);
}

public class DevVendorCategoryOptions : IVendorCategoryOptions
{
    private readonly ApplicationDbContext _db;

    public DevVendorCategoryOptions(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken ct = default) =>
        await _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.IsActive, 0))
            .ToListAsync(ct);
}
