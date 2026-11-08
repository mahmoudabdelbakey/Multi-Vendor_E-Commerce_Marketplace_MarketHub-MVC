namespace MarketHub.BLL.Models;

public record TopVendorDto(string StoreName, decimal GrossSales, decimal Commission);

public record DailyRevenueDto(DateTime Date, decimal Amount);

public record AdminDashboardDto(
    int TotalVendors, int PendingVendors, int ApprovedVendors, int SuspendedVendors,
    int TotalProducts, int ActiveProducts, int RegisteredUsers,
    int TotalOrders, int PaidOrders, decimal GrossRevenue, decimal TotalCommission,
    IReadOnlyList<TopVendorDto> TopVendors, IReadOnlyList<DailyRevenueDto> Last7Days);

public record CommissionSettingDto(int Id, decimal RatePercent, DateTime EffectiveFrom, bool IsActive);

public record AdminProductDto(
    int Id, string Name, string StoreName, string CategoryName, decimal Price, int StockQuantity, bool IsActive);

public record CommissionResult(decimal CommissionAmount, decimal VendorNetAmount);
