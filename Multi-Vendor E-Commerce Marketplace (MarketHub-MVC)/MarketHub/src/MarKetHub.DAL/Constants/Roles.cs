namespace MarketHub.DAL.Constants;

public static class Roles
{
    public const string Customer = "Customer";
    public const string Vendor = "Vendor";
    public const string SuperAdmin = "SuperAdmin";

    public static readonly string[] All = { Customer, Vendor, SuperAdmin };
}
