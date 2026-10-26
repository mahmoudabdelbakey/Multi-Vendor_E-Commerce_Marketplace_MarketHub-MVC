namespace MarketHub.DAL.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews", table =>
            {
                table.HasCheckConstraint(
                    "CK_Reviews_Rating",
                    "[Rating] >= 1 AND [Rating] <= 5");
            });

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new
            {
                r.ProductId,
                r.CustomerId
            })
            .IsUnique();
        }
    }
}
