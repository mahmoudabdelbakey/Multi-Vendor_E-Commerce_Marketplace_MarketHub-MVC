using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace MarketHub.BLL.Services;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly ApplicationDbContext _db;
    private readonly ICommissionService _commissionService;
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly string _webhookSecret;

    public StripeWebhookService(
        ApplicationDbContext db,
        ICommissionService commissionService,
        ILogger<StripeWebhookService> logger,
        IConfiguration configuration)
    {
        _db = db;
        _commissionService = commissionService;
        _logger = logger;
        _webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new InvalidOperationException("Stripe:WebhookSecret not configured");
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

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var webhookEvent = new StripeWebhookEvent
            {
                StripeEventId = stripeEvent.Id,
                EventType = stripeEvent.Type,
                ProcessedAt = DateTime.UtcNow
            };
            _db.StripeWebhookEvents.Add(webhookEvent);

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    await HandleCheckoutCompletedAsync(stripeEvent, ct);
                    break;

                case "checkout.session.expired":
                    await HandleCheckoutExpiredAsync(stripeEvent, ct);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailedAsync(stripeEvent, ct);
                    break;
            }

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task HandleCheckoutCompletedAsync(Event stripeEvent, CancellationToken ct)
    {
        var session = stripeEvent.Data.Object as Session;
        if (session == null) return;

        var orderId = int.Parse(session.Metadata["order_id"]);

        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null) return;

        if (order.Payment != null && order.Payment.Amount != session.AmountTotal / 100m)
        {
            _logger.LogWarning("Payment amount mismatch for order {OrderId}", orderId);
            return;
        }

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;

        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.Succeeded;
            order.Payment.StripePaymentIntentId = session.PaymentIntentId;
        }

        await _commissionService.CreateForOrderAsync(order, ct);
    }

    private async Task HandleCheckoutExpiredAsync(Event stripeEvent, CancellationToken ct)
    {
        var session = stripeEvent.Data.Object as Session;
        if (session == null) return;

        if (!session.Metadata.ContainsKey("order_id")) return;
        var orderId = int.Parse(session.Metadata["order_id"]);

        await CancelOrderAndReturnStockAsync(orderId, "Checkout session expired", ct);
    }

    private async Task HandlePaymentFailedAsync(Event stripeEvent, CancellationToken ct)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null) return;

        var payment = await _db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id, ct);

        if (payment == null) return;

        payment.Status = PaymentStatus.Failed;
        payment.FailureMessage = paymentIntent.LastPaymentError?.Message ?? "Payment failed";

        await CancelOrderAndReturnStockAsync(payment.Order.Id, "Payment failed", ct);
    }

    private async Task CancelOrderAndReturnStockAsync(int orderId, string reason, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null || order.Status == OrderStatus.Cancelled) return;

        order.Status = OrderStatus.Cancelled;

        foreach (var item in order.Items)
        {
            var product = await _db.Products.FindAsync(new object[] { item.ProductId }, ct);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
            }
            item.FulfillmentStatus = FulfillmentStatus.Cancelled;
        }
    }
}
