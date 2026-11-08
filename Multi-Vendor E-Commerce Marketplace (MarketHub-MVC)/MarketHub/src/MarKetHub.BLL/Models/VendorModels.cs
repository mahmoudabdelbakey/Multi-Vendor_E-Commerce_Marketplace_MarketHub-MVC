using MarKetHub.DAL.Enums;

namespace MarketHub.BLL.Models;

public record VendorDto(
    int Id, string UserId, string StoreName, string? Description, string? LogoPath,
    VendorStatus Status, string? RejectionReason, DateTime CreatedAt, DateTime? ReviewedAt,
    string OwnerName, string OwnerEmail);

public class VendorApplicationInput
{
    public string StoreName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class StoreProfileInput
{
    public string StoreName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public record VendorDashboardDto(
    decimal GrossSales, decimal TotalCommission, decimal NetEarnings,
    int ProductCount, int ActiveProductCount,
    int PendingItems, int ShippedItems, int DeliveredItems,
    IReadOnlyList<VendorOrderItemDto> RecentItems);

public class ProductInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Base64 of Product.RowVersion, round-tripped through the edit form to detect concurrent edits.</summary>
    public string? RowVersion { get; set; }
}

public record ProductManageDto(
    int Id, string Name, decimal Price, int StockQuantity, bool IsActive,
    int CategoryId, string CategoryName, string? ImagePath);

public record ProductEditDto(int Id, ProductInput Input, IReadOnlyList<ProductImageDto> Images);

/// <summary>Framework-neutral description of an uploaded file (keeps ASP.NET types out of the BLL).</summary>
public record UploadedFile(string FileName, string ContentType, long Length, Stream Content);
