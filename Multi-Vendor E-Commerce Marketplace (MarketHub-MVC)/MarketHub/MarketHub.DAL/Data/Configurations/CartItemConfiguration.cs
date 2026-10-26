namespace MarketHub.DAL.Data.Configurations
{
    public class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_CartItems_Quantity",
                    "[Quantity] > 0");
            });

            builder.HasKey(c => c.Id);

            builder.Property(c => c.AddedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => new
            {
                c.CustomerId,
                c.ProductId
            })
            .IsUnique();
        }
    }
}
