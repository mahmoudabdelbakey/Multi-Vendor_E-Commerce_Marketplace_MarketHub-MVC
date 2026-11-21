using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;

    public CategoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken ct = default)
    {
        var categories = await _db.Categories
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.IsActive,
                c.Products.Count(p => p.IsActive)))
            .ToListAsync(ct);

        return categories;
    }

    public async Task<CategoryDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.IsActive,
                c.Products.Count(p => p.IsActive)))
            .FirstOrDefaultAsync(ct);

        return category;
    }

    public async Task<Result<int>> CreateAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<int>.Fail("Name is required.");

        var slug = name.ToLower().Replace(" ", "-");

        var exists = await _db.Categories
            .AnyAsync(c => c.Name == name || c.Slug == slug, ct);

        if (exists)
            return Result<int>.Fail("Category with this name already exists.");

        var category = new Category
        {
            Name = name,
            Slug = slug,
            IsActive = true
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);

        return Result<int>.Ok(category.Id);
    }

    public async Task<Result> UpdateAsync(int id, string name, bool isActive, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, ct);

        if (category == null)
            return Result.NotFoundResult("Category not found.");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Name is required.");

        var slug = name.ToLower().Replace(" ", "-");

        var duplicate = await _db.Categories
            .AnyAsync(c => c.Id != id && (c.Name == name || c.Slug == slug), ct);

        if (duplicate)
            return Result.Fail("Category with this name already exists.");

        category.Name = name;
        category.Slug = slug;
        category.IsActive = isActive;

        await _db.SaveChangesAsync(ct);

        return Result.Ok();
    }

    public async Task<int> CountActiveProductsAsync(int categoryId, CancellationToken ct = default)
    {
        return await _db.Products
            .CountAsync(p => p.CategoryId == categoryId && p.IsActive, ct);
    }
}
