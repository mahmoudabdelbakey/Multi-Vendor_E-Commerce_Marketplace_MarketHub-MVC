using System.ComponentModel.DataAnnotations;
using MarketHub.BLL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarketHub.MVC.ViewModels;

/// <summary>
/// Create / Edit form of a vendor product (TASK-19).
/// There is NO VendorProfileId field on purpose: the owner always comes from ICurrentVendorService, so a vendor can not
/// create or move a product into somebody else's store by posting an extra form value (over-posting / IDOR).
/// Shape and format are checked here (DataAnnotations); business rules (stock, ownership, images) are in ProductService.
/// </summary>
public class ProductFormViewModel
{
    /// <summary>Set by the controller from the route. Never bound from the request.</summary>
    [BindNever]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the product name."),
     StringLength(150, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 150 characters."),
     Display(Name = "Product name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "The description can not be longer than 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Please enter the price."),
     Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "The price must be between 0.01 and 1,000,000.")]
    [Display(Name = "Price (USD)")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Please enter the stock quantity."),
     Range(0, 1000000, ErrorMessage = "The stock must be between 0 and 1,000,000.")]
    [Display(Name = "Stock quantity")]
    public int? StockQuantity { get; set; }

    [Required(ErrorMessage = "Please choose a category.")]
    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Visible in the store")]
    public bool IsActive { get; set; } = true;

    /// <summary>Base64 of Product.RowVersion, round-tripped through the edit form (concurrent edit detection, TASK-35).</summary>
    public string? RowVersion { get; set; }

    /// <summary>New images chosen in the form. The type / size rules are checked by ProductService and IFileStorage.</summary>
    [Display(Name = "Add images")]
    public List<IFormFile>? NewImages { get; set; }

    // ---- display only: filled by the controller, never validated or trusted ----

    [ValidateNever, BindNever]
    public IReadOnlyList<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();

    [ValidateNever, BindNever]
    public IReadOnlyList<ProductImageDto> ExistingImages { get; set; } = Array.Empty<ProductImageDto>();
}
