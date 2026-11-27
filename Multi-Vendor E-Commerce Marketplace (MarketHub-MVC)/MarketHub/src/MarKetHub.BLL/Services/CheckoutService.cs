using System.Security.Cryptography;
using MarketHub.BLL.Common;
using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using MarketHub.DAL.Data;
using MarketHub.DAL.Entities;
using MarKetHub.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketHub.BLL.Services;

/// <summary>
/// TASK-39: Re-validates the customer's cart from the database, reserves stock conditionally,
/// creates the pending order with snapshots and initial payment attempt in a single transaction,
/// and then creates the external payment session outside the database transaction.
/// </summary>
public sealed class CheckoutService : ICheckoutService
{
    private const int MaxQuantityPerLine = 99;
    private const string DefaultCurrency = "USD";

    private readonly ApplicationDbContext _db;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        ApplicationDbContext db,
        IPaymentService paymentService,
        IOrderService orderService,
        ILogger<CheckoutService> logger)
    {
        _db = db;
        _paymentService = paymentService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<Result<string>> CreateOrderAndSessionAsync(
        string customerId, ShippingInfo shipping, string successUrl, string cancelUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Result<string>.Fail("Please sign in to place an order.");
        }

        var shippingError = ValidateShipping(shipping);
        if (shippingError is not null)
        {
            return Result<string>.Fail(shippingError);
        }

        if (string.IsNullOrWhiteSpace(successUrl) || string.IsNullOrWhiteSpace(cancelUrl))
        {
            return Result<string>.Fail("Checkout return URLs are required.");
        }

        var customerEmail = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == customerId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);

        Order order;
        Payment payment;

        await using (var tx = await _db.Database.BeginTransactionAsync(ct))
        {
            // Order by ProductId ascending so concurrent transactions lock products in a consistent order.
            var cartLines = await _db.CartItems
                .AsNoTracking()
                .Where(c => c.CustomerId == customerId)
                .OrderBy(c => c.ProductId)
                .Select(c => new
                {
                    CartItemId = c.Id,
                    c.ProductId,
                    c.Quantity,
                    ProductName = c.Product.Name,
                    c.Product.Price,
                    c.Product.StockQuantity,
                    c.Product.IsActive,
                    c.Product.VendorProfileId,
                    VendorStatus = c.Product.Vendor.Status
                })
                .ToListAsync(ct);

            if (cartLines.Count == 0)
            {
                await tx.RollbackAsync(ct);
                return Result<string>.Fail("Your cart is empty.");
            }

            foreach (var line in cartLines)
            {
                if (line.Quantity < 1 || line.Quantity > MaxQuantityPerLine)
                {
                    await tx.RollbackAsync(ct);
                    return Result<string>.Fail($"Quantity for '{line.ProductName}' must be between 1 and {MaxQuantityPerLine}.");
                }

                if (!line.IsActive)
                {
                    await tx.RollbackAsync(ct);
                    return Result<string>.Fail($"'{line.ProductName}' is no longer available. Please remove it from your cart.");
                }

                if (line.VendorStatus != VendorStatus.Approved)
                {
                    await tx.RollbackAsync(ct);
                    return Result<string>.Fail($"'{line.ProductName}' belongs to a store that is currently unavailable.");
                }

                if (line.Quantity > line.StockQuantity)
                {
                    await tx.RollbackAsync(ct);
                    var msg = line.StockQuantity <= 0
                        ? $"'{line.ProductName}' is out of stock."
                        : $"Only {line.StockQuantity} unit(s) of '{line.ProductName}' are currently in stock.";
                    return Result<string>.Fail(msg);
                }
            }

            var now = DateTime.UtcNow;

            foreach (var line in cartLines)
            {
                var updatedRows = await _db.Products
                    .Where(p => p.Id == line.ProductId
                        && p.IsActive
                        && p.Vendor.Status == VendorStatus.Approved
                        && p.StockQuantity >= line.Quantity)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.StockQuantity, p => p.StockQuantity - line.Quantity)
                        .SetProperty(p => p.UpdatedAt, _ => now), ct);

                if (updatedRows != 1)
                {
                    await tx.RollbackAsync(ct);
                    return Result<string>.Fail(
                        $"'{line.ProductName}' could not be reserved because stock or availability just changed. Please review your cart.");
                }
            }

            // Deleting the cart lines inside the transaction prevents concurrent checkouts from ordering the same cart twice.
            var cartItemIds = cartLines.Select(c => c.CartItemId).ToList();
            var deletedCartRows = await _db.CartItems
                .Where(c => c.CustomerId == customerId && cartItemIds.Contains(c.Id))
                .ExecuteDeleteAsync(ct);

            if (deletedCartRows != cartLines.Count)
            {
                await tx.RollbackAsync(ct);
                return Result<string>.Fail("Your cart changed while placing the order. Please review your cart and try again.");
            }

            var orderItems = cartLines.Select(line => new OrderItem
            {
                ProductId = line.ProductId,
                VendorProfileId = line.VendorProfileId,
                ProductNameSnapshot = line.ProductName,
                UnitPrice = line.Price,
                Quantity = line.Quantity,
                LineTotal = line.Price * line.Quantity,
                FulfillmentStatus = FulfillmentStatus.Pending
            }).ToList();

            var totalAmount = orderItems.Sum(i => i.LineTotal);
            if (totalAmount <= 0)
            {
                await tx.RollbackAsync(ct);
                return Result<string>.Fail("Order total must be greater than zero.");
            }

            payment = new Payment
            {
                Provider = ResolveProviderName(_paymentService),
                Amount = totalAmount,
                Currency = DefaultCurrency,
                Status = PaymentStatus.Pending,
                CreatedAt = now
            };

            order = new Order
            {
                OrderNumber = GenerateOrderNumber(now),
                CustomerId = customerId,
                Status = OrderStatus.PendingPayment,
                TotalAmount = totalAmount,
                Currency = DefaultCurrency,
                ShippingFullName = shipping.FullName.Trim(),
                ShippingPhone = shipping.Phone.Trim(),
                ShippingAddress = shipping.Address.Trim(),
                ShippingCity = shipping.City.Trim(),
                ShippingCountry = shipping.Country.Trim(),
                CreatedAt = now,
                Items = orderItems,
                Payments = new List<Payment> { payment }
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        // Payment session creation runs outside the SQL transaction so external latency never holds database locks.
        var orderIdText = order.Id.ToString();
        var resolvedSuccessUrl = successUrl.Replace("{ORDER_ID}", orderIdText, StringComparison.Ordinal);
        var resolvedCancelUrl = cancelUrl.Replace("{ORDER_ID}", orderIdText, StringComparison.Ordinal);

        var sessionRequest = new PaymentSessionRequest(
            order.Id,
            order.OrderNumber,
            order.Currency,
            order.Items.Select(i => new PaymentLine(i.ProductNameSnapshot, i.UnitPrice, i.Quantity)).ToList(),
            resolvedSuccessUrl,
            resolvedCancelUrl,
            customerEmail);

        PaymentSessionResult session;
        try
        {
            session = await _paymentService.CreateCheckoutSessionAsync(sessionRequest, ct);
            if (string.IsNullOrWhiteSpace(session.SessionId) || string.IsNullOrWhiteSpace(session.Url))
            {
                throw new InvalidOperationException("Payment provider returned an empty session.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create payment session for order {OrderId} ({OrderNumber}).", order.Id, order.OrderNumber);
            await MarkPaymentFailedAndReleaseOrderAsync(order.Id, payment.Id, "Payment session creation failed.");
            return Result<string>.Fail("We could not start the payment session right now. Your order was cancelled and no stock was held. Please try again.");
        }

        try
        {
            payment.StripeCheckoutSessionId = session.SessionId;
            await _db.SaveChangesAsync(ct);
            return Result<string>.Ok(session.Url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Created payment session {SessionId} for order {OrderId}, but failed to persist the session id.", session.SessionId, order.Id);
            await TryExpireSessionAsync(session.SessionId);
            await MarkPaymentFailedAndReleaseOrderAsync(order.Id, payment.Id, "Failed to save payment session.");
            return Result<string>.Fail("We could not finalize your payment session. Your order was cancelled. Please try again.");
        }
    }

    private async Task MarkPaymentFailedAndReleaseOrderAsync(int orderId, int paymentId, string reason)
    {
        try
        {
            var now = DateTime.UtcNow;
            var message = reason.Length > 500 ? reason[..500] : reason;

            _db.ChangeTracker.Clear();
            await _db.Payments
                .Where(p => p.Id == paymentId && p.Status == PaymentStatus.Pending)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, PaymentStatus.Failed)
                    .SetProperty(p => p.FailureMessage, _ => message)
                    .SetProperty(p => p.CompletedAt, _ => now), CancellationToken.None);

            await _orderService.CancelAndReleaseStockAsync(orderId, reason, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to roll back order {OrderId} after payment session failure. Cleanup job will expire it.", orderId);
        }
    }

    private async Task TryExpireSessionAsync(string sessionId)
    {
        try
        {
            await _paymentService.ExpireSessionAsync(sessionId, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to expire orphaned payment session {SessionId}.", sessionId);
        }
    }

    private static string ResolveProviderName(IPaymentService paymentService) =>
        paymentService.GetType().Name.Contains("Fake", StringComparison.OrdinalIgnoreCase)
            ? "Fake"
            : "Stripe";

    private static string GenerateOrderNumber(DateTime utcNow)
    {
        var suffix = RandomNumberGenerator.GetInt32(100000, 1000000);
        var token = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
        return $"MH-{utcNow:yyyyMMdd}-{suffix}{token}";
    }

    private static string? ValidateShipping(ShippingInfo? shipping)
    {
        if (shipping is null)
        {
            return "Shipping information is required.";
        }

        if (string.IsNullOrWhiteSpace(shipping.FullName) || shipping.FullName.Trim().Length > 100)
        {
            return "Enter a valid recipient full name (up to 100 characters).";
        }

        if (string.IsNullOrWhiteSpace(shipping.Phone) || shipping.Phone.Trim().Length > 30)
        {
            return "Enter a valid phone number (up to 30 characters).";
        }

        if (string.IsNullOrWhiteSpace(shipping.Address) || shipping.Address.Trim().Length > 250)
        {
            return "Enter a valid street address (up to 250 characters).";
        }

        if (string.IsNullOrWhiteSpace(shipping.City) || shipping.City.Trim().Length > 80)
        {
            return "Enter a valid city (up to 80 characters).";
        }

        if (string.IsNullOrWhiteSpace(shipping.Country) || shipping.Country.Trim().Length > 80)
        {
            return "Enter a valid country (up to 80 characters).";
        }

        return null;
    }
}
