# Policies and Roles

## Roles

| Role | Description |
|------|-------------|
| SuperAdmin | Full system access, manage vendors and commissions |
| Vendor | Manage own store, products, and orders |
| Customer | Browse products, place orders, leave reviews |

## Authorization Policies

### SuperAdminOnly
- Requires `SuperAdmin` role
- Used for: Admin area, commission settings, vendor management

### ApprovedVendor
- Requires `Vendor` role
- Requires approved vendor profile
- Used for: Vendor area, product management, order fulfillment

### CustomerOnly
- Requires authenticated user
- Not a vendor or admin
- Used for: Cart, checkout, order history

## Areas

### Admin Area (`/Admin`)
- Policy: `SuperAdminOnly`
- Controllers:
  - DashboardController
  - CommissionController
  - CategoryController
  - OrdersController
  - ProductsController

### Vendor Area (`/Vendor`)
- Policy: `ApprovedVendor`
- Controllers:
  - OrdersController
  - ProductsController
  - DashboardController

## Policy Names

```csharp
public static class PolicyNames
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string ApprovedVendor = "ApprovedVendor";
    public const string CustomerOnly = "CustomerOnly";
}
```
