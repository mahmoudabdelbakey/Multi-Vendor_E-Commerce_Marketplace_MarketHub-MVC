using System.ComponentModel.DataAnnotations;

namespace MarketHub.MVC.ViewModels;

public sealed class AddToCartViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a valid product.")]
    public int ProductId { get; set; }

    [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
    public int Quantity { get; set; } = 1;

    [StringLength(2048)]
    public string? ReturnUrl { get; set; }
}

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