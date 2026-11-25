using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

public class ProductsController : AdminControllerBase
{
    private readonly IAdminService _adminService;

    public ProductsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, CancellationToken ct = default)
    {
        var products = await _adminService.ListProductsAsync(search, page, 20, ct);
        ViewBag.Search = search;
        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hide(int id, CancellationToken ct)
    {
        var result = await _adminService.SetProductActiveAsync(id, false, ct);

        if (!result.Success)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Product hidden.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Show(int id, CancellationToken ct)
    {
        var result = await _adminService.SetProductActiveAsync(id, true, ct);

        if (!result.Success)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Product shown.";

        return RedirectToAction(nameof(Index));
    }
}
