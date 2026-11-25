# Database Diagram

## Entity Relationships

```
┌─────────────────┐       ┌─────────────────┐
│ ApplicationUser │───────│  VendorProfile  │
└─────────────────┘  1:1  └────────┬────────┘
                                   │1
                                   │
                                   │n
┌─────────────────┐       ┌────────▼────────┐
│    Category     │◄──────│    Product      │
└─────────────────┘  1:n  └────────┬────────┘
                                   │1
                                   │
                                   │n
                          ┌────────▼────────┐
                          │  ProductImage   │
                          └─────────────────┘

┌─────────────────┐       ┌─────────────────┐
│    Customer     │───────│     Order       │
└─────────────────┘  1:n  └────────┬────────┘
                                   │1
                                   │
                                   │n
                          ┌────────▼────────┐       ┌─────────────────┐
                          │   OrderItem     │───────│    Product      │
                          └────────┬────────┘  n:1  └─────────────────┘
                                   │
                                   │1
                                   │
                          ┌────────▼────────┐
                          │CommissionRecord │
                          └────────┬────────┘
                                   │n
                                   │
                                   │1
                          ┌────────▼────────┐
                          │CommissionSetting│
                          └─────────────────┘

┌─────────────────┐       ┌─────────────────┐
│     Order       │───────│    Payment      │
└─────────────────┘  1:1  └─────────────────┘

┌─────────────────┐
│StripeWebhookEvent│
└─────────────────┘
```

## Key Entities

### VendorProfile
- UserId (FK to ApplicationUser)
- StoreName
- Status (Pending, Approved, Rejected, Suspended)

### Product
- VendorProfileId (FK)
- CategoryId (FK)
- Name, Price, StockQuantity
- IsActive

### Order
- CustomerId (FK)
- Status (PendingPayment, Paid, Completed, Cancelled)
- TotalAmount, Currency

### OrderItem
- OrderId (FK)
- ProductId (FK)
- VendorProfileId (FK)
- FulfillmentStatus (Pending, Shipped, Delivered, Cancelled)

### CommissionSetting
- RatePercent
- EffectiveFrom
- IsActive

### CommissionRecord
- OrderItemId (FK)
- VendorProfileId (FK)
- CommissionSettingId (FK)
- GrossAmount, CommissionAmount, VendorNetAmount
