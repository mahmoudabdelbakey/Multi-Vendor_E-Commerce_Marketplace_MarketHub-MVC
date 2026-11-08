using System.Security.Claims;
using MarketHub.BLL.Interfaces;
using MarketHub.DAL.Data;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.MVC.Infrastructure;

/// <summary>
/// TASK-12. Finds the vendor profile of the logged-in user. The user id comes from the login cookie, the vendor id
/// from the database, so nothing the browser sends can change it. Only an APPROVED vendor has an id.
/// Week 2 note: when VendorService exists this can call IVendorService.GetApprovedVendorIdAsync instead.
/// </summary>
public class CurrentVendorService : ICurrentVendorService
{
    private readonly IHttpContextAccessor _http;
    private readonly ApplicationDbContext _db;
    private int? _cached; // one database lookup per request (the service is scoped)

    public CurrentVendorService(IHttpContextAccessor http, ApplicationDbContext db)
    {
        _http = http;
        _db = db;
    }

    public async Task<int> GetVendorProfileIdAsync(CancellationToken ct = default)
    {
        if (_cached is { } cached)
        {
            return cached;
        }

        var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new InvalidOperationException("There is no authenticated user.");

        var vendorId = await _db.VendorProfiles
            .AsNoTracking()
            .Where(v => v.UserId == userId && v.Status == VendorStatus.Approved)
            .Select(v => (int?)v.Id)
            .FirstOrDefaultAsync(ct);

        _cached = vendorId ?? throw new InvalidOperationException("The current user is not an approved vendor.");
        return _cached.Value;
    }
}
