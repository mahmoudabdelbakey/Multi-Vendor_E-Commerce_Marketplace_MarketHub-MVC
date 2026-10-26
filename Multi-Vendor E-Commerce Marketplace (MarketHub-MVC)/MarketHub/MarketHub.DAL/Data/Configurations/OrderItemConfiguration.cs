namespace MarketHub.DAL.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderItems_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_OrderItems_UnitPrice",
                    "[UnitPrice] > 0");

                table.HasCheckConstraint(
                    "CK_OrderItems_LineTotal",
                    "[LineTotal] = [UnitPrice] * [Quantity]");

            });

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductNameSnapshot)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.LineTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.FulfillmentStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(i => i.ShippedAt)
                .HasColumnType("datetime2");

            builder.Property(i => i.DeliveredAt)
                .HasColumnType("datetime2");

            builder.HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Vendor)
                .WithMany()
                .HasForeignKey(i => i.VendorProfileId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(i => new
            {
                i.VendorProfileId,
                i.OrderId
            });

            builder.HasIndex(i => i.ProductId);
        }
    }
}
