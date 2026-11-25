using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Options;
using MarketHub.BLL.Services;

namespace MarketHub.MVC.Infrastructure.Features;

public static class AdminPaymentsFeature
{
    public static IServiceCollection AddAdminPaymentsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommissionService, CommissionService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));
        services.AddScoped<IStripeWebhookService, StripeWebhookService>();
        return services;
    }
}
