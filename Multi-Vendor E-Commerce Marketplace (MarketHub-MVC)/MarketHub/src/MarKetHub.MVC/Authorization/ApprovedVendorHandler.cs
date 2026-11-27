using System.Security.Claims;
using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Constants;
using Microsoft.AspNetCore.Authorization;

namespace MarketHub.MVC.Authorization;

/// <summary>TASK-30. Marker for the policy: "this user is a vendor whose store is Approved right now".</summary>
public class ApprovedVendorRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// TASK-30. The role "Vendor" in the login cookie is NOT enough, because the cookie can be old: it still says
/// "Vendor" after the admin suspended the store. So the database is asked on EVERY request
/// (one small query, and the answer is never cached between requests).
/// Result: a suspended vendor is locked out of the vendor area on the very next click, without waiting for the
/// cookie to refresh. The catalog and the cart already filter by the vendor status, so the products disappear at once too.
/// </summary>
public class ApprovedVendorHandler : AuthorizationHandler<ApprovedVendorRequirement>
{
    private readonly IVendorService _vendors;

    public ApprovedVendorHandler(IVendorService vendors)
    {
        _vendors = vendors;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ApprovedVendorRequirement requirement)
    {
        if (!context.User.IsInRole(Roles.Vendor))
        {
            return; // no Succeed -> the policy fails (403 / access denied page)
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return;
        }

        if (await _vendors.GetApprovedVendorIdAsync(userId) is not null)
        {
            context.Succeed(requirement);
        }
    }
}
