namespace MarketHub.BLL.Common;

/// <summary>A webhook event was understood but must be rejected (for example amount or currency mismatch).</summary>
public class WebhookProcessingException : Exception
{
    public WebhookProcessingException(string message) : base(message)
    {
    }
}
