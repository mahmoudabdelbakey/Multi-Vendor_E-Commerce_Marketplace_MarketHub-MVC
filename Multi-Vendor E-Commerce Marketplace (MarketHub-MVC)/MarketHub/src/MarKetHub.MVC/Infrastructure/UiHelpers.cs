using System.Globalization;
using MarKetHub.DAL.Enums;

namespace MarketHub.MVC.Infrastructure;

/// <summary>Small presentation helpers used by the Razor views.</summary>
public static class UiHelpers
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    /// <summary>MVP: one currency (USD), always formatted the same way regardless of the server culture.</summary>
    public static string Money(decimal amount) => amount.ToString("C", UsCulture);

    public static string Percent(decimal value) => value.ToString("0.##", UsCulture) + "%";

    public static string DateText(DateTime value) => value.ToString("yyyy-MM-dd HH:mm", UsCulture) + " UTC";

    public static string DateText(DateTime? value) => value is { } v ? DateText(v) : "-";

    public static string ImageUrl(string? relativePath) =>
        string.IsNullOrWhiteSpace(relativePath) ? "/img/placeholder.svg" : "/" + relativePath.TrimStart('/');

    public static string OrderBadge(OrderStatus status) => status switch
    {
        OrderStatus.PendingPayment => "text-bg-warning",
        OrderStatus.Paid => "text-bg-primary",
        OrderStatus.Completed => "text-bg-success",
        _ => "text-bg-secondary"
    };

    public static string FulfillmentBadge(FulfillmentStatus status) => status switch
    {
        FulfillmentStatus.Pending => "text-bg-warning",
        FulfillmentStatus.Shipped => "text-bg-info",
        FulfillmentStatus.Delivered => "text-bg-success",
        _ => "text-bg-secondary"
    };

    public static string VendorBadge(VendorStatus status) => status switch
    {
        VendorStatus.Pending => "text-bg-warning",
        VendorStatus.Approved => "text-bg-success",
        VendorStatus.Rejected => "text-bg-danger",
        _ => "text-bg-secondary"
    };

    public static string Stars(double rating)
    {
        var full = (int)Math.Round(rating, MidpointRounding.AwayFromZero);
        return new string('★', Math.Clamp(full, 0, 5)) + new string('☆', 5 - Math.Clamp(full, 0, 5));
    }
}
