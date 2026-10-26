
namespace MarketHub.DAL.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<VendorProfile> VendorProfiles => Set<VendorProfile>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<CommissionSetting> CommissionSettings => Set<CommissionSetting>();

        public DbSet<CommissionRecord> CommissionRecords => Set<CommissionRecord>();

        public DbSet<StripeWebhookEvent> StripeWebhookEvents => Set<StripeWebhookEvent>();

        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
    }
}
