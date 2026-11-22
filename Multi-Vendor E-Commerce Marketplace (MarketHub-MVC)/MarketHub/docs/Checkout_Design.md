# TASK-23: Checkout Transaction Design

**Owner:** Member D  
**Status:** Design input for checkout implementation  
**Dependencies:** None. TASK-39 and later checkout tasks use this design.

## 1. Goal

Create one pending order from the signed-in customer’s current cart, preserve the purchase-time product and shipping details, and reserve stock without allowing two customers to buy the same last unit. Keep the database transaction short and recover safely if payment-session creation fails or the application stops partway through checkout.

This document describes the intended flow. It does not implement the checkout service, payment provider, webhook, or user interface.

## 2. Existing project contracts and data

The design follows the current project types:

- ICheckoutService.CreateOrderAndSessionAsync receives a customer ID, ShippingInfo, success URL, cancel URL, and cancellation token. Its contract says the URLs may contain the token {ORDER_ID}.
- PaymentSessionRequest carries the order ID and number, currency, line items, success and cancel URLs, and optional customer email.
- PaymentSessionResult returns the provider session ID and redirect URL.
- Order stores the customer, order number, status, total, currency, shipping snapshot, creation time, and paid time.
- OrderItem stores a product and vendor reference plus product name, unit price, quantity, and line-total snapshots.
- Payment stores the provider, amount, currency, payment status, provider session and intent IDs, failure message, and timestamps.
- Product has a row-version concurrency token. CartItem has one unique row per customer and product.
- OrderNumber and non-null Stripe checkout-session IDs have unique indexes. Order item quantity and line total also have database constraints.

The relevant code is in [IServices.cs](../src/MarKetHub.BLL/Interfaces/IServices.cs), [OrderModels.cs](../src/MarKetHub.BLL/Models/OrderModels.cs), [Order.cs](../src/MarKetHub.DAL/Entities/Order.cs), [OrderItem.cs](../src/MarKetHub.DAL/Entities/OrderItem.cs), and [Payment.cs](../src/MarKetHub.DAL/Entities/Payment.cs).

## 3. Business and security invariants

1. The customer ID comes from the authenticated NameIdentifier claim. It is never accepted from a form.
2. The cart is re-read from the database during checkout. Product IDs and quantities identify the cart lines, but submitted prices, totals, stock, vendor status, currency, and order status are never trusted.
3. Current product prices are read from Products. OrderItem snapshots preserve the purchased name, vendor, unit price, quantity, and line total even if the product changes later.
4. The vendor must still be approved, the product must be active, the requested quantity must be valid, and stock must be sufficient at reservation time.
5. Stock cannot become negative. A reservation succeeds only when a conditional database update changes exactly one product row.
6. Every line reservation, order header, order items, pending payment row, and cart removal is part of the same database transaction. Any failure before commit rolls back all of them.
7. The transaction is committed before calling a payment provider. Network latency must not hold database locks open.
8. A browser return to a success URL is not proof of payment. The verified payment webhook is the authority for marking an order paid.
9. Releasing stock is idempotent. Only a transition from PendingPayment to Cancelled can release stock; repeating cancellation or cleanup must not add stock a second time.
10. No card number, CVC, or other card data is stored. Only provider identifiers and payment state are persisted.

## 4. Proposed happy path

### 4.1 Request and authoritative revalidation

1. The MVC checkout POST is restricted to Customers, validates shipping fields, obtains the customer ID from the login claim, and creates success and cancel URLs on the server.
2. CheckoutService loads that customer’s cart lines and joins each line to its current Product and Vendor.
3. It rejects an empty cart, inactive products, unapproved vendors, invalid quantities, or lines whose quantity exceeds current stock.
4. It calculates each line total from the database price and quantity, then sums the order total. The current schema has no shipping-fee or tax fields, so TotalAmount is the merchandise subtotal.
5. It obtains customer email from the server-side identity record, when available.

### 4.2 Database transaction: reserve and create

Start an explicit SQL transaction before reading the cart. Serialize a customer’s checkout of the same cart so concurrent submissions cannot create two orders from the same cart. Use the existing customer/product cart index where possible. Process product reservations in ascending ProductId order to reduce deadlock risk.

Within the transaction:

1. Re-read and validate the customer’s current cart and product/vendor state.
2. For each line, issue a conditional stock decrement equivalent to: subtract the requested quantity only when the product is active, its vendor is approved, and StockQuantity is at least the requested quantity. Use the row version or another atomic condition to detect a concurrent product update. Require one affected row per product.
3. If any line fails reservation, roll back the transaction and return a useful stock or availability error. Do not leave partial reservations or an order.
4. Create an Order with status PendingPayment, a server-generated unique OrderNumber, the server-calculated TotalAmount and Currency, and shipping address snapshots from the validated ShippingInfo.
5. Create an OrderItem for every cart line, including ProductId, VendorProfileId, product-name snapshot, current unit price, quantity, and line total.
6. Create one Payment attempt with the same amount and currency and status Pending. Set Provider to the selected fake or real provider.
7. Delete the checked-out cart lines for this customer in the same transaction. If the transaction rolls back, the cart remains available.
8. Save and commit.

Clearing the cart when the pending order commits prevents a repeated submit from ordering the same cart again. If payment-session creation later fails, the order is cancelled and stock is released; this design does not automatically restore the cleared cart. The customer can add the products again. If the team wants cart restoration, that behavior must be designed before implementation.

### 4.3 Create payment session outside the transaction

After the database commit:

1. Build PaymentSessionRequest only from the saved order and order-item snapshots.
2. Replace {ORDER_ID} in the server-generated success and cancel URL templates with the new order ID.
3. Call IPaymentService.CreateCheckoutSessionAsync. TASK-40 uses the fake provider; TASK-49 replaces it with Stripe while keeping the same business contract.
4. Save the returned provider session ID on the Payment row, then return the provider URL through Result<string> so MVC can redirect the customer.

Use an idempotency key derived from the order or payment-attempt ID where the provider supports it. This limits duplicate sessions if the request is retried.

## 5. State transitions

| Event | Order status | Payment status | Stock action |
|---|---|---|---|
| Order and payment attempt committed | PendingPayment | Pending | Already reserved |
| Verified successful payment event | Paid | Succeeded | None |
| Provider confirms session expired or startup permanently fails | Cancelled | Expired or Failed | Release exactly once |
| Stale pending-order cleanup | Cancelled | Expired | Release exactly once |
| Vendor fulfillment completes | Completed | Succeeded | None |

Only valid transitions are applied. A successful webhook must not change a Cancelled order back to Paid. If a late provider success arrives for a cancelled order, record and reconcile it with the provider; do not reserve stock again or release stock twice.

## 6. Webhook and payment confirmation

The webhook handler must:

- Verify the provider signature against the raw request body.
- Record the provider event ID under a unique constraint so duplicate deliveries are harmless.
- Update Payment and Order together in a transaction.
- Mark payment Succeeded and order Paid only when the order is still PendingPayment and the event refers to the expected order/payment/session.
- Treat already-applied events as successful no-ops.
- Avoid granting fulfillment from a success or cancel browser redirect alone.

Stripe-specific event mapping and signature verification belong to TASK-49. The success and cancel result pages in TASK-50 read the saved order/payment state and display it; they do not change payment state.

## 7. Failure and recovery cases

| Failure | Required behavior |
|---|---|
| Cart is empty or invalid | Return a business failure. Create no order or payment. |
| Product/vendor becomes unavailable | Roll back and show the affected product or store issue. |
| Stock update affects zero rows | Roll back all reservations and records; ask the customer to refresh the cart. |
| Database write fails before commit | Roll back; retain cart lines and leave no partial order or reservation. |
| Payment-session creation fails after commit | Mark the payment attempt Failed, cancel the pending order, release stock in a new transaction, and return a useful checkout error. |
| Session exists but saving its ID fails | Attempt to expire the session, then cancel and release. Include OrderId in provider metadata so a later webhook can be reconciled. |
| Process stops after order commit but before session creation | The order remains PendingPayment with stock reserved. TASK-42 cleanup expires stale pending orders and releases stock after the configured age. |
| Customer returns through the cancel URL | Show the current state. Do not release stock solely because the browser visited the cancel URL; confirm or expire the provider session first. |
| Duplicate cancellation or cleanup call | Detect that the order is no longer PendingPayment and do not release stock again. |
| Duplicate payment webhook | The unique event record and state-transition checks make it a no-op. |

## 8. Task boundaries

- **TASK-23:** This design document.
- **TASK-39:** Implement pending order creation, snapshots, payment row, and atomic stock reservation.
- **TASK-40:** Shipping form, checkout result pages, and fake provider.
- **TASK-41:** Customer order list/details and pending-order cancellation.
- **TASK-42:** Periodic stale-order cleanup using the same cancellation and stock-release path.
- **TASK-49:** Real Stripe Checkout Session using saved order lines.
- **TASK-50:** Real success and cancel result pages.
- **TASK-51:** Useful payment-start error handling and cancellation/release when session creation fails.

## 9. Assumptions to confirm during implementation

- Currency remains the server-selected USD value currently used by Order and Payment; the client cannot choose it.
- Order total is the sum of merchandise line totals because the current schema has no tax or shipping-charge fields.
- Stock is reserved by decrementing Products.StockQuantity and restored only through the idempotent cancellation path.
- The pending order is committed before the provider session call, as required by the existing ICheckoutService contract.