using System.Security.Claims;
using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.MVC.ViewModels;
using MarKetHub.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

/// <summary>
/// TASK-29. Super Admin only (the policy comes from AdminControllerBase). List with a status filter, approve,
/// reject with a required reason, suspend, restore. Every change is a POST; the antiforgery filter is global.
/// </summary>
public class VendorApplicationsController : AdminControllerBase
{
    private const int PageSize = 15;

    private readonly IVendorAdminService _vendors;

    public VendorApplicationsController(IVendorAdminService vendors)
    {
        _vendors = vendors;
    }

    [HttpGet]
    public async Task<IActionResult> Index(VendorStatus? status, int page = 1, CancellationToken ct = default) =>
        View(new VendorApplicationsViewModel
        {
            Status = status,
            Result = await _vendors.ListAsync(status, page, PageSize, ct)
        });

    [HttpPost]
    public async Task<IActionResult> Approve(int id, VendorStatus? status, CancellationToken ct) =>
        Done(await _vendors.ApproveAsync(id, AdminId, ct), status);

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string? reason, VendorStatus? status, CancellationToken ct)
    {
        // The service checks the reason too; this early check gives the message without a database call.
        if (string.IsNullOrWhiteSpace(reason))
        {
            TempData["Error"] = "A rejection reason is required.";
            return RedirectToAction(nameof(Index), new { status });
        }

        return Done(await _vendors.RejectAsync(id, AdminId, reason.Trim(), ct), status);
    }

    [HttpPost]
    public async Task<IActionResult> Suspend(int id, VendorStatus? status, CancellationToken ct) =>
        Done(await _vendors.SuspendAsync(id, AdminId, ct), status);

    [HttpPost]
    public async Task<IActionResult> Restore(int id, VendorStatus? status, CancellationToken ct) =>
        Done(await _vendors.ReinstateAsync(id, AdminId, ct), status);

    // ------------------------------------------------------------------ helpers

    private string AdminId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("There is no authenticated user.");

    /// <summary>Not found -> 404. Otherwise flash the message and go back to the same filter (Post-Redirect-Get).</summary>
    private IActionResult Done(Result result, VendorStatus? status)
    {
        if (result.NotFound)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            TempData[result.Success ? "Success" : "Error"] = result.Message;
        }

        return RedirectToAction(nameof(Index), new { status });
    }
}
