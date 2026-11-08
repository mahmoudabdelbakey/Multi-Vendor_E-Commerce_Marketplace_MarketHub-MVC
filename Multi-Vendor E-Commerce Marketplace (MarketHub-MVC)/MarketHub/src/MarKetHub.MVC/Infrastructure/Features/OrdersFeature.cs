namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>
/// OWNER: Member D (cart, checkout, orders, Stripe Checkout Session, cleanup job). Register YOUR services here, for example:
///   services.AddScoped&lt;ICartService, CartService&gt;();
///   services.AddHostedService&lt;PendingOrderCleanupService&gt;();
/// Week 2: register the fake payment (Infrastructure/Dev). Week 3: replace it with the real Stripe service.
/// </summary>
public static class OrdersFeature
{
    public static IServiceCollection AddOrdersFeature(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
