using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Services;

namespace MarketHub.MVC.Infrastructure.Features;

public static class AdminPaymentsFeature
{
    public static IServiceCollection AddAdminPaymentsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommissionService, CommissionService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStripeWebhookService, StripeWebhookService>();

        return services;
    }
}
