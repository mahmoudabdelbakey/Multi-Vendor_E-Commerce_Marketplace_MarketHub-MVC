namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>
/// OWNER: Member B (catalog, reviews). Register YOUR services here and only here, for example:
///   services.AddScoped&lt;ICatalogService, CatalogService&gt;();
/// </summary>
public static class CatalogFeature
{
    public static IServiceCollection AddCatalogFeature(this IServiceCollection services)
    {
        return services;
    }
}
