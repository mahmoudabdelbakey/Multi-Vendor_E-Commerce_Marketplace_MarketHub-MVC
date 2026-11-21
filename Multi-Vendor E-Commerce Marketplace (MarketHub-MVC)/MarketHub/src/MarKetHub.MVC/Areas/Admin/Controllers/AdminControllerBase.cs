using MarketHub.MVC.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PolicyNames.SuperAdminOnly)]
public abstract class AdminControllerBase : Controller
{
}
