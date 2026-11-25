using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace MarketHub.BLL.Services;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly ApplicationDbContext _db;
    private readonly string _webhookSecret;

    public StripeWebhookService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Stripe:WebhookSecret not configured");
    }

    public async Task HandleAsync(string rawJson, string signatureHeader, CancellationToken ct = default)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(rawJson, signatureHeader, _webhookSecret);
        }
        catch (StripeException)
        {
            throw;
        }

        var exists = await _db.StripeWebhookEvents
            .AnyAsync(e => e.StripeEventId == stripeEvent.Id, ct);

        if (exists)
            return;

        var webhookEvent = new StripeWebhookEvent
        {
            StripeEventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            ProcessedAt = DateTime.UtcNow
        };

        _db.StripeWebhookEvents.Add(webhookEvent);
        await _db.SaveChangesAsync(ct);
    }
}
