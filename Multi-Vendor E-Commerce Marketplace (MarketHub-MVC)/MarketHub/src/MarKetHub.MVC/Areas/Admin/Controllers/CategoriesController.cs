using MarketHub.BLL.Interfaces;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

/// <summary>
/// TASK-26. Super Admin manages the categories. The name of the controller is "Categories" because the Layout menu links to it.
/// Rules of the team: ModelState check -> call the service -> flash the Result -> redirect (Post-Redirect-Get).
/// </summary>
public class CategoriesController : AdminControllerBase
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _categories.ListAsync(ct);
        return View(list.OrderBy(c => c.Name).ToList());
    }

    [HttpGet]
    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _categories.CreateAsync(model.Name.Trim(), ct);
        if (!result.Success)
        {
            ModelState.AddModelError(nameof(model.Name), result.Message);
            return View(model);
        }

        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var category = await _categories.GetAsync(id, ct);
        if (category is null)
        {
            return NotFound();
        }

        return View(new CategoryFormViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive,
            WasActive = category.IsActive,
            ActiveProductCount = category.ProductCount
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model, CancellationToken ct)
    {
        model.Id = id;

        // Display data is read again from the database, never trusted from the form.
        var current = await _categories.GetAsync(id, ct);
        if (current is null)
        {
            return NotFound();
        }

        model.WasActive = current.IsActive;
        model.ActiveProductCount = await _categories.CountActiveProductsAsync(id, ct);

        // Hiding a category that still has visible products needs an explicit confirmation.
        if (current.IsActive && !model.IsActive && model.ActiveProductCount > 0 && !model.ConfirmHide)
        {
            ModelState.AddModelError(nameof(model.ConfirmHide),
                $"This category has {model.ActiveProductCount} active product(s). Tick the box to confirm hiding it.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _categories.UpdateAsync(id, model.Name.Trim(), model.IsActive, ct);
        if (result.NotFound)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            ModelState.AddModelError(nameof(model.Name), result.Message);
            return View(model);
        }

        TempData["Success"] = "Category saved.";
        return RedirectToAction(nameof(Index));
    }
}
