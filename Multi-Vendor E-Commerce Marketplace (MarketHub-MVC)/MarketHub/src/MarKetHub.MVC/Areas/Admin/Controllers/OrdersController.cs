using MarketHub.BLL.Interfaces;
using MarKetHub.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Areas.Admin.Controllers;

public class OrdersController : AdminControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(OrderStatus? status, int page = 1, CancellationToken ct = default)
    {
        var orders = await _orderService.ListAllAsync(status, page, 20, ct);
        ViewBag.CurrentStatus = status;
        return View(orders);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var order = await _orderService.GetForAdminAsync(id, ct);
        if (order == null)
            return NotFound();

        return View(order);
    }
}
