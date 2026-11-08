using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class StripeWebhookEventConfiguration : IEntityTypeConfiguration<StripeWebhookEvent>
{
    public void Configure(EntityTypeBuilder<StripeWebhookEvent> b)
    {
        b.ToTable("StripeWebhookEvents");

        b.Property(e => e.StripeEventId).HasMaxLength(255).IsRequired();
        b.Property(e => e.EventType).HasMaxLength(100).IsRequired();

        b.HasIndex(e => e.StripeEventId).IsUnique();
    }
}
