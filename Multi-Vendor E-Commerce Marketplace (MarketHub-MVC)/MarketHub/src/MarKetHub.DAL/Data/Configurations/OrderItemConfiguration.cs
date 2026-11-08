using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems", t =>
        {
            t.HasCheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_OrderItems_LineTotal", "[LineTotal] = [UnitPrice] * [Quantity]");
        });

        b.Property(i => i.ProductNameSnapshot).HasMaxLength(150).IsRequired();
        b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(i => i.LineTotal).HasColumnType("decimal(18,2)");
        b.Property(i => i.FulfillmentStatus).HasConversion<string>().HasMaxLength(20);

        // Only Order -> OrderItems cascades. Everything else is Restrict to avoid multiple cascade paths.
        b.HasOne(i => i.Order).WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(i => i.Vendor).WithMany()
            .HasForeignKey(i => i.VendorProfileId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(i => i.Product).WithMany()
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(i => new { i.VendorProfileId, i.OrderId });
        b.HasIndex(i => i.OrderId);
        b.HasIndex(i => i.ProductId);
    }
}
