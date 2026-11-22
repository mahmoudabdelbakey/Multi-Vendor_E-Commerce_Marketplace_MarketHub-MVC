using System.Security.Claims;
using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Constants;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.ViewComponents;

[ViewComponent(Name = "CartCount")]
public sealed class CartCountViewComponent : ViewComponent
{
    private readonly ICartService _cartService;

    public CartCountViewComponent(ICartService cartService) => _cartService = cartService;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (UserClaimsPrincipal.Identity?.IsAuthenticated != true ||
            !UserClaimsPrincipal.IsInRole(Roles.Customer))
        {
            return Content(string.Empty);
        }

        var customerId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Content(string.Empty);
        }

        var count = await _cartService.CountAsync(customerId);
        return View(count);
    }
}