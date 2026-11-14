using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Services;

namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>
/// OWNER: Member C (vendor products, images, file storage). Register YOUR services here and only here.
/// </summary>
public static class VendorProductsFeature
{
    public static IServiceCollection AddVendorProductsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        // wwwroot is resolved when the service is first needed, so Program.cs does not have to pass anything.
        services.AddSingleton(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            return new FileStorageOptions
            {
                WebRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot")
            };
        });

        services.AddSingleton<IFileStorage, LocalFileStorage>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}