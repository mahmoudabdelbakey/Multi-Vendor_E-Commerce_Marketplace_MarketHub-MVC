using Microsoft.AspNetCore.Identity;

namespace MarketHub.DAL.Entities;

/// <summary>Identity user (AspNetUsers) with the extra columns from the data dictionary.</summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>Soft-ban flag. Users are never physically deleted (they own financial history).</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public VendorProfile? VendorProfile { get; set; }
}
