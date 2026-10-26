namespace MarketHub.DAL.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Provider)
                .HasMaxLength(20)
                .HasDefaultValue("Stripe")
                .IsRequired();

            builder.Property(p => p.StripeCheckoutSessionId)
                .HasMaxLength(255);

            builder.Property(p => p.StripePaymentIntentId)
                .HasMaxLength(255);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Currency)
                .HasColumnType("char(3)")
                .HasDefaultValue("USD")
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.FailureMessage)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(p => p.CompletedAt)
                .HasColumnType("datetime2");

            builder.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.StripeCheckoutSessionId)
                .IsUnique()
                .HasFilter("[StripeCheckoutSessionId] IS NOT NULL");

            builder.HasIndex(p => p.StripePaymentIntentId);
        }
    }
}
