using MarKetHub.DAL.Enums;

namespace MarketHub.DAL.Entities;

/// <summary>Immutable financial record: one per paid OrderItem.</summary>
public class CommissionRecord
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public int VendorProfileId { get; set; }
    public VendorProfile Vendor { get; set; } = null!;

    public int CommissionSettingId { get; set; }
    public CommissionSetting Setting { get; set; } = null!;

    public decimal GrossAmount { get; set; }
    public decimal RatePercentSnapshot { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal VendorNetAmount { get; set; }

    public CommissionStatus Status { get; set; } = CommissionStatus.Recorded;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
