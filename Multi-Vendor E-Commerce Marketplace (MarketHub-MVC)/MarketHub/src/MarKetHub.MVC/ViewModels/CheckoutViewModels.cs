using System.ComponentModel.DataAnnotations;
using MarketHub.BLL.Models;

namespace MarketHub.MVC.ViewModels;

public sealed class CheckoutViewModel
{
    [Required(ErrorMessage = "Recipient full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(30, ErrorMessage = "Phone number cannot exceed 30 characters.")]
    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required.")]
    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    [Display(Name = "Street address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(80, ErrorMessage = "City cannot exceed 80 characters.")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(80, ErrorMessage = "Country cannot exceed 80 characters.")]
    [Display(Name = "Country")]
    public string Country { get; set; } = string.Empty;

    public CartDto Cart { get; set; } = new();
}
