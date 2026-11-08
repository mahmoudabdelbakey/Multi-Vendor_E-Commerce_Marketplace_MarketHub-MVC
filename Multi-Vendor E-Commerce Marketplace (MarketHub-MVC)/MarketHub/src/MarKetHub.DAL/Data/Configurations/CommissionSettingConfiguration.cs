using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class CommissionSettingConfiguration : IEntityTypeConfiguration<CommissionSetting>
{
    public void Configure(EntityTypeBuilder<CommissionSetting> b)
    {
        b.ToTable("CommissionSettings", t =>
            t.HasCheckConstraint("CK_CommissionSettings_Rate", "[RatePercent] >= 0 AND [RatePercent] <= 100"));

        b.Property(c => c.RatePercent).HasColumnType("decimal(5,2)");
        b.Property(c => c.CreatedByUserId).HasMaxLength(450);

        b.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(c => c.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        // At most ONE active row: a filtered unique index on a constant-true predicate.
        b.HasIndex(c => c.IsActive)
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
