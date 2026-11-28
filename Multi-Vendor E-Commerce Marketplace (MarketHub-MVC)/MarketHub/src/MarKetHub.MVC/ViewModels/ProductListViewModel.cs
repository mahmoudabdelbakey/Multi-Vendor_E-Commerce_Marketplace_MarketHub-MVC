using MarketHub.BLL.Common;
using MarketHub.BLL.Models;

namespace MarketHub.MVC.ViewModels;

/// <summary>TASK-37. The vendor's product list: one page of products plus the search text that produced it.</summary>
public class ProductListViewModel
{
    public PagedResult<ProductManageDto> Products { get; init; } = null!;

    /// <summary>The (trimmed) search text, or null when the list is not filtered.</summary>
    public string? Search { get; init; }
}
