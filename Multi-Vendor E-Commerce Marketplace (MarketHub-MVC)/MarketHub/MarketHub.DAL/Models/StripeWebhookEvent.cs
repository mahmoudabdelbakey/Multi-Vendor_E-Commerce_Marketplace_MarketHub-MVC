namespace MarketHub.DAL.Models
{
    public class StripeWebhookEvent
    {
        public int Id { get; set; }

        public string StripeEventId { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;

        public DateTime ProcessedAt { get; set; }
    }
}
