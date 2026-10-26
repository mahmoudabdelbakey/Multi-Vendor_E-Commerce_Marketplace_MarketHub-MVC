using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Data;
using MarketHub.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

}

