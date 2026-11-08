using MarketHub.BLL.Common;
using MarketHub.BLL.Models;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;

namespace MarketHub.BLL.Interfaces;

/// <summary>Public storefront: browse, search, filter, sort, paginate.</summary>
public interface ICatalogService
{
    Task<PagedResult<ProductListItemDto>> SearchAsync(ProductQuery query, CancellationToken ct = default);
    Task<ProductDetailsDto?> GetDetailsAsync(int productId, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductListItemDto>> GetFeaturedAsync(int take = 8, CancellationToken ct = default);
}

/// <summary>Vendor self-service: application, store profile, dashboard.</summary>
public interface IVendorService
{
    Task<Result<int>> ApplyAsync(string userId, VendorApplicationInput input, CancellationToken ct = default);
    Task<VendorDto?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<bool> IsStoreNameAvailableAsync(string storeName, int? exceptVendorId = null, CancellationToken ct = default);

    /// <summary>The vendor id of the user, or null when the user is not an APPROVED vendor.</summary>
    Task<int?> GetApprovedVendorIdAsync(string userId, CancellationToken ct = default);

    Task<Result> UpdateProfileAsync(int vendorId, StoreProfileInput input, CancellationToken ct = default);
    Task<VendorDashboardDto> GetDashboardAsync(int vendorId, CancellationToken ct = default);
}

/// <summary>Super Admin operations on vendors.</summary>
public interface IVendorAdminService
{
    Task<PagedResult<VendorDto>> ListAsync(VendorStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<Result> ApproveAsync(int vendorId, string adminUserId, CancellationToken ct = default);
    Task<Result> RejectAsync(int vendorId, string adminUserId, string reason, CancellationToken ct = default);
    Task<Result> SuspendAsync(int vendorId, string adminUserId, CancellationToken ct = default);
    Task<Result> ReinstateAsync(int vendorId, string adminUserId, CancellationToken ct = default);
}

/// <summary>
/// Vendor-scoped product management. EVERY method takes the vendorId and filters by it:
/// a product that belongs to another vendor behaves exactly like a product that does not exist (404).
/// </summary>
public interface IProductService
{
    Task<PagedResult<ProductManageDto>> ListAsync(int vendorId, string? search, int page, int pageSize, CancellationToken ct = default);
    Task<ProductEditDto?> GetForEditAsync(int vendorId, int productId, CancellationToken ct = default);
    Task<Result<int>> CreateAsync(int vendorId, ProductInput input, IReadOnlyList<UploadedFile> images, CancellationToken ct = default);
    Task<Result> UpdateAsync(int vendorId, int productId, ProductInput input, IReadOnlyList<UploadedFile> newImages, CancellationToken ct = default);
    Task<Result> SetActiveAsync(int vendorId, int productId, bool isActive, CancellationToken ct = default);
    Task<Result> AdjustStockAsync(int vendorId, int productId, int newQuantity, CancellationToken ct = default);
    Task<Result> DeleteImageAsync(int vendorId, int productId, int imageId, CancellationToken ct = default);
    Task<Result> SetPrimaryImageAsync(int vendorId, int productId, int imageId, CancellationToken ct = default);
    Task<Result> DeleteAsync(int vendorId, int productId, CancellationToken ct = default);
}

public interface IFileStorage
{
    /// <summary>Stores the file under a generated GUID name and returns the path relative to wwwroot.</summary>
    Task<string> SaveAsync(Stream content, string extension, CancellationToken ct = default);
    void Delete(string relativePath);
}

public interface ICartService
{
    Task<CartDto> GetCartAsync(string customerId, CancellationToken ct = default);
    Task<int> CountAsync(string customerId, CancellationToken ct = default);
    Task<Result> AddAsync(string customerId, int productId, int quantity, CancellationToken ct = default);
    Task<Result> UpdateQuantityAsync(string customerId, int cartItemId, int quantity, CancellationToken ct = default);
    Task<Result> RemoveAsync(string customerId, int cartItemId, CancellationToken ct = default);
}

public interface ICheckoutService
{
    /// <summary>
    /// Re-validates the cart from the database, creates Order + OrderItems + Payment, reserves stock in ONE transaction,
    /// then creates the Stripe Checkout Session. Returns the Stripe URL to redirect the customer to.
    /// The URLs may contain the token {ORDER_ID} which is replaced with the new order id.
    /// </summary>
    Task<Result<string>> CreateOrderAndSessionAsync(
        string customerId, ShippingInfo shipping, string successUrl, string cancelUrl, CancellationToken ct = default);
}

public interface IOrderService
{
    // Customer
    Task<PagedResult<OrderSummaryDto>> ListForCustomerAsync(string customerId, int page, int pageSize, CancellationToken ct = default);
    Task<OrderDetailsDto?> GetForCustomerAsync(string customerId, int orderId, CancellationToken ct = default);
    Task<Result> CancelPendingAsync(string customerId, int orderId, CancellationToken ct = default);

    // System (webhook, background job)
    Task<bool> CancelAndReleaseStockAsync(int orderId, string reason, CancellationToken ct = default);
    Task<int> ExpireStalePendingOrdersAsync(TimeSpan olderThan, CancellationToken ct = default);

    // Vendor
    Task<PagedResult<VendorOrderItemDto>> ListItemsForVendorAsync(
        int vendorId, FulfillmentStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<Result> UpdateFulfillmentAsync(int vendorId, int orderItemId, FulfillmentStatus newStatus, CancellationToken ct = default);

    // Admin
    Task<PagedResult<AdminOrderDto>> ListAllAsync(OrderStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<OrderDetailsDto?> GetForAdminAsync(int orderId, CancellationToken ct = default);
}

public interface IPaymentService
{
    Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentSessionRequest request, CancellationToken ct = default);

    /// <summary>Expires an open Checkout Session so it can no longer be paid.</summary>
    Task<Result> ExpireSessionAsync(string sessionId, CancellationToken ct = default);
}

public interface IStripeWebhookService
{
    /// <summary>
    /// Verifies the signature against the RAW body, applies the event idempotently inside a transaction.
    /// Throws Stripe.StripeException for a bad signature / payload and WebhookProcessingException for rejected events.
    /// </summary>
    Task HandleAsync(string rawJson, string signatureHeader, CancellationToken ct = default);
}

public interface ICommissionService
{
    Task<CommissionSettingDto?> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CommissionSettingDto>> GetHistoryAsync(CancellationToken ct = default);
    Task<Result<CommissionSettingDto>> SetRateAsync(decimal ratePercent, string adminUserId, CancellationToken ct = default);

    /// <summary>
    /// Adds a CommissionRecord for every item of the order that has none yet. It does NOT call SaveChanges:
    /// the caller owns the transaction. Throws InvalidOperationException when no active commission setting exists.
    /// </summary>
    Task CreateForOrderAsync(Order order, CancellationToken ct = default);
}

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetAsync(int id, CancellationToken ct = default);
    Task<Result<int>> CreateAsync(string name, CancellationToken ct = default);
    Task<Result> UpdateAsync(int id, string name, bool isActive, CancellationToken ct = default);
    Task<int> CountActiveProductsAsync(int categoryId, CancellationToken ct = default);
}

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<PagedResult<AdminProductDto>> ListProductsAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<Result> SetProductActiveAsync(int productId, bool isActive, CancellationToken ct = default);
}

public interface IReviewService
{
    Task<bool> CanReviewAsync(string customerId, int productId, CancellationToken ct = default);
    Task<Result> AddAsync(string customerId, int productId, int rating, string? comment, CancellationToken ct = default);
}
