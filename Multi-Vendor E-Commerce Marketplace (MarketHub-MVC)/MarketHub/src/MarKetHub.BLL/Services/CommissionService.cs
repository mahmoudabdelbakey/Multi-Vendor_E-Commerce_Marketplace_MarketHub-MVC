using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

public class CommissionService : ICommissionService
{
    private readonly ApplicationDbContext _db;

    public CommissionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CommissionSettingDto?> GetActiveAsync(CancellationToken ct = default)
    {
        var setting = await _db.CommissionSettings
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(ct);

        if (setting == null)
            return null;

        return new CommissionSettingDto(setting.Id, setting.RatePercent, setting.EffectiveFrom, setting.IsActive);
    }

    public async Task<IReadOnlyList<CommissionSettingDto>> GetHistoryAsync(CancellationToken ct = default)
    {
        var settings = await _db.CommissionSettings
            .OrderByDescending(c => c.EffectiveFrom)
            .ToListAsync(ct);

        return settings.Select(s => new CommissionSettingDto(s.Id, s.RatePercent, s.EffectiveFrom, s.IsActive)).ToList();
    }

    public async Task<Result<CommissionSettingDto>> SetRateAsync(decimal ratePercent, string adminUserId, CancellationToken ct = default)
    {
        if (ratePercent < 0 || ratePercent > 100)
            return Result<CommissionSettingDto>.Fail("Rate must be between 0 and 100.");

        var currentActive = await _db.CommissionSettings
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(ct);

        if (currentActive != null)
            currentActive.IsActive = false;

        var newSetting = new CommissionSetting
        {
            RatePercent = ratePercent,
            EffectiveFrom = DateTime.UtcNow,
            CreatedByUserId = adminUserId,
            IsActive = true
        };

        _db.CommissionSettings.Add(newSetting);
        await _db.SaveChangesAsync(ct);

        return Result<CommissionSettingDto>.Ok(new CommissionSettingDto(newSetting.Id, newSetting.RatePercent, newSetting.EffectiveFrom, newSetting.IsActive));
    }

    public async Task CreateForOrderAsync(Order order, CancellationToken ct = default)
    {
        var activeSetting = await _db.CommissionSettings
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(ct);

        if (activeSetting == null)
            throw new InvalidOperationException("No active commission setting exists.");

        foreach (var item in order.Items)
        {
            var existingRecord = await _db.CommissionRecords
                .AnyAsync(r => r.OrderItemId == item.Id, ct);

            if (existingRecord)
                continue;

            var gross = item.UnitPrice * item.Quantity;
            var commission = Math.Round(gross * activeSetting.RatePercent / 100m, 2, MidpointRounding.AwayFromZero);
            var net = gross - commission;

            var record = new CommissionRecord
            {
                OrderItemId = item.Id,
                VendorProfileId = item.VendorProfileId,
                CommissionSettingId = activeSetting.Id,
                GrossAmount = gross,
                RatePercentSnapshot = activeSetting.RatePercent,
                CommissionAmount = commission,
                VendorNetAmount = net
            };

            _db.CommissionRecords.Add(record);
        }
    }
}
