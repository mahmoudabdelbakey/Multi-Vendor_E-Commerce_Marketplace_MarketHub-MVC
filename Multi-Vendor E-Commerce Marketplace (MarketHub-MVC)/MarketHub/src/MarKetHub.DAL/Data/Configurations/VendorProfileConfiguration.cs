using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class VendorProfileConfiguration : IEntityTypeConfiguration<VendorProfile>
{
    public void Configure(EntityTypeBuilder<VendorProfile> b)
    {
        b.ToTable("VendorProfiles");

        b.Property(v => v.StoreName).HasMaxLength(100).IsRequired();
        b.Property(v => v.Description).HasMaxLength(1000);
        b.Property(v => v.LogoPath).HasMaxLength(260);
        b.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(v => v.RejectionReason).HasMaxLength(500);
        b.Property(v => v.ReviewedByUserId).HasMaxLength(450);

        // 1-to-1 with the identity user. Never cascade from the user (financial history must survive).
        b.HasOne(v => v.User)
            .WithOne(u => u.VendorProfile)
            .HasForeignKey<VendorProfile>(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // The admin who reviewed the application.
        b.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(v => v.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(v => v.UserId).IsUnique();
        b.HasIndex(v => v.StoreName).IsUnique();
        b.HasIndex(v => v.Status);
    }
}
