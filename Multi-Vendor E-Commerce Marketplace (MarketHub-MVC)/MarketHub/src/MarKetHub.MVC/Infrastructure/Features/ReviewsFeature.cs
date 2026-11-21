using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Services;

namespace MarketHub.MVC.Infrastructure.Features;

public static class ReviewsFeature
{
    public static IServiceCollection AddReviewsFeature(this IServiceCollection services)
    {
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}