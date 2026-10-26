namespace MarketHub.DAL.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", table =>
            {
                table.HasCheckConstraint(
                    "CK_Products_Price",
                    "[Price] > 0");

                table.HasCheckConstraint(
                    "CK_Products_Stock",
                    "[StockQuantity] >= 0");
            });

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(4000);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.StockQuantity)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.RowVersion)
                .IsRowVersion();

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2");

            builder.HasOne(p => p.Vendor)
                .WithMany(v => v.Products)
                .HasForeignKey(p => p.VendorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new
            {
                p.VendorProfileId,
                p.IsActive
            });

            builder.HasIndex(p => new
            {
                p.CategoryId,
                p.IsActive,
                p.Price
            });
        }
    }
}
