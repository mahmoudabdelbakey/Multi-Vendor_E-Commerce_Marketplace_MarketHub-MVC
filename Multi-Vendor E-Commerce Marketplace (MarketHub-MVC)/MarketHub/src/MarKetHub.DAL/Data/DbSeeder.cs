using MarketHub.DAL.Constants;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarketHub.DAL.Data;

public class SeedOptions
{
    /// <summary>Super Admin credentials come from configuration (user-secrets / environment variables), never from code.</summary>
    public string? AdminEmail { get; set; }
    public string? AdminPassword { get; set; }

    /// <summary>Demo vendors / products / customers. Intended for Development and the online demo only.</summary>
    public bool SeedDemoData { get; set; }
    public string DemoPassword { get; set; } = "Demo@12345";

    public decimal DefaultCommissionPercent { get; set; } = 10m;
}

/// <summary>Idempotent seeder: running it twice never duplicates anything.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, SeedOptions options, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await ApplyDatabaseAsync(db, logger, ct);
        await SeedRolesAsync(roleManager);
        await SeedSuperAdminAsync(userManager, options, logger);
        await SeedCommissionAsync(db, options, ct);
        await SeedCategoriesAsync(db, ct);

        if (options.SeedDemoData)
        {
            await SeedDemoDataAsync(db, userManager, options, logger, ct);
            await SeedDemoOrdersAsync(db, userManager, logger, ct);
        }
    }

    private static async Task ApplyDatabaseAsync(ApplicationDbContext db, ILogger logger, CancellationToken ct)
    {
        if (db.Database.GetMigrations().Any())
        {
            await db.Database.MigrateAsync(ct);
        }
        else
        {
            // Convenience for the very first run, before the first migration is generated.
            logger.LogWarning("No EF migrations found. Using EnsureCreated. Generate one with: dotnet ef migrations add InitialCreate");
            await db.Database.EnsureCreatedAsync(ct);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, SeedOptions options, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(options.AdminEmail) || string.IsNullOrWhiteSpace(options.AdminPassword))
        {
            logger.LogWarning("Seed:AdminEmail / Seed:AdminPassword are not configured. The Super Admin account was NOT created.");
            return;
        }

        var admin = await userManager.FindByEmailAsync(options.AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = options.AdminEmail,
                Email = options.AdminEmail,
                EmailConfirmed = true,
                FullName = "Super Admin",
                IsActive = true
            };

            var created = await userManager.CreateAsync(admin, options.AdminPassword);
            if (!created.Succeeded)
            {
                logger.LogError("Could not create the Super Admin: {Errors}",
                    string.Join("; ", created.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, Roles.SuperAdmin))
        {
            await userManager.AddToRoleAsync(admin, Roles.SuperAdmin);
        }
    }

    private static async Task SeedCommissionAsync(ApplicationDbContext db, SeedOptions options, CancellationToken ct)
    {
        if (!await db.CommissionSettings.AnyAsync(c => c.IsActive, ct))
        {
            db.CommissionSettings.Add(new CommissionSetting
            {
                RatePercent = options.DefaultCommissionPercent,
                EffectiveFrom = DateTime.UtcNow,
                IsActive = true
            });
            await db.SaveChangesAsync(ct);
        }
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var wanted = new (string Name, string Slug)[]
        {
            ("Electronics", "electronics"),
            ("Fashion", "fashion"),
            ("Home & Kitchen", "home-kitchen"),
            ("Books", "books"),
            ("Sports & Outdoors", "sports-outdoors")
        };

        var existing = await db.Categories.Select(c => c.Slug).ToListAsync(ct);
        foreach (var (name, slug) in wanted)
        {
            if (!existing.Contains(slug))
            {
                db.Categories.Add(new Category { Name = name, Slug = slug, IsActive = true });
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedDemoDataAsync(
        ApplicationDbContext db, UserManager<ApplicationUser> userManager,
        SeedOptions options, ILogger logger, CancellationToken ct)
    {
        if (await db.VendorProfiles.AnyAsync(v => v.StoreName == "TechNest", ct))
        {
            return; // already seeded
        }

        logger.LogInformation("Seeding demo data (vendors, products, customer).");

        var v1 = await CreateDemoVendorAsync(db, userManager, options, "vendor1@demo.test", "Demo Vendor One",
            "TechNest", "Gadgets and electronics for everyday life.", VendorStatus.Approved, ct);
        var v2 = await CreateDemoVendorAsync(db, userManager, options, "vendor2@demo.test", "Demo Vendor Two",
            "Urban Threads", "Modern clothing and accessories.", VendorStatus.Approved, ct);
        await CreateDemoVendorAsync(db, userManager, options, "pending@demo.test", "Pending Vendor",
            "Fresh Pending Shop", "Waiting for approval.", VendorStatus.Pending, ct);

        var customer = await userManager.FindByEmailAsync("customer@demo.test");
        if (customer is null)
        {
            customer = new ApplicationUser
            {
                UserName = "customer@demo.test",
                Email = "customer@demo.test",
                EmailConfirmed = true,
                FullName = "Demo Customer"
            };
            await userManager.CreateAsync(customer, options.DemoPassword);
            await userManager.AddToRoleAsync(customer, Roles.Customer);
        }

        var cats = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id, ct);

        if (v1 is not null && v2 is not null)
        {
            var products = new List<Product>
            {
                P(v1.Id, cats["electronics"], "Wireless Headphones", "Over-ear Bluetooth headphones with 30h battery.", 59.99m, 40),
                P(v1.Id, cats["electronics"], "Smart Watch", "Fitness tracking and notifications on your wrist.", 89.50m, 25),
                P(v1.Id, cats["electronics"], "USB-C Fast Charger", "65W GaN charger for phone and laptop.", 24.90m, 120),
                P(v1.Id, cats["electronics"], "Mechanical Keyboard", "Hot-swappable switches, RGB backlight.", 74.00m, 15),
                P(v1.Id, cats["home-kitchen"], "Electric Kettle", "1.7L stainless steel kettle.", 32.25m, 60),
                P(v2.Id, cats["fashion"], "Classic Denim Jacket", "Unisex mid-wash denim jacket.", 64.00m, 30),
                P(v2.Id, cats["fashion"], "Cotton T-Shirt (3 pack)", "Soft 100% cotton basics.", 27.50m, 200),
                P(v2.Id, cats["fashion"], "Leather Belt", "Genuine leather, brushed buckle.", 19.99m, 80),
                P(v2.Id, cats["sports-outdoors"], "Running Shoes", "Lightweight daily trainers.", 79.90m, 35),
                P(v2.Id, cats["sports-outdoors"], "Yoga Mat", "6mm non-slip mat.", 21.00m, 90),
                P(v2.Id, cats["books"], "Clean Code (Paperback)", "A handbook of agile software craftsmanship.", 38.00m, 20),
                P(v1.Id, cats["books"], "Pragmatic Programmer", "Your journey to mastery.", 42.00m, 18)
            };

            db.Products.AddRange(products);
            await db.SaveChangesAsync(ct);
        }
    }

    // ------------------------------------------------------------------ demo orders (TASK-04)

    public const string DeliveredOrderNumber = "MH-SEED-DELIVERED";
    public const string PendingOrderNumber = "MH-SEED-PENDING";

    /// <summary>
    /// Two ready-made orders so every teammate can work without waiting for somebody else:
    ///  - a Delivered (Completed) multi-vendor order with commission records -> reviews (B), vendor dashboard (C), admin numbers (E)
    ///  - a PendingPayment order with a Pending payment                       -> webhook tests (E), cancel / cleanup tests (D)
    /// Idempotent: each order is created only when its OrderNumber does not exist yet.
    /// </summary>
    private static async Task SeedDemoOrdersAsync(
        ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct)
    {
        var customer = await userManager.FindByEmailAsync("customer@demo.test");
        var commission = await db.CommissionSettings.FirstOrDefaultAsync(c => c.IsActive, ct);
        if (customer is null || commission is null)
        {
            return;
        }

        var byName = await db.Products
            .Where(p => p.Name == "Wireless Headphones" || p.Name == "Classic Denim Jacket"
                        || p.Name == "Smart Watch" || p.Name == "Cotton T-Shirt (3 pack)")
            .ToDictionaryAsync(p => p.Name, ct);
        if (byName.Count < 4)
        {
            return; // the demo products are missing, nothing to build the orders from
        }

        if (!await db.Orders.AnyAsync(o => o.OrderNumber == DeliveredOrderNumber, ct))
        {
            var paidAt = DateTime.UtcNow.AddDays(-7);
            var order = NewOrder(DeliveredOrderNumber, customer.Id, OrderStatus.Completed, paidAt);

            AddLine(order, byName["Wireless Headphones"], 1, FulfillmentStatus.Delivered);
            AddLine(order, byName["Classic Denim Jacket"], 2, FulfillmentStatus.Delivered);
            order.TotalAmount = order.Items.Sum(i => i.LineTotal);
            order.PaidAt = paidAt;
            order.Payments.Add(new Payment
            {
                StripeCheckoutSessionId = "cs_test_seed_delivered",
                StripePaymentIntentId = "pi_test_seed_delivered",
                Amount = order.TotalAmount,
                Status = PaymentStatus.Succeeded,
                CreatedAt = paidAt,
                CompletedAt = paidAt
            });

            foreach (var item in order.Items)
            {
                var gross = item.LineTotal;
                var fee = Math.Round(gross * commission.RatePercent / 100m, 2, MidpointRounding.AwayFromZero);
                item.ShippedAt = paidAt.AddDays(1);
                item.DeliveredAt = paidAt.AddDays(3);
                item.CommissionRecord = new CommissionRecord
                {
                    VendorProfileId = item.VendorProfileId,
                    CommissionSettingId = commission.Id,
                    GrossAmount = gross,
                    RatePercentSnapshot = commission.RatePercent,
                    CommissionAmount = fee,
                    VendorNetAmount = gross - fee,
                    Status = CommissionStatus.Recorded,
                    CreatedAt = paidAt
                };
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Orders.AnyAsync(o => o.OrderNumber == PendingOrderNumber, ct))
        {
            var order = NewOrder(PendingOrderNumber, customer.Id, OrderStatus.PendingPayment, DateTime.UtcNow);

            AddLine(order, byName["Smart Watch"], 1, FulfillmentStatus.Pending);
            AddLine(order, byName["Cotton T-Shirt (3 pack)"], 2, FulfillmentStatus.Pending);
            order.TotalAmount = order.Items.Sum(i => i.LineTotal);
            order.Payments.Add(new Payment
            {
                StripeCheckoutSessionId = "cs_test_seed_pending",
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending
            });

            db.Orders.Add(order);
            await db.SaveChangesAsync(ct);
        }

        logger.LogInformation("Demo orders are ready ({Delivered}, {Pending}).", DeliveredOrderNumber, PendingOrderNumber);
    }

    private static Order NewOrder(string number, string customerId, OrderStatus status, DateTime createdAt) =>
        new()
        {
            OrderNumber = number,
            CustomerId = customerId,
            Status = status,
            Currency = "USD",
            ShippingFullName = "Demo Customer",
            ShippingPhone = "+201000000000",
            ShippingAddress = "12 Demo Street",
            ShippingCity = "Cairo",
            ShippingCountry = "Egypt",
            CreatedAt = createdAt
        };

    /// <summary>Adds a line with the name/price snapshot, and takes the quantity out of the stock (as a real checkout would).</summary>
    private static void AddLine(Order order, Product product, int quantity, FulfillmentStatus fulfillment)
    {
        product.StockQuantity -= quantity;
        order.Items.Add(new OrderItem
        {
            ProductId = product.Id,
            VendorProfileId = product.VendorProfileId,
            ProductNameSnapshot = product.Name,
            UnitPrice = product.Price,
            Quantity = quantity,
            LineTotal = product.Price * quantity,
            FulfillmentStatus = fulfillment
        });
    }

    private static Product P(int vendorId, int categoryId, string name, string description, decimal price, int stock) =>
        new()
        {
            VendorProfileId = vendorId,
            CategoryId = categoryId,
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stock,
            IsActive = true
        };

    private static async Task<VendorProfile?> CreateDemoVendorAsync(
        ApplicationDbContext db, UserManager<ApplicationUser> userManager, SeedOptions options,
        string email, string fullName, string storeName, string description, VendorStatus status, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName
            };

            var created = await userManager.CreateAsync(user, options.DemoPassword);
            if (!created.Succeeded)
            {
                return null;
            }
        }

        await userManager.AddToRoleAsync(user, status == VendorStatus.Approved ? Roles.Vendor : Roles.Customer);

        var profile = new VendorProfile
        {
            UserId = user.Id,
            StoreName = storeName,
            Description = description,
            Status = status,
            ReviewedAt = status == VendorStatus.Approved ? DateTime.UtcNow : null
        };

        db.VendorProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
        return profile;
    }
}
