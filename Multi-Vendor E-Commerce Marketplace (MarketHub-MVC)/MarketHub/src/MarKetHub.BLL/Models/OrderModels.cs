using MarKetHub.DAL.Enums;

namespace MarketHub.BLL.Models;

public class ShippingInfo
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public record OrderSummaryDto(
    int Id, string OrderNumber, OrderStatus Status, decimal TotalAmount, string Currency,
    DateTime CreatedAt, DateTime? PaidAt, int ItemCount);

public record OrderItemDto(
    int Id, int ProductId, string ProductName, int VendorId, string StoreName,
    decimal UnitPrice, int Quantity, decimal LineTotal,
    FulfillmentStatus FulfillmentStatus, DateTime? ShippedAt, DateTime? DeliveredAt);

public record OrderDetailsDto(
    int Id, string OrderNumber, OrderStatus Status, decimal TotalAmount, string Currency,
    DateTime CreatedAt, DateTime? PaidAt, ShippingInfo Shipping,
    string CustomerName, string CustomerEmail,
    IReadOnlyList<OrderItemDto> Items);

/// <summary>What a vendor sees: only his own items, never the rest of the order.</summary>
public record VendorOrderItemDto(
    int ItemId, int OrderId, string OrderNumber, DateTime OrderDate,
    int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal,
    FulfillmentStatus FulfillmentStatus,
    string ShipToName, string ShipToPhone, string ShipToAddress, string ShipToCity, string ShipToCountry,
    decimal? CommissionAmount, decimal? VendorNetAmount);

public record AdminOrderDto(
    int Id, string OrderNumber, string CustomerEmail, OrderStatus Status,
    decimal TotalAmount, string Currency, DateTime CreatedAt, int ItemCount, int VendorCount);

public record PaymentLine(string Name, decimal UnitPrice, int Quantity);

public record PaymentSessionRequest(
    int OrderId, string OrderNumber, string Currency,
    IReadOnlyList<PaymentLine> Lines, string SuccessUrl, string CancelUrl, string? CustomerEmail);

public record PaymentSessionResult(string SessionId, string Url);
