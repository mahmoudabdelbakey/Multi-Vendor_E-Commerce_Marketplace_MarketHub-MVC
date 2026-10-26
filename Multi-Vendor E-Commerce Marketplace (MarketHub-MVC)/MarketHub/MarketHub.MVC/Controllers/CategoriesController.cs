using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /Categories
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetActiveCategoriesAsync(cancellationToken);

        return View(categories);
    }

}

