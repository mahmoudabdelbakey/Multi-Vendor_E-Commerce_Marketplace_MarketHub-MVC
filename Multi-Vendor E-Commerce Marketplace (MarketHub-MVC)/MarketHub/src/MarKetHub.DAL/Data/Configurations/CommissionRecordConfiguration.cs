using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class CommissionRecordConfiguration : IEntityTypeConfiguration<CommissionRecord>
{
    public void Configure(EntityTypeBuilder<CommissionRecord> b)
    {
        b.ToTable("CommissionRecords", t =>
            t.HasCheckConstraint("CK_CommissionRecords_Net", "[VendorNetAmount] = [GrossAmount] - [CommissionAmount]"));

        b.Property(c => c.GrossAmount).HasColumnType("decimal(18,2)");
        b.Property(c => c.RatePercentSnapshot).HasColumnType("decimal(5,2)");
        b.Property(c => c.CommissionAmount).HasColumnType("decimal(18,2)");
        b.Property(c => c.VendorNetAmount).HasColumnType("decimal(18,2)");
        b.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        // 1-to-1 with the order item (one commission record per paid item).
        b.HasOne(c => c.OrderItem).WithOne(i => i.CommissionRecord)
            .HasForeignKey<CommissionRecord>(c => c.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(c => c.Vendor).WithMany()
            .HasForeignKey(c => c.VendorProfileId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(c => c.Setting).WithMany()
            .HasForeignKey(c => c.CommissionSettingId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(c => c.OrderItemId).IsUnique();
        b.HasIndex(c => c.VendorProfileId);
    }
}
