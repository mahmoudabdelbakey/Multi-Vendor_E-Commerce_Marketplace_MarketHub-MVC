namespace MarketHub.DAL.Entities;

/// <summary>Idempotency guard: the UNIQUE index on StripeEventId makes a second processing of the same event impossible.</summary>
public class StripeWebhookEvent
{
    public int Id { get; set; }
    public string StripeEventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
