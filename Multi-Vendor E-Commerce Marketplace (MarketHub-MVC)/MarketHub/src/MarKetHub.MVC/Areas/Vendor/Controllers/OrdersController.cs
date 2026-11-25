using MarketHub.BLL.Interfaces;
using MarKetHub.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Vendor.Controllers;

public class OrdersController : VendorControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICurrentVendorService _currentVendorService;

    public OrdersController(IOrderService orderService, ICurrentVendorService currentVendorService)
    {
        _orderService = orderService;
        _currentVendorService = currentVendorService;
    }

    public async Task<IActionResult> Index(FulfillmentStatus? status, int page = 1, CancellationToken ct = default)
    {
        var vendorId = await _currentVendorService.GetVendorProfileIdAsync(ct);
        var items = await _orderService.ListItemsForVendorAsync(vendorId, status, page, 20, ct);

        ViewBag.CurrentStatus = status;
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ship(int itemId, CancellationToken ct)
    {
        var vendorId = await _currentVendorService.GetVendorProfileIdAsync(ct);
        var result = await _orderService.UpdateFulfillmentAsync(vendorId, itemId, FulfillmentStatus.Shipped, ct);

        if (!result.Success)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Item marked as shipped.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deliver(int itemId, CancellationToken ct)
    {
        var vendorId = await _currentVendorService.GetVendorProfileIdAsync(ct);
        var result = await _orderService.UpdateFulfillmentAsync(vendorId, itemId, FulfillmentStatus.Delivered, ct);

        if (!result.Success)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Item marked as delivered.";

        return RedirectToAction(nameof(Index));
    }
}
