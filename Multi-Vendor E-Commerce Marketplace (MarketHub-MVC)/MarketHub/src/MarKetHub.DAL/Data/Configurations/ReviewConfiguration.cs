using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.ToTable("Reviews", t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5"));

        b.Property(r => r.CustomerId).HasMaxLength(450).IsRequired();
        b.Property(r => r.Comment).HasMaxLength(1000);

        b.HasOne(r => r.Product).WithMany()
            .HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(r => r.Customer).WithMany()
            .HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Restrict);

        // BR-12: one review per (customer, product).
        b.HasIndex(r => new { r.CustomerId, r.ProductId }).IsUnique();
    }
}
