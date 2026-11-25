using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Options;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace MarketHub.BLL.Services;

/// <summary>
/// TASK-27 (receive and store). Verifies the Stripe signature on the RAW body, then stores the event id.
/// The UNIQUE index on StripeEventId makes a second delivery of the same event a harmless no-op.
/// This version does NOT change any order yet: marking an order Paid / Cancelled is added in Week 2
/// (TASK-43 / TASK-44) inside the "switch" below.
/// </summary>
public class StripeWebhookService : IStripeWebhookService
{
    private readonly ApplicationDbContext _db;
    private readonly StripeOptions _options;
    private readonly ILogger<StripeWebhookService> _logger;

    public StripeWebhookService(
        ApplicationDbContext db, IOptions<StripeOptions> options, ILogger<StripeWebhookService> logger)
    {
        _db = db;
        _options = options.Value;
        _logger = logger;
    }

    public async Task HandleAsync(string rawJson, string signatureHeader, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");
        }

        // Throws StripeException when the signature is wrong or the payload is malformed -> the controller answers 400.
        var stripeEvent = EventUtility.ConstructEvent(
            rawJson, signatureHeader, _options.WebhookSecret, throwOnApiVersionMismatch: false);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        _db.StripeWebhookEvents.Add(new StripeWebhookEvent
        {
            StripeEventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            ProcessedAt = DateTime.UtcNow
        });

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
        {
            _logger.LogInformation("Stripe event {EventId} was already received. Ignoring the duplicate.", stripeEvent.Id);
            return; // answer 200, no side effects
        }

        switch (stripeEvent.Type)
        {
            // Week 2: TASK-43 -> "checkout.session.completed" / "checkout.session.async_payment_succeeded" (mark Paid)
            //         TASK-44 -> "checkout.session.expired" / "checkout.session.async_payment_failed" (cancel + release stock)
            default:
                _logger.LogInformation("Stripe event {Type} ({EventId}) stored.", stripeEvent.Type, stripeEvent.Id);
                break;
        }

        await tx.CommitAsync(ct);
    }
}
