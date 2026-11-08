using System.Diagnostics;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

/// <summary>
/// PLACEHOLDER from Member A so that the site opens and the error re-execution path works.
/// Member B owns this controller and replaces Index (week 1) and the Error / Status pages (week 2).
/// </summary>
[AllowAnonymous]
public class HomeController : AppControllerBase
{
    public IActionResult Index() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    /// <summary>Re-executed by UseStatusCodePagesWithReExecute for 401/403/404/... responses that have no body.</summary>
    public IActionResult Status(int code)
    {
        var model = code switch
        {
            404 => new ErrorViewModel { StatusCode = 404, Title = "Page not found", Message = "We could not find what you were looking for." },
            403 => new ErrorViewModel { StatusCode = 403, Title = "Access denied", Message = "You do not have permission to open this page." },
            _ => new ErrorViewModel { StatusCode = code, Title = "Something went wrong", Message = "The request could not be completed." }
        };

        Response.StatusCode = code;
        return View("Error", model);
    }
}
