using System.Diagnostics;
using MarketHub.BLL.Interfaces;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

/// <summary>
/// Home page and error handling.
/// </summary>
[AllowAnonymous]
public class HomeController : AppControllerBase
{
    private readonly ICatalogService _catalogService;

    public HomeController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            Categories = await _catalogService.GetActiveCategoriesAsync(),
            LatestProducts = await _catalogService.GetFeaturedAsync(8)
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        Response.StatusCode = 500;
        var model = new ErrorViewModel
        {
            StatusCode = 500,
            Title = "Internal server error",
            Message = "Something went wrong on our end. Please try again in a few moments.",
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View("Error", model);
    }

    /// <summary>Re-executed by UseStatusCodePagesWithReExecute for 401/403/404/500 responses that have no body.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Status(int code)
    {
        var model = code switch
        {
            404 => new ErrorViewModel
            {
                StatusCode = 404,
                Title = "Page not found",
                Message = "The page you are looking for might have been removed, renamed, or is temporarily unavailable."
            },
            403 => new ErrorViewModel
            {
                StatusCode = 403,
                Title = "Access denied",
                Message = "You do not have permission to view this page. Try signing in with an authorized account."
            },
            500 => new ErrorViewModel
            {
                StatusCode = 500,
                Title = "Internal server error",
                Message = "Something went wrong on our end. Please try again in a few moments.",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            },
            _ => new ErrorViewModel
            {
                StatusCode = code,
                Title = "Something went wrong",
                Message = "The request could not be completed. Please try again later."
            }
        };

        Response.StatusCode = code;
        return View("Error", model);
    }
}