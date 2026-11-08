using System.Security.Claims;
using MarketHub.BLL.Common;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

/// <summary>Shared helpers: the current user id and one-line flash messages.</summary>
public abstract class AppControllerBase : Controller
{
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("There is no authenticated user.");

    protected void Flash(Result result) => Flash(result.Success, result.Message);

    protected void Flash(bool success, string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            TempData[success ? "Success" : "Error"] = message;
        }
    }

    /// <summary>Success -> redirect, "not found" -> 404 (never reveals whether someone else's data exists), else flash + redirect.</summary>
    protected IActionResult RedirectWithResult(Result result, string action, object? routeValues = null)
    {
        if (result.NotFound)
        {
            return NotFound();
        }

        Flash(result);
        return RedirectToAction(action, routeValues);
    }
}
