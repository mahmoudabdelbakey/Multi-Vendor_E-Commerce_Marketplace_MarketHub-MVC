using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketHub.DAL.Data.Configurations
{
    public class StripeWebhookEventConfiguration : IEntityTypeConfiguration<StripeWebhookEvent>
    {
        public void Configure(EntityTypeBuilder<StripeWebhookEvent> builder)
        {
            builder.ToTable("StripeWebhookEvents");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.StripeEventId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.ProcessedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(e => e.StripeEventId)
                .IsUnique();
        }
    }
}
