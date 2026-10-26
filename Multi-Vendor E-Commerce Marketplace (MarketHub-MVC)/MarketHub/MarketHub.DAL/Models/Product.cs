namespace MarketHub.DAL.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int VendorProfileId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public VendorProfile Vendor { get; set; } = null!;

        public Category Category { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; }
            = new List<ProductImage>();
    }
}
