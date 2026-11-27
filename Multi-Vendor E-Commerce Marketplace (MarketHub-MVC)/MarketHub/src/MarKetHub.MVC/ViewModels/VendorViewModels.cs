using System.ComponentModel.DataAnnotations;
using MarketHub.BLL.Models;

namespace MarketHub.MVC.ViewModels;

/// <summary>TASK-28. The "Sell on MarketHub" form. There is no role or status field on purpose (over-posting).</summary>
public class ApplyVendorViewModel
{
    [Required(ErrorMessage = "Please enter a store name."),
     StringLength(100, MinimumLength = 3, ErrorMessage = "The store name must be between 3 and 100 characters."),
     Display(Name = "Store name")]
    public string StoreName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "The description cannot be longer than 1000 characters.")]
    public string? Description { get; set; }
}

/// <summary>TASK-28. One page, two states: no application yet (show the form) or an application (show its status).</summary>
public class VendorStatusViewModel
{
    public VendorDto? Vendor { get; init; }
    public ApplyVendorViewModel Form { get; init; } = new();
}
