using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Services;

namespace MarketHub.MVC.Infrastructure.Features;

public static class CatalogFeature
{
    public static IServiceCollection AddCatalogFeature(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}