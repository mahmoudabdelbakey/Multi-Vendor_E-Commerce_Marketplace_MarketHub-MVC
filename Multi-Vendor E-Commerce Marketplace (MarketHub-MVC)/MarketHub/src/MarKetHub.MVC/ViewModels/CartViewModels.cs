using System.ComponentModel.DataAnnotations;

namespace MarketHub.MVC.ViewModels;

public sealed class CartQuantityViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a valid cart item.")]
    public int CartItemId { get; set; }

    [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
    public int Quantity { get; set; }
}

public sealed class RemoveCartItemViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a valid cart item.")]
    public int CartItemId { get; set; }
}