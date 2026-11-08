using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        b.ToTable("ProductImages");

        b.Property(i => i.ImagePath).HasMaxLength(260).IsRequired();

        // Images are meaningless without their product -> cascade.
        b.HasOne(i => i.Product).WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(i => i.ProductId);
    }
}
