namespace MarketHub.BLL.Interfaces;

/// <summary>
/// Tells the code WHICH vendor is logged in. The id always comes from the authenticated user (through the database),
/// never from a query string or a form field - that is what stops IDOR / over-posting.
/// Contract owner: Member A. Used by: Member C (vendor products), Member E (vendor orders).
/// </summary>
public interface ICurrentVendorService
{
    /// <summary>The VendorProfile id of the logged-in APPROVED vendor. Throws when the user is not an approved vendor.</summary>
    Task<int> GetVendorProfileIdAsync(CancellationToken ct = default);
}
