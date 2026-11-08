namespace MarketHub.MVC.Authorization;

/// <summary>Policy names are constants so a typo is a compile error, not a silently open page.</summary>
public static class PolicyNames
{
    public const string ApprovedVendor = "ApprovedVendor";
    public const string SuperAdminOnly = "SuperAdminOnly";
}
