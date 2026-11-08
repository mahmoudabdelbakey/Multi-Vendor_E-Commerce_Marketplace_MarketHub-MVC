using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");

        b.Property(o => o.OrderNumber).HasMaxLength(30).IsRequired();
        b.Property(o => o.CustomerId).HasMaxLength(450).IsRequired();
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        b.Property(o => o.Currency).HasMaxLength(3).IsFixedLength().IsUnicode(false).IsRequired();

        b.Property(o => o.ShippingFullName).HasMaxLength(100).IsRequired();
        b.Property(o => o.ShippingPhone).HasMaxLength(30).IsRequired();
        b.Property(o => o.ShippingAddress).HasMaxLength(250).IsRequired();
        b.Property(o => o.ShippingCity).HasMaxLength(80).IsRequired();
        b.Property(o => o.ShippingCountry).HasMaxLength(80).IsRequired();

        b.HasOne(o => o.Customer).WithMany()
            .HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(o => o.OrderNumber).IsUnique();
        b.HasIndex(o => new { o.CustomerId, o.CreatedAt }).IsDescending(false, true);
        b.HasIndex(o => o.Status);
    }
}
