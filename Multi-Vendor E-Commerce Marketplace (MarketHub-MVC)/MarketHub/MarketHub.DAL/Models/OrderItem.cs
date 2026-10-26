namespace MarketHub.DAL.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int VendorProfileId { get; set; }

        public string ProductNameSnapshot { get; set; }
            = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }

        public FulfillmentStatus FulfillmentStatus { get; set; }

        public DateTime? ShippedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public VendorProfile Vendor { get; set; } = null!;

        public CommissionRecord? CommissionRecord { get; set; }
    }
}
