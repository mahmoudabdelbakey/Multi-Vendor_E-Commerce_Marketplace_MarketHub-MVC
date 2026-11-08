using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketHub.DAL.Data;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> b)
    {
        b.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        // No HasDefaultValue(true) on IsActive: EF treats "false" as "not set" for a bool with a database default,
        // so a deactivated user could silently be inserted as active. The C# initializer already defaults it to true.
        b.Property(u => u.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
