namespace MarketHub.DAL.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(c => c.Slug)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.HasIndex(c => c.Slug)
                .IsUnique();

            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

