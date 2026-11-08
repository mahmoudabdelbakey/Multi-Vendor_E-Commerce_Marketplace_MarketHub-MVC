using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");

        b.Property(c => c.Name).HasMaxLength(80).IsRequired();
        b.Property(c => c.Slug).HasMaxLength(100).IsRequired();

        b.HasIndex(c => c.Name).IsUnique();
        b.HasIndex(c => c.Slug).IsUnique();
    }
}
