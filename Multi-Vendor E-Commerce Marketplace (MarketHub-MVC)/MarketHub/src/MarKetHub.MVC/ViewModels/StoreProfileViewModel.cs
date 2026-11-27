using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MarketHub.MVC.ViewModels;

/// <summary>
/// TASK-36. The vendor edits the public name and description of the own store.
/// No vendor id, status or logo field on purpose: the owner comes from ICurrentVendorService and the status is changed
/// only by a Super Admin, so nothing can be over-posted. Limits are the same as in VendorService / the database columns.
/// </summary>
public class StoreProfileViewModel
{
    [Required(ErrorMessage = "Please enter a store name."),
     StringLength(100, MinimumLength = 3, ErrorMessage = "The store name must be between 3 and 100 characters."),
     Display(Name = "Store name")]
    public string StoreName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "The description cannot be longer than 1000 characters.")]
    public string? Description { get; set; }

    /// <summary>Display only (Approved / Suspended ...). Filled by the controller, never bound.</summary>
    [ValidateNever, BindNever]
    public string StatusText { get; set; } = string.Empty;
}
