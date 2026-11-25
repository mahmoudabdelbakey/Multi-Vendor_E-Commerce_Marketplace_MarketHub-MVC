using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IStripeWebhookService _webhookService;

    public WebhookController(IStripeWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpPost("stripe")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Stripe(CancellationToken ct)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        try
        {
            await _webhookService.HandleAsync(json, signature, ct);
            return Ok();
        }
        catch (Stripe.StripeException)
        {
            return BadRequest();
        }
    }
}
