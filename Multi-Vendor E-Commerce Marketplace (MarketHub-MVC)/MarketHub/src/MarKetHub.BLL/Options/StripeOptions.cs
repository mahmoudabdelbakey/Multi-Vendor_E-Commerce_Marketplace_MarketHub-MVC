namespace MarketHub.BLL.Options;

public class StripeOptions
{
    public const string SectionName = "Stripe";

    /// <summary>sk_test_... (user-secrets / environment variable only).</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>whsec_... Signing secret of the webhook endpoint (or of "stripe listen" locally).</summary>
    public string WebhookSecret { get; set; } = string.Empty;

    public string PublishableKey { get; set; } = string.Empty;

    /// <summary>Stripe requires a Checkout Session to live at least 30 minutes.</summary>
    public int CheckoutExpiryMinutes { get; set; } = 30;
}
