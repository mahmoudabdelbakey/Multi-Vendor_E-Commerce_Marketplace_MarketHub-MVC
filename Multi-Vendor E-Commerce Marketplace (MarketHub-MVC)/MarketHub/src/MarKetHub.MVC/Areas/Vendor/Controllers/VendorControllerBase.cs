using MarketHub.MVC.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Vendor.Controllers;

[Area("Vendor")]
[Authorize(Policy = PolicyNames.ApprovedVendor)]
public abstract class VendorControllerBase : Controller
{
}
