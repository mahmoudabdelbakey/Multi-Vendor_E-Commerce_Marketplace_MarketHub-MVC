using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Constants;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Services;

/// <summary>
/// TASK-29. What the Super Admin can do with a vendor application. Allowed moves only:
/// Pending -> Approved | Rejected, Approved -> Suspended, Suspended -> Approved (restore).
/// Anything else is refused with a clear message, so a double click or an old page can not break the state.
/// </summary>
public class VendorAdminService : IVendorAdminService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public VendorAdminService(ApplicationDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<PagedResult<VendorDto>> ListAsync(
        VendorStatus? status, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.VendorProfiles.AsNoTracking().AsQueryable();
        if (status is { } s)
        {
            query = query.Where(v => v.Status == s);
        }

        var total = await query.CountAsync(ct);

        // Newest first. The Id tie-breaker keeps the pages stable when two applications have the same time.
        var items = await query
            .OrderByDescending(v => v.CreatedAt).ThenByDescending(v => v.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(v => new VendorDto(
                v.Id, v.UserId, v.StoreName, v.Description, v.LogoPath,
                v.Status, v.RejectionReason, v.CreatedAt, v.ReviewedAt,
                v.User.FullName, v.User.Email ?? string.Empty))
            .ToListAsync(ct);

        return new PagedResult<VendorDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<Result> ApproveAsync(int vendorId, string adminUserId, CancellationToken ct = default)
    {
        var vendor = await _db.VendorProfiles.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == vendorId, ct);
        if (vendor is null)
        {
            return Result.NotFoundResult("Application not found.");
        }

        if (vendor.Status != VendorStatus.Pending)
        {
            return Result.Fail("This application has already been reviewed.");
        }

        // The profile status and the Identity role are saved together or not at all.
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        vendor.Status = VendorStatus.Approved;
        vendor.RejectionReason = null;
        vendor.ReviewedByUserId = adminUserId;
        vendor.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        if (!await _users.IsInRoleAsync(vendor.User, Roles.Vendor))
        {
            var added = await _users.AddToRoleAsync(vendor.User, Roles.Vendor);
            if (!added.Succeeded)
            {
                // Leaving the method without Commit rolls the transaction back.
                return Result.Fail("Could not give the Vendor role: " + string.Join("; ", added.Errors.Select(e => e.Description)));
            }
        }

        // A new security stamp makes the old cookie invalid on its next check (every minute, TASK-07),
        // so the user gets the new role without logging in again.
        await _users.UpdateSecurityStampAsync(vendor.User);

        await tx.CommitAsync(ct);
        return Result.Ok($"\"{vendor.StoreName}\" was approved.");
    }

    public async Task<Result> RejectAsync(int vendorId, string adminUserId, string reason, CancellationToken ct = default)
    {
        reason = (reason ?? string.Empty).Trim();
        if (reason.Length == 0)
        {
            return Result.Fail("A rejection reason is required.");
        }

        if (reason.Length > 500)
        {
            return Result.Fail("The reason cannot be longer than 500 characters.");
        }

        var vendor = await _db.VendorProfiles.FirstOrDefaultAsync(v => v.Id == vendorId, ct);
        if (vendor is null)
        {
            return Result.NotFoundResult("Application not found.");
        }

        if (vendor.Status != VendorStatus.Pending)
        {
            return Result.Fail("This application has already been reviewed.");
        }

        vendor.Status = VendorStatus.Rejected;
        vendor.RejectionReason = reason;
        vendor.ReviewedByUserId = adminUserId;
        vendor.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result.Ok($"\"{vendor.StoreName}\" was rejected.");
    }

    public Task<Result> SuspendAsync(int vendorId, string adminUserId, CancellationToken ct = default) =>
        ChangeStatusAsync(vendorId, adminUserId, VendorStatus.Approved, VendorStatus.Suspended,
            "Only an approved vendor can be suspended.",
            "The vendor was suspended. The products are hidden and the vendor area is closed.", ct);

    public Task<Result> ReinstateAsync(int vendorId, string adminUserId, CancellationToken ct = default) =>
        ChangeStatusAsync(vendorId, adminUserId, VendorStatus.Suspended, VendorStatus.Approved,
            "Only a suspended vendor can be restored.", "The vendor was restored.", ct);

    private async Task<Result> ChangeStatusAsync(
        int vendorId, string adminUserId, VendorStatus from, VendorStatus to,
        string wrongStateMessage, string successMessage, CancellationToken ct)
    {
        var vendor = await _db.VendorProfiles.FirstOrDefaultAsync(v => v.Id == vendorId, ct);
        if (vendor is null)
        {
            return Result.NotFoundResult("Vendor not found.");
        }

        if (vendor.Status != from)
        {
            return Result.Fail(wrongStateMessage);
        }

        vendor.Status = to;
        vendor.ReviewedByUserId = adminUserId;
        vendor.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        // The role stays. What the vendor can do is decided by the Status in the database:
        // the catalog and the cart filter by it, and the ApprovedVendor policy (TASK-30) checks it on every request.
        return Result.Ok(successMessage);
    }
}
