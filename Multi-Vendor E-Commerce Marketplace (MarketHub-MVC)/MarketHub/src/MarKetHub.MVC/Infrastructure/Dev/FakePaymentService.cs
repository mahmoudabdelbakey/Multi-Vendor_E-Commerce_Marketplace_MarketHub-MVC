using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;

namespace MarketHub.MVC.Infrastructure.Dev;

/// <summary>
/// TASK-40 (Development payment stub). Implements IPaymentService without calling Stripe so checkout
/// and order flows can be tested end-to-end before TASK-49 wires Stripe Checkout Sessions.
/// Week 3: replace its registration in OrdersFeature with the real StripePaymentService.
/// </summary>
public sealed class FakePaymentService : IPaymentService
{
    private readonly ILogger<FakePaymentService> _logger;

    public FakePaymentService(ILogger<FakePaymentService> logger) => _logger = logger;

    public Task<PaymentSessionResult> CreateCheckoutSessionAsync(
        PaymentSessionRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sessionId = $"cs_fake_{request.OrderId}_{Guid.NewGuid():N}";
        var redirectUrl = string.IsNullOrWhiteSpace(request.SuccessUrl)
            ? $"/Checkout/Success?orderId={request.OrderId}"
            : request.SuccessUrl;

        _logger.LogInformation(
            "Fake payment session {SessionId} created for order {OrderId} ({OrderNumber}).",
            sessionId, request.OrderId, request.OrderNumber);

        return Task.FromResult(new PaymentSessionResult(sessionId, redirectUrl));
    }

    public Task<Result> ExpireSessionAsync(string sessionId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.FromResult(Result.Fail("Payment session id is required."));
        }

        _logger.LogInformation("Fake payment session {SessionId} expired.", sessionId);
        return Task.FromResult(Result.Ok("Fake payment session expired."));
    }
}
