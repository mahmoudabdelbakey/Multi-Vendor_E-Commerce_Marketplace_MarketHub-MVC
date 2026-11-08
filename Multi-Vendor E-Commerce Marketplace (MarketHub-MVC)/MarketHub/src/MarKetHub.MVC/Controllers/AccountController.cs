using MarketHub.DAL.Constants;
using MarketHub.DAL.Entities;
using MarketHub.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers;

/// <summary>TASK-08: register, log in, log out, access denied. (The vendor application is added in week 2.)</summary>
public class AccountController : AppControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, ILogger<AccountController> logger)
    {
        _users = users;
        _signIn = signIn;
        _logger = logger;
    }

    // ------------------------------------------------------------------ register

    [AllowAnonymous, HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            IsActive = true
        };

        var created = await _users.CreateAsync(user, model.Password);
        if (!created.Succeeded)
        {
            // Identity messages: "password too weak", "email already taken", ...
            foreach (var error in created.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Everybody starts as a Customer. The Vendor role is added ONLY when a Super Admin approves an application (week 2).
        await _users.AddToRoleAsync(user, Roles.Customer);
        _logger.LogInformation("New customer registered: {UserId}", user.Id);

        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    // ------------------------------------------------------------------ login / logout

    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _users.FindByEmailAsync(model.Email.Trim());

        // ONE generic message for "unknown user", "deactivated" and "wrong password": nothing is revealed to an attacker.
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var result = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {UserId} is locked out after too many failed attempts.", user.Id);
            ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in a few minutes.");
            return View(model);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // Open-redirect protection: only LOCAL urls are accepted ("https://evil.com" is ignored).
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        if (await _users.IsInRoleAsync(user, Roles.SuperAdmin))
        {
            return RedirectToAction("Index", "Commission", new { area = "Admin" });
        }

        if (await _users.IsInRoleAsync(user, Roles.Vendor))
        {
            return RedirectToAction("Index", "Products", new { area = "Vendor" });
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>POST only (a GET link could be triggered by any other site). The antiforgery filter is global.</summary>
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
