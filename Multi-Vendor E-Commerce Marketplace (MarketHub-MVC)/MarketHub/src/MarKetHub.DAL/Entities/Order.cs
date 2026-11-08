using MarKetHub.DAL.Enums;

namespace MarketHub.DAL.Entities;

/// <summary>A customer order. It may contain items from several vendors (Approach A: Order + OrderItems).</summary>
public class Order
{
    public int Id { get; set; }

    /// <summary>Human readable unique number, e.g. MH-20261007-483920.</summary>
    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;
    public ApplicationUser Customer { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";

    // Shipping address snapshot (taken at order time).
    public string ShippingFullName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
