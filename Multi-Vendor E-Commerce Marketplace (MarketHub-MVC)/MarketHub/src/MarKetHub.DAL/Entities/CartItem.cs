namespace MarketHub.DAL.Entities;

/// <summary>A cart line. There is intentionally NO price column (BR-03): price is always read from Products.</summary>
public class CartItem
{
    public int Id { get; set; }

    public string CustomerId { get; set; } = string.Empty;
    public ApplicationUser Customer { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
