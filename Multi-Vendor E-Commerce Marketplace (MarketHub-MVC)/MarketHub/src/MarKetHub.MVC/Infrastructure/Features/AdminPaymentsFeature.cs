namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>
/// OWNER: Member E (commission, categories, Stripe webhook, admin area, vendor shipping). Register YOUR services here, for example:
///   services.AddScoped&lt;ICommissionService, CommissionService&gt;();
///   services.AddScoped&lt;IStripeWebhookService, StripeWebhookService&gt;();
/// </summary>
public static class AdminPaymentsFeature
{
    public static IServiceCollection AddAdminPaymentsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
