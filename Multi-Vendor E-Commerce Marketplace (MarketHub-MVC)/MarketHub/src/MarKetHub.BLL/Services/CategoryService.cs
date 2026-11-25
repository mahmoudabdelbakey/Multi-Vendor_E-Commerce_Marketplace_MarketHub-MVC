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
        name = (name ?? string.Empty).Trim();
        if (name.Length == 0)
            return Result<int>.Fail("Name is required.");

        if (name.Length > MaxNameLength)
            return Result<int>.Fail($"The name can have at most {MaxNameLength} characters.");

        var slug = Slugify(name);
        if (slug.Length == 0)
            return Result<int>.Fail("The name must contain letters or digits.");

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

        name = (name ?? string.Empty).Trim();
        if (name.Length == 0)
            return Result.Fail("Name is required.");

        if (name.Length > MaxNameLength)
            return Result.Fail($"The name can have at most {MaxNameLength} characters.");

        var slug = Slugify(name);
        if (slug.Length == 0)
            return Result.Fail("The name must contain letters or digits.");

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

    private const int MaxNameLength = 80;   // same as the database column
    private const int MaxSlugLength = 100;  // same as the database column

    internal static string Slugify(string name)
    {
        var sb = new System.Text.StringBuilder();
        var lastWasDash = true; // true at the start: no leading dash
        foreach (var ch in name.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
                lastWasDash = false;
            }
            else if (!lastWasDash)
            {
                sb.Append('-');
                lastWasDash = true;
            }
        }

        var slug = sb.ToString().Trim('-');
        return slug.Length > MaxSlugLength ? slug[..MaxSlugLength].Trim('-') : slug;
    }
    public async Task<int> CountActiveProductsAsync(int categoryId, CancellationToken ct = default)
    {
        return await _db.Products
            .CountAsync(p => p.CategoryId == categoryId && p.IsActive, ct);
    }
}
