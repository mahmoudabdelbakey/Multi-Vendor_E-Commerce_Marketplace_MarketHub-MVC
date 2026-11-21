using System.Security.Claims;
using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

public class CommissionController : AdminControllerBase
{
    private readonly ICommissionService _commissionService;

    public CommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var current = await _commissionService.GetActiveAsync(ct);
        var history = await _commissionService.GetHistoryAsync(ct);

        ViewBag.CurrentRate = current;
        ViewBag.History = history;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRate(decimal ratePercent, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _commissionService.SetRateAsync(ratePercent, userId, ct);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
        }
        else
        {
            TempData["Success"] = "Commission rate updated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
