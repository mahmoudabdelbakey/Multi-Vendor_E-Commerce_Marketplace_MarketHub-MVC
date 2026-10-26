namespace MarketHub.DAL.Models
{
    public class VendorProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? LogoPath { get; set; }

        public VendorStatus Status { get; set; } = VendorStatus.Pending;

        public string? RejectionReason { get; set; }

        public int? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ApplicationUser? ReviewedByUser { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}