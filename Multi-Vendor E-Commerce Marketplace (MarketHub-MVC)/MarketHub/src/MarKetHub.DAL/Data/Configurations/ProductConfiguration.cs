using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products", t =>
        {
            t.HasCheckConstraint("CK_Products_Price", "[Price] > 0");
            t.HasCheckConstraint("CK_Products_Stock", "[StockQuantity] >= 0");
        });

        b.Property(p => p.Name).HasMaxLength(150).IsRequired();
        b.Property(p => p.Description).HasMaxLength(4000);
        b.Property(p => p.Price).HasColumnType("decimal(18,2)");
        b.Property(p => p.RowVersion).IsRowVersion();

        b.HasOne(p => p.Vendor).WithMany(v => v.Products)
            .HasForeignKey(p => p.VendorProfileId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(p => new { p.VendorProfileId, p.IsActive });
        b.HasIndex(p => new { p.CategoryId, p.IsActive, p.Price });
        b.HasIndex(p => p.Name);
    }
}
