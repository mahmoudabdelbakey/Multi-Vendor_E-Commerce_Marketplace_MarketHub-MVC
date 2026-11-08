using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Constants;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarketHub.MVC.Authorization;
using MarketHub.MVC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MarketHub.MVC.Infrastructure.Features;

/// <summary>OWNER: Member A. Identity, roles, cookies, authorization policies. Nobody else edits this file.</summary>
public static class IdentityFeature
{
    public static IServiceCollection AddIdentityFeature(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddHttpContextAccessor();

        // ------------------------------------------------------------ TASK-07: Identity rules
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false; // keeps the sign-up hint simple: upper, lower, digit

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;  // MVP: there is no email provider yet

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;     // 5 wrong passwords ...
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // ... lock the account for 10 minutes
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.Name = "MarketHub.Auth";
            options.Cookie.HttpOnly = true;                     // JavaScript cannot read the cookie
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;                    // HTTPS only outside Development
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
        });

        // A role change (e.g. vendor approval in week 2) reaches the user within a minute, without a manual re-login.
        services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(1));

        // ------------------------------------------------------------ TASK-10 / TASK-11: authorization
        services.AddAuthorizationBuilder()
            // "Closed by default": every endpoint needs a login unless it says [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            // PRELIMINARY (week 1): role check only, so C and E can protect their areas from day one.
            // Week 2 (TASK-30) replaces it with a requirement that asks the database "is the vendor Approved right now?".
            .AddPolicy(PolicyNames.ApprovedVendor, p => p.RequireAuthenticatedUser().RequireRole(Roles.Vendor))
            .AddPolicy(PolicyNames.SuperAdminOnly, p => p.RequireAuthenticatedUser().RequireRole(Roles.SuperAdmin));

        // ------------------------------------------------------------ TASK-12
        services.AddScoped<ICurrentVendorService, CurrentVendorService>();

        return services;
    }
}
