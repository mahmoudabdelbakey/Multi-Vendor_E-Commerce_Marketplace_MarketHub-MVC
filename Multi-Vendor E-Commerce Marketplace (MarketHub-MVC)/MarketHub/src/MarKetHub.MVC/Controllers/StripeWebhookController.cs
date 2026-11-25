using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MarketHub.MVC.Controllers;

/// <summary>
/// UC-31. Stripe calls this endpoint server-to-server, so there is no login and no antiforgery token:
/// the request is authenticated by its Stripe-Signature header instead (verified against the RAW body).
/// </summary>
[AllowAnonymous]
[IgnoreAntiforgeryToken]
[Route("stripe/webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly IStripeWebhookService _webhooks;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(IStripeWebhookService webhooks, ILogger<StripeWebhookController> logger)
    {
        _webhooks = webhooks;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> Post(CancellationToken ct)
    {
        // The signature is computed over the exact bytes Stripe sent: read the body as-is, never re-serialise it.
        string json;
        using (var reader = new StreamReader(Request.Body))
        {
            json = await reader.ReadToEndAsync(ct);
        }

        var signature = Request.Headers["Stripe-Signature"].ToString();
        if (string.IsNullOrEmpty(signature))
        {
            return BadRequest();
        }

        try
        {
            await _webhooks.HandleAsync(json, signature, ct);
            return Ok();
        }
        catch (StripeException ex)
        {
            // Wrong signature or malformed payload.
            _logger.LogWarning(ex, "Stripe webhook rejected (invalid signature or payload).");
            return BadRequest();
        }
        catch (WebhookProcessingException ex)
        {
            // Understood but refused (for example amount mismatch). Non-2xx makes Stripe retry and keeps the alert visible.
            _logger.LogError(ex, "Stripe webhook event refused: {Message}", ex.Message);
            return BadRequest();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Unexpected failure: the transaction was rolled back, Stripe will deliver the event again.
            _logger.LogError(ex, "Stripe webhook processing failed.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
