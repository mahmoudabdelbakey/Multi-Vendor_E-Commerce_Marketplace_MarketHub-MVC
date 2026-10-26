using MarketHub.DAL.Models;

namespace MarketHub.BLL.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(
    CancellationToken cancellationToken = default);
}
