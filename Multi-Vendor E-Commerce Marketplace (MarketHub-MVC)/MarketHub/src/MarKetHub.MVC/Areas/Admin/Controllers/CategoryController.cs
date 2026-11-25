using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

public class CategoryController : AdminControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var categories = await _categoryService.ListAsync(ct);
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(name, ct);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return View();
        }

        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var category = await _categoryService.GetAsync(id, ct);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, bool isActive, CancellationToken ct)
    {
        var activeProducts = await _categoryService.CountActiveProductsAsync(id, ct);

        if (!isActive && activeProducts > 0)
        {
            TempData["Error"] = $"Cannot hide category with {activeProducts} active products.";
            var category = await _categoryService.GetAsync(id, ct);
            return View(category);
        }

        var result = await _categoryService.UpdateAsync(id, name, isActive, ct);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            var category = await _categoryService.GetAsync(id, ct);
            return View(category);
        }

        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }
}
