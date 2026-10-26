namespace MarketHub.DAL.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users", table =>
            {
                table.HasCheckConstraint(
                    "CK_Users_AccessFailedCount",
                    "[AccessFailedCount] >= 0");
            });

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.EmailConfirmed)
                .HasDefaultValue(false);

            builder.Property(u => u.PhoneNumberConfirmed)
                .HasDefaultValue(false);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.AccessFailedCount)
                .HasDefaultValue(0);

            builder.Property(u => u.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
