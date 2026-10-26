namespace MarketHub.DAL.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; }

        public ApplicationUser Customer { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
