using MarketHub.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketHub.DAL;

public static class DependencyInjection
{
    /// <summary>Registers the DbContext. Identity itself is configured by the presentation layer (cookies live there).</summary>
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
