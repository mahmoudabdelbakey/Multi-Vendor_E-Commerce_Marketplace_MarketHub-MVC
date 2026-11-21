using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Constants;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

[Authorize(Roles = Roles.Customer)]
public sealed class CartController : AppControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId, ct);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(CartQuantityViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            Flash(false, "Enter a valid cart item and a quantity from 1 to 99.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _cartService.UpdateQuantityAsync(
            CurrentUserId, model.CartItemId, model.Quantity, ct);

        return RedirectWithResult(result, nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(RemoveCartItemViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            Flash(false, "Select a valid cart item to remove.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _cartService.RemoveAsync(CurrentUserId, model.CartItemId, ct);
        return RedirectWithResult(result, nameof(Index));
    }
}