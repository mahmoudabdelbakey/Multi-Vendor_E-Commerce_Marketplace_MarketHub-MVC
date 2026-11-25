using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketHub.MVC.Controllers;

[Authorize]
[Route("Reviews")]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        int productId,
        int rating,
        string? comment,
        CancellationToken ct)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(customerId))
            return Challenge();

        var result = await _reviewService.AddAsync(
            customerId, productId, rating, comment, ct);

        if (!result.Success)
        {
            TempData["ReviewError"] = result.Message;
            return Redirect($"/Products/Details/{productId}#reviews");
        }

        TempData["ReviewSuccess"] = result.Message;

        var details = Url.Action("Details", "Catalog", new { id = productId }) ?? "/Products";
        return Redirect(details + "#reviews");
    }
}