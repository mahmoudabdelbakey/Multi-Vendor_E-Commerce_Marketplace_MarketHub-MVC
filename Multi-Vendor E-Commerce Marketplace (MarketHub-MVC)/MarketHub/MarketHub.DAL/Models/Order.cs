namespace MarketHub.DAL.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "USD";

        public string ShippingFullName { get; set; } = string.Empty;

        public string ShippingPhone { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string ShippingCity { get; set; } = string.Empty;

        public string ShippingCountry { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public ApplicationUser Customer { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();
    }
}
