# Payment and Webhook Flow

## Stripe Integration

### Checkout Flow

```
Customer          MarketHub              Stripe
   │                  │                    │
   │  Checkout        │                    │
   ├─────────────────►│                    │
   │                  │  Create Session    │
   │                  ├───────────────────►│
   │                  │  Session URL       │
   │                  │◄───────────────────┤
   │  Redirect        │                    │
   │◄─────────────────┤                    │
   │                  │                    │
   │  Pay on Stripe   │                    │
   ├──────────────────┼───────────────────►│
   │                  │                    │
   │                  │  Webhook Event     │
   │                  │◄───────────────────┤
   │                  │  200 OK            │
   │                  ├───────────────────►│
   │  Success Page    │                    │
   │◄─────────────────┤                    │
```

## Webhook Events

### checkout.session.completed
- Order status → `Paid`
- Payment status → `Succeeded`
- Commission records created

### checkout.session.expired
- Order status → `Cancelled`
- Stock returned

### payment_intent.payment_failed
- Payment status → `Failed`
- Order status → `Cancelled`
- Stock returned

## Idempotency

- Each Stripe event has unique `StripeEventId`
- Events stored in `StripeWebhookEvents` table
- Duplicate events are ignored

## Webhook Endpoint

```
POST /api/webhook/stripe
```

- Raw body required for signature verification
- Returns 200 OK on success
- Returns 400 Bad Request on signature failure
