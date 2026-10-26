namespace MarketHub.DAL.Models
{
    public class CommissionRecord
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }

        public int VendorProfileId { get; set; }

        public int CommissionSettingId { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal RatePercentSnapshot { get; set; }

        public decimal CommissionAmount { get; set; }

        public decimal VendorNetAmount { get; set; }

        public CommissionRecordStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderItem OrderItem { get; set; } = null!;

        public VendorProfile Vendor { get; set; } = null!;

        public CommissionSetting CommissionSetting { get; set; }
            = null!;
    }
}
