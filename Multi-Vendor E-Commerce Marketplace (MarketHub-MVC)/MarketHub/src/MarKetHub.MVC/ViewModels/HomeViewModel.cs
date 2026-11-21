using MarketHub.BLL.Models;

namespace MarketHub.MVC.ViewModels
{
    public class HomeViewModel
    {
        public IReadOnlyList<CategoryDto> Categories { get; set; } = [];

        public IReadOnlyList<ProductListItemDto> LatestProducts { get; set; } = [];
    }
}