using MarKetHub.DAL.Enums;

namespace MarketHub.DAL.Entities;

/// <summary>A payment attempt for an order. Only Stripe identifiers are stored - never card data.</summary>
public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Provider { get; set; } = "Stripe";
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? FailureMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
