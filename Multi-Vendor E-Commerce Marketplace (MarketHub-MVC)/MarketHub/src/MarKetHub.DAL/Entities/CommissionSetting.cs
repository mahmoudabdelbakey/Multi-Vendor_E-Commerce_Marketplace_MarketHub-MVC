namespace MarketHub.DAL.Entities;

/// <summary>Append-only history of commission rates. Exactly one row has IsActive = true.</summary>
public class CommissionSetting
{
    public int Id { get; set; }
    public decimal RatePercent { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public string? CreatedByUserId { get; set; }
    public bool IsActive { get; set; }
}
