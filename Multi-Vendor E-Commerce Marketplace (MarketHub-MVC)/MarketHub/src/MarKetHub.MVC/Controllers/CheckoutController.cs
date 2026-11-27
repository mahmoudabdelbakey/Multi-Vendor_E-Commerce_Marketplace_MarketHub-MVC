using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Constants;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

/// <summary>TASK-40: Shipping details capture and payment result pages.</summary>
[Authorize(Roles = Roles.Customer)]
public sealed class CheckoutController : AppControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICheckoutService _checkoutService;
    private readonly IOrderService _orderService;

    public CheckoutController(
        ICartService cartService,
        ICheckoutService checkoutService,
        IOrderService orderService)
    {
        _cartService = cartService;
        _checkoutService = checkoutService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId, ct);
        if (cart.IsEmpty)
        {
            Flash(false, "Your cart is empty.");
            return RedirectToAction("Index", "Cart");
        }

        if (cart.HasUnavailable)
        {
            Flash(false, "Please remove unavailable items or adjust quantities before checking out.");
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel { Cart = cart });
    }

    [HttpPost]
    public async Task<IActionResult> Index(CheckoutViewModel model, CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId, ct);
        if (cart.IsEmpty)
        {
            Flash(false, "Your cart is empty.");
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            model.Cart = cart;
            return View(model);
        }

        var successBase = Url.Action(nameof(Success), "Checkout", null, Request.Scheme)
            ?? "/Checkout/Success";
        var cancelBase = Url.Action(nameof(Cancel), "Checkout", null, Request.Scheme)
            ?? "/Checkout/Cancel";

        var successUrl = $"{successBase}?orderId={{ORDER_ID}}";
        var cancelUrl = $"{cancelBase}?orderId={{ORDER_ID}}";

        var shipping = new ShippingInfo
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Address = model.Address,
            City = model.City,
            Country = model.Country
        };

        var result = await _checkoutService.CreateOrderAndSessionAsync(
            CurrentUserId, shipping, successUrl, cancelUrl, ct);

        if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
        {
            Flash(false, result.Message);
            var refreshedCart = await _cartService.GetCartAsync(CurrentUserId, ct);
            if (refreshedCart.IsEmpty)
            {
                return RedirectToAction("Index", "Cart");
            }

            model.Cart = refreshedCart;
            return View(model);
        }

        return Redirect(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Success(int orderId, CancellationToken ct)
    {
        if (orderId <= 0)
        {
            return NotFound();
        }

        var order = await _orderService.GetForCustomerAsync(CurrentUserId, orderId, ct);
        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Cancel(int orderId, CancellationToken ct)
    {
        if (orderId <= 0)
        {
            return NotFound();
        }

        var order = await _orderService.GetForCustomerAsync(CurrentUserId, orderId, ct);
        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }
}
