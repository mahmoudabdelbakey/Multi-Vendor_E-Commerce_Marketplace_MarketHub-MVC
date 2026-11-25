namespace MarketHub.BLL.Services;

/// <summary>
/// TASK-24. The one place where the commission of a line is calculated, so it can be unit tested without a database.
/// Rounding: 2 decimals, away from zero. commission + net always equals the gross amount.
/// </summary>
public static class CommissionCalculator
{
    public static (decimal Commission, decimal Net) Split(decimal grossAmount, decimal ratePercent)
    {
        if (grossAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(grossAmount), "The amount can not be negative.");
        }

        if (ratePercent < 0 || ratePercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(ratePercent), "The rate must be between 0 and 100.");
        }

        var commission = Math.Round(grossAmount * ratePercent / 100m, 2, MidpointRounding.AwayFromZero);
        return (commission, grossAmount - commission);
    }
}
