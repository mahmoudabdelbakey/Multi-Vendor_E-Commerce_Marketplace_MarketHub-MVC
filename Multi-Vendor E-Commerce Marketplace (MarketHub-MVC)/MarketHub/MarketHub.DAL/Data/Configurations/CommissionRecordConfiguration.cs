namespace MarketHub.DAL.Data.Configurations
{
    public class CommissionRecordConfiguration : IEntityTypeConfiguration<CommissionRecord>
    {
        public void Configure(EntityTypeBuilder<CommissionRecord> builder)
        {
            builder.ToTable("CommissionRecords", table =>
            {
                table.HasCheckConstraint(
                    "CK_CommissionRecords_Rate",
                    "[RatePercentSnapshot] >= 0 AND [RatePercentSnapshot] <= 100");

                table.HasCheckConstraint(
                    "CK_CommissionRecords_VendorNetAmount",
                    "[VendorNetAmount] = [GrossAmount] - [CommissionAmount]");

            });

            builder.HasKey(c => c.Id);

            builder.Property(c => c.GrossAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.RatePercentSnapshot)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.CommissionAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.VendorNetAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(c => c.OrderItem)
                .WithOne(i => i.CommissionRecord)
                .HasForeignKey<CommissionRecord>(c => c.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Vendor)
                .WithMany()
                .HasForeignKey(c => c.VendorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CommissionSetting)
                .WithMany(s => s.CommissionRecords)
                .HasForeignKey(c => c.CommissionSettingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.OrderItemId)
                .IsUnique();

            builder.HasIndex(c => c.VendorProfileId);
        }
    }
}
