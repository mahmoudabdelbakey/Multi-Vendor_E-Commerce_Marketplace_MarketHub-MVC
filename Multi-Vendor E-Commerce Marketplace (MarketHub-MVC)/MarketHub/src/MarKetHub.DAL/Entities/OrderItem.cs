using MarKetHub.DAL.Enums;

namespace MarketHub.DAL.Entities;

/// <summary>One line of an order. It belongs to exactly one vendor - this is the heart of the marketplace.</summary>
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Snapshot of the product's vendor at purchase time.</summary>
    public int VendorProfileId { get; set; }
    public VendorProfile Vendor { get; set; } = null!;

    // Snapshots: later price / name changes must not rewrite history.
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public FulfillmentStatus FulfillmentStatus { get; set; } = FulfillmentStatus.Pending;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public CommissionRecord? CommissionRecord { get; set; }
}
