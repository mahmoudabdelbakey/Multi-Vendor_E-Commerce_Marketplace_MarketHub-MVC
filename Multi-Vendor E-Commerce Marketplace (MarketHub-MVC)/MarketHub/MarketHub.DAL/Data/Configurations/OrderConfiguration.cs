namespace MarketHub.DAL.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Currency)
                .HasColumnType("char(3)")
                .HasDefaultValue("USD")
                .IsRequired();

            builder.Property(o => o.ShippingFullName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.ShippingPhone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.ShippingAddress)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(o => o.ShippingCity)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.ShippingCountry)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(o => o.PaidAt)
                .HasColumnType("datetime2");

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.HasIndex(o => new
            {
                o.CustomerId,
                o.CreatedAt
            });

            builder.HasIndex(o => o.Status);

            builder.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
