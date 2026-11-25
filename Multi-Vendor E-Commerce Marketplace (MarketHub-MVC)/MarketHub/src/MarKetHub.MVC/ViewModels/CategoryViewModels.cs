using System.ComponentModel.DataAnnotations;

namespace MarketHub.MVC.ViewModels;

/// <summary>TASK-26. Create and edit form of a category. Max length 80 = the database column.</summary>
public sealed class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(80, ErrorMessage = "The name can have at most 80 characters.")]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>The admin must tick this to hide a category that still has visible products.</summary>
    public bool ConfirmHide { get; set; }

    // display only (never posted back as a trusted value)
    public int ActiveProductCount { get; set; }
    public bool WasActive { get; set; }
}
