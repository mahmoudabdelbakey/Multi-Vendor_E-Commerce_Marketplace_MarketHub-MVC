namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>
/// OWNER: Member C (vendor products, images, file storage). Register YOUR services here and only here, for example:
///   services.AddScoped&lt;IProductService, ProductService&gt;();
///   services.AddSingleton&lt;IFileStorage, LocalFileStorage&gt;();
/// </summary>
public static class VendorProductsFeature
{
    public static IServiceCollection AddVendorProductsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
