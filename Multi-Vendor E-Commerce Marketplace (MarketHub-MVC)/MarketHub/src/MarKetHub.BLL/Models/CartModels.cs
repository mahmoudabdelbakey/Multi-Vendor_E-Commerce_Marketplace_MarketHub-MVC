namespace MarketHub.BLL.Models;

public record CartItemDto(
    int Id, int ProductId, string ProductName, string StoreName,
    decimal UnitPrice, int Quantity, int StockQuantity,
    bool IsAvailable, string? UnavailableReason, string? ImagePath)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartDto
{
    public IReadOnlyList<CartItemDto> Items { get; init; } = Array.Empty<CartItemDto>();

    /// <summary>Total of the purchasable lines only; unavailable items are flagged but not charged.</summary>
    public decimal Total => Items.Where(i => i.IsAvailable).Sum(i => i.LineTotal);
    public bool HasUnavailable => Items.Any(i => !i.IsAvailable);
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool IsEmpty => Items.Count == 0;
}
