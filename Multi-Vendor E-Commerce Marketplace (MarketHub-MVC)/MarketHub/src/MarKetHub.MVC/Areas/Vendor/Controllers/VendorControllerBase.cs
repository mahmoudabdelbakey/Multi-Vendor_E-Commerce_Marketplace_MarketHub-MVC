using MarketHub.BLL.Interfaces;
using MarketHub.MVC.Authorization;
using MarketHub.MVC.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarketHub.MVC.Areas.Vendor.Controllers;

/// <summary>
/// TASK-19. Base class of EVERY controller in the Vendor area (Products, and later Dashboard / Store / Orders).
///  - [Authorize(ApprovedVendor)]: the whole area is closed to everybody else.
///  - <see cref="VendorId"/> is resolved ONCE per request from the login cookie through ICurrentVendorService.
///    It is never read from a form, a query string or a route value, so one vendor can not reach another vendor's data.
///  - A user who still has the Vendor role but is no longer an approved vendor (suspended) gets 403 instead of a crash.
///    (The policy only checks the role until TASK-30 makes it check the database.)
/// </summary>
[Area("Vendor")]
[Authorize(Policy = PolicyNames.ApprovedVendor)]
public abstract class VendorControllerBase : AppControllerBase
{
    private readonly ICurrentVendorService _currentVendor;

    protected VendorControllerBase(ICurrentVendorService currentVendor)
    {
        _currentVendor = currentVendor;
    }

    /// <summary>The VendorProfile id of the logged-in APPROVED vendor.</summary>
    protected int VendorId { get; private set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            VendorId = await _currentVendor.GetVendorProfileIdAsync(HttpContext.RequestAborted);
        }
        catch (InvalidOperationException)
        {
            // "The current user is not an approved vendor." -> /Account/AccessDenied
            context.Result = Forbid();
            return;
        }

        await next();
    }
}
