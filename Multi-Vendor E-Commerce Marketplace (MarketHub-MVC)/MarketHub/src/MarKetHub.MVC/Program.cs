using MarketHub.DAL;
using MarketHub.DAL.Data;
using MarketHub.MVC.Infrastructure.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------- configuration
// Secrets (connection string, admin password, Stripe keys) come from user-secrets / environment variables, never from code.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection("Seed"));

// ---------------------------------------------------------------- layers
builder.Services.AddDataAccess(connectionString);   // DAL: DbContext

// ---------------------------------------------------------------- features (TASK-05)
// ONE line per owner, ONE file per owner (Infrastructure/Features). Nobody edits somebody else's file, so two people
// never touch the same line of Program.cs.
builder.Services.AddIdentityFeature(builder.Configuration, builder.Environment);  // A: identity, roles, policies
builder.Services.AddCatalogFeature();                                             // B: catalog, reviews
builder.Services.AddVendorProductsFeature(builder.Configuration);                 // C: vendor products, files
builder.Services.AddOrdersFeature(builder.Configuration);                         // D: cart, checkout, orders, Stripe session
builder.Services.AddAdminPaymentsFeature(builder.Configuration);                  // E: commission, webhook, admin

// ---------------------------------------------------------------- MVC
builder.Services.AddControllersWithViews(options =>
{
    // CSRF protection on every unsafe request (POST/PUT/DELETE). The Stripe webhook opts out with
    // [IgnoreAntiforgeryToken] and is protected by its signature instead.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

var app = builder.Build();

// ---------------------------------------------------------------- pipeline (the ORDER matters)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // never show stack traces outside Development
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");

// One culture for number/date parsing so "49.99" binds the same on every machine.
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US"));

app.UseRouting();
app.UseAuthentication();   // who are you?   (must come BEFORE authorization)
app.UseAuthorization();    // what may you do?

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ---------------------------------------------------------------- database + seed
var seedOptions = app.Services.GetRequiredService<IOptions<SeedOptions>>().Value;
await DbSeeder.SeedAsync(app.Services, seedOptions);

app.Run();
