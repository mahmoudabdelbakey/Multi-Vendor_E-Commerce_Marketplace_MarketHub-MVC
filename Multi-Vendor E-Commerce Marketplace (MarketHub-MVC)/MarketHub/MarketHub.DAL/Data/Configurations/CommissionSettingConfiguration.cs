namespace MarketHub.DAL.Data.Configurations
{
    public class CommissionSettingConfiguration : IEntityTypeConfiguration<CommissionSetting>
    {
        public void Configure(EntityTypeBuilder<CommissionSetting> builder)
        {
            builder.ToTable("CommissionSettings", table =>
            {
                table.HasCheckConstraint(
                    "CK_CommissionSettings_RatePercent",
                    "[RatePercent] >= 0 AND [RatePercent] <= 100");
            });

            builder.HasKey(c => c.Id);

            builder.Property(c => c.RatePercent)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.EffectiveFrom)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.IsActive)
                .IsUnique()
                .HasFilter("[IsActive] = 1");
        }
    }
}
