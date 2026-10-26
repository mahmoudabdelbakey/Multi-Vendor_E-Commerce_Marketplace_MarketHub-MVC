namespace MarketHub.DAL.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ImagePath)
                .HasMaxLength(260)
                .IsRequired();

            builder.Property(i => i.IsPrimary)
                .HasDefaultValue(false);

            builder.Property(i => i.SortOrder)
                .HasDefaultValue(0);

            builder.HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => new
            {
                i.ProductId,
                i.SortOrder
            });

            builder.HasIndex(i => i.ProductId)
                .IsUnique()
                .HasFilter("[IsPrimary] = 1");
        }
    }
}
