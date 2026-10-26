namespace MarketHub.DAL.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool IsActive { get; set; } = true;

        public int AccessFailedCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public VendorProfile? VendorProfile { get; set; }
    }

}