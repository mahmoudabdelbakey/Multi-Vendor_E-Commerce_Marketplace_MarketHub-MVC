# Order Lifecycle

## Order Statuses

```
┌──────────────────┐
│  PendingPayment  │  Customer created order, awaiting payment
└────────┬─────────┘
         │
    ┌────┴────┐
    │         │
    ▼         ▼
┌───────┐  ┌───────────┐
│ Paid  │  │ Cancelled │  Payment failed or expired
└───┬───┘  └───────────┘
    │
    ▼
┌───────────┐
│ Completed │  All items delivered
└───────────┘
```

## Fulfillment Statuses (Per Item)

```
┌─────────┐
│ Pending │  Order paid, awaiting shipment
└────┬────┘
     │
     ▼
┌─────────┐
│ Shipped │  Vendor shipped the item
└────┬────┘
     │
     ▼
┌───────────┐
│ Delivered │  Item delivered to customer
└───────────┘
```

## Order Flow

1. **Customer adds items to cart**
2. **Customer initiates checkout**
   - Order created with status `PendingPayment`
   - Stock reserved
   - Stripe Checkout Session created
3. **Customer pays on Stripe**
4. **Webhook: checkout.session.completed**
   - Order status → `Paid`
   - Commission records created for each item
5. **Vendor ships item**
   - Item fulfillment status → `Shipped`
6. **Vendor marks delivered**
   - Item fulfillment status → `Delivered`
7. **All items delivered**
   - Order status → `Completed`

## Cancellation Flow

- **Webhook: checkout.session.expired**
  - Order status → `Cancelled`
  - Stock returned to inventory

- **Webhook: payment_intent.payment_failed**
  - Order status → `Cancelled`
  - Stock returned to inventory
