using MarKetHub.DAL.Enums;

namespace MarketHub.DAL.Entities;

public class VendorProfile
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string StoreName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }

    public VendorStatus Status { get; set; } = VendorStatus.Pending;
    public string? RejectionReason { get; set; }

    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
