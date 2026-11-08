# Team rules (TASK-06)

## Git
- Two permanent branches: `main` (stable, what we deliver) and `develop` (integration).
- One feature = one branch: `feature/<short-name>` created from `develop`.
- No direct push to `main` or `develop`. Every Pull Request is reviewed by a DIFFERENT person.
- Merge small PRs at least every 2 days. `develop` must always build.
- Commit message: short, imperative ("Add CartService.AddItem"). One idea per commit.

## Ownership (one owner per file)
| Person | Owns |
|---|---|
| A | Account, Program.cs, `_Layout`, DbSeeder, Identity/Policies, `Infrastructure/Features/IdentityFeature.cs`, migrations |
| B | Home, Products, Reviews, CatalogService, ReviewService, site.css, `CatalogFeature.cs` |
| C | Areas/Vendor (Products, Store, Dashboard), ProductService, IFileStorage, `VendorProductsFeature.cs` |
| D | Cart, Checkout, Orders, CartService, CheckoutService, Stripe Checkout Session, `OrdersFeature.cs` |
| E | Areas/Admin, Vendor Orders, CommissionService, Webhook, CategoryService, `AdminPaymentsFeature.cs` |

Need a change in a file you do not own? Ask the owner in the daily standup. Only A creates EF migrations.

## Validation style (agreed)
1. **ViewModel** (MVC): DataAnnotations for shape and format (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`) with a clear message.
2. **Service** (BLL): business rules (stock, ownership, status changes) returned as `Result.Fail("message")`. Never throw for an expected failure.
3. **Database**: constraints from the Configurations (unique, check, max length) are the last line of defence.
4. Controllers: `if (!ModelState.IsValid) return View(model);` then call the service, then flash the `Result` message and redirect (Post-Redirect-Get).

## Security rules every PR must respect
- Every controller has `[Authorize(...)]` or `[AllowAnonymous]` on purpose (the site is closed by default).
- Every POST is protected by antiforgery (global filter). The Stripe webhook is the only exception.
- A vendor id or user id is NEVER read from a form or URL. Use `ICurrentVendorService` / `CurrentUserId`.
- Someone else's data answers 404, exactly like data that does not exist.
- No secrets in the repository: user-secrets or environment variables.

## Definition of Done
Builds with no new warnings; tried by hand (happy path + 2 failures: bad input, user not allowed); validation in ViewModel AND Service; correct protection on the controller; PR reviewed by someone else and merged into `develop`.
