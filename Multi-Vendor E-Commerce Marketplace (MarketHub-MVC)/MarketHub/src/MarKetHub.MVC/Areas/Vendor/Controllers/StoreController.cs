using System.Security.Claims;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Vendor.Controllers;

/// <summary>
/// TASK-36. The vendor edits the own store profile (name + description).
/// The vendor id comes from <see cref="VendorControllerBase.VendorId"/> (login), never from the form.
/// The unique store name rule and the length rules live in VendorService; this controller only shows the Result.
/// </summary>
public class StoreController : VendorControllerBase
{
    private readonly IVendorService _vendors;

    public StoreController(IVendorService vendors, ICurrentVendorService currentVendor) : base(currentVendor)
    {
        _vendors = vendors;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken ct)
    {
        var vendor = await LoadOwnVendorAsync(ct);
        if (vendor is null)
        {
            return NotFound();
        }

        return View(new StoreProfileViewModel
        {
            StoreName = vendor.StoreName,
            Description = vendor.Description,
            StatusText = vendor.Status.ToString()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(StoreProfileViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return await ReShowAsync(model, ct);
        }

        var input = new StoreProfileInput { StoreName = model.StoreName, Description = model.Description };
        var result = await _vendors.UpdateProfileAsync(VendorId, input, ct);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            // Name already taken, wrong length ...: show it on the form and keep what the vendor typed.
            ModelState.AddModelError(string.Empty, result.Message);
            return await ReShowAsync(model, ct);
        }

        Flash(result);
        return RedirectToAction(nameof(Edit)); // Post-Redirect-Get
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>The own store, found by the logged-in user. Null if it is somehow not the store of VendorId.</summary>
    private async Task<VendorDto?> LoadOwnVendorAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var vendor = await _vendors.GetByUserIdAsync(userId, ct);
        return vendor is not null && vendor.Id == VendorId ? vendor : null;
    }

    private async Task<IActionResult> ReShowAsync(StoreProfileViewModel model, CancellationToken ct)
    {
        var vendor = await LoadOwnVendorAsync(ct);
        if (vendor is null)
        {
            return NotFound();
        }

        model.StatusText = vendor.Status.ToString();
        return View(model);
    }
}
