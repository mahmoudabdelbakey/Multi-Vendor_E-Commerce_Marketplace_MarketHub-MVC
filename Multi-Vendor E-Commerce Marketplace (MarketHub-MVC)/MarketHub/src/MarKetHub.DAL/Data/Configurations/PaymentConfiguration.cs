using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");

        b.Property(p => p.Provider).HasMaxLength(20).IsRequired();
        b.Property(p => p.StripeCheckoutSessionId).HasMaxLength(255);
        b.Property(p => p.StripePaymentIntentId).HasMaxLength(255);
        b.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        b.Property(p => p.Currency).HasMaxLength(3).IsFixedLength().IsUnicode(false).IsRequired();
        b.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(p => p.FailureMessage).HasMaxLength(500);

        b.HasOne(p => p.Order).WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);

        // The session id is NULL until Stripe creates it, so the unique index is filtered.
        b.HasIndex(p => p.StripeCheckoutSessionId)
            .IsUnique()
            .HasFilter("[StripeCheckoutSessionId] IS NOT NULL");
        b.HasIndex(p => p.StripePaymentIntentId);
        b.HasIndex(p => p.OrderId);
    }
}
