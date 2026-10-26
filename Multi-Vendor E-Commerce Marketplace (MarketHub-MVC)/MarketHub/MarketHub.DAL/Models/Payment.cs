namespace MarketHub.DAL.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Provider { get; set; } = "Stripe";

        public string? StripeCheckoutSessionId { get; set; }

        public string? StripePaymentIntentId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public PaymentStatus Status { get; set; }

        public string? FailureMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
