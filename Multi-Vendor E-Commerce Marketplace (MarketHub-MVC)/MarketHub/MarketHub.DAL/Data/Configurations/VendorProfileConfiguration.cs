namespace MarketHub.DAL.Data.Configurations
{
    public class VendorProfileConfiguration : IEntityTypeConfiguration<VendorProfile>
    {
        public void Configure(EntityTypeBuilder<VendorProfile> builder)
        {
            builder.ToTable("VendorProfiles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.StoreName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Description)
                .HasMaxLength(1000);

            builder.Property(v => v.LogoPath)
                .HasMaxLength(260);

            builder.Property(v => v.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(VendorStatus.Pending)
                .IsRequired();

            builder.Property(v => v.RejectionReason)
                .HasMaxLength(500);

            builder.Property(v => v.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(v => v.ReviewedAt)
                .HasColumnType("datetime2");

            builder.HasIndex(v => v.UserId)
                .IsUnique();

            builder.HasIndex(v => v.StoreName)
                .IsUnique();

            builder.HasIndex(v => v.Status);

            builder.HasOne(v => v.User)
                .WithOne(u => u.VendorProfile)
                .HasForeignKey<VendorProfile>(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ReviewedByUser)
                .WithMany()
                .HasForeignKey(v => v.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
