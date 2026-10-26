using MarketHub.BLL.Interfaces;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ICategoryService _categoryService;

    public HomeController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var categories =
            await _categoryService.GetActiveCategoriesAsync(
                cancellationToken);

        var viewModel = new HomeViewModel
        {
            Title = "Welcome to MarketHub",
            Subtitle =
                "Discover products from multiple stores in one place.",

            Categories = categories
                .Select(category => new HomeCategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug
                })
                .ToList()
        };

        return View(viewModel);
    }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
}
