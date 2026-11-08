using System.ComponentModel.DataAnnotations;

namespace MarketHub.MVC.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your email."), EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your password."), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    /// <summary>Where to go after login. Only LOCAL urls are followed (open-redirect protection).</summary>
    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Registration always creates a Customer. There is no role field on purpose: a user must never be able to choose a role
/// by posting an extra form value (over-posting). The vendor application is added in week 2.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your full name."), StringLength(100, MinimumLength = 2), Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email."), EmailAddress(ErrorMessage = "Please enter a valid email address."), StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please choose a password."), StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters."),
     DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please repeat the password."), DataType(DataType.Password),
     Compare(nameof(Password), ErrorMessage = "The two passwords do not match."), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ErrorViewModel
{
    public int? StatusCode { get; init; }
    public string Title { get; init; } = "Something went wrong";
    public string Message { get; init; } = "An unexpected error occurred. Please try again.";
    public string? RequestId { get; init; }
}
