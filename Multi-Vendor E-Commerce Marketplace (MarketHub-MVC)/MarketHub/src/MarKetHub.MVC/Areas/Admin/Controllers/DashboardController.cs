using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

public class DashboardController : AdminControllerBase
{
    private readonly IAdminService _adminService;

    public DashboardController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var dashboard = await _adminService.GetDashboardAsync(ct);
        return View(dashboard);
    }
}
