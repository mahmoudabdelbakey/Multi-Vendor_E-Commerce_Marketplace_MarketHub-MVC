namespace MarketHub.DAL.Models
{
    public class CommissionSetting
    {
        public int Id { get; set; }

        public decimal RatePercent { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public int CreatedByUserId { get; set; }

        public bool IsActive { get; set; }

        public ApplicationUser CreatedByUser { get; set; }
            = null!;

        public ICollection<CommissionRecord> CommissionRecords { get; set; }
            = new List<CommissionRecord>();
    }
}
