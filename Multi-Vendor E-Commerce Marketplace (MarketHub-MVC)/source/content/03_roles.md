# 3. الأدوار والصلاحيات (User Roles and Authorization)

## 3.1 الفرق بين Authentication و Authorization

- **Authentication** معناها: *مين انت؟* بنتأكد من هوية المستخدم (Email + Password). بيتم عن طريق `ASP.NET Core Identity` وبيطلع Cookie بعد تسجيل الدخول.
- **Authorization** معناها: *انت مسموحلك تعمل إيه؟* بنستخدم `Role-Based Authorization` (Customer, Vendor, SuperAdmin) و `Policy-Based Authorization` (زي `ApprovedVendor`).

هنستخدم الاتنين مع بعض: الـ Role بتحدد النوع العام، والـ Policy بتضيف شروط دقيقة زي "الـ Vendor لازم يكون Approved".

## 3.2 تحليل الـ Actors

| الـ Actor | Authentication | Permissions / Actions | Restricted | Accessible Pages | Use Cases |
|---|---|---|---|---|---|
| **Guest** | مش مطلوب | تصفح، بحث، تفاصيل منتج، Register، Login، Vendor Application | Cart, Checkout, Orders, أي Area | `/`, `/Products`, `/Products/Details/{id}`, `/Account/*` | UC-01..06 |
| **Customer** | `[Authorize(Roles="Customer")]` | كل اللي بيعمله Guest + Cart, Checkout, Payment, Orders بتاعته، Reviews | بيانات Vendors، Admin Area، Orders غيره | `/Cart`, `/Checkout`, `/Orders`, `/Account/Manage` | UC-07..13 |
| **Vendor** | Role `Vendor` + Policy `ApprovedVendor` | Store Profile، Products بتاعته، Inventory، OrderItems بتاعته، Sales/Commission بتاعته | منتجات/Orders/Commissions Vendor تاني، Admin Area، تعديل نسبة العمولة | `/Vendor/*` | UC-14..22 |
| **Super Admin** | Role `SuperAdmin` | Approvals، Suspend، Categories، Commission Rate، كل الـ Orders، Reports، Moderation | لا يشتري ولا يبيع (حساب إداري) | `/Admin/*` | UC-23..29 |

> الـ `Super Admin` بيتعمل من الـ `DbSeeder` عند أول تشغيل، وبياناته (Email/Password) بتيجي من `user-secrets` أو Environment Variables، **مش مكتوبة في الـ Code**.

## 3.3 تطبيق الـ Roles بـ ASP.NET Core Identity

### الخطوة 1: ApplicationUser و Roles

```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public VendorProfile? VendorProfile { get; set; }
}

public static class Roles
{
    public const string Customer = "Customer";
    public const string Vendor = "Vendor";
    public const string SuperAdmin = "SuperAdmin";
}
```

### الخطوة 2: تسجيل Identity في `Program.cs`

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false; // MVP: no email provider yet
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApprovedVendor", p => p.AddRequirements(new ApprovedVendorRequirement()))
    .AddPolicy("SuperAdminOnly", p => p.RequireRole(Roles.SuperAdmin));

builder.Services.AddScoped<IAuthorizationHandler, ApprovedVendorHandler>();
```

> **[Team Decision]:** اسم الـ Methods (`AddIdentity` مقابل `AddIdentityCore`) والإعدادات الافتراضية بتتغير بين الإصدارات. راجعوا Microsoft Learn: Introduction to Identity on ASP.NET Core للنسخة اللي بتستخدموها.

### الخطوة 3: Policy بتتأكد إن الـ Vendor معتمد فعلاً

```csharp
public class ApprovedVendorRequirement : IAuthorizationRequirement { }

public class ApprovedVendorHandler : AuthorizationHandler<ApprovedVendorRequirement>
{
    private readonly ApplicationDbContext _db;
    public ApprovedVendorHandler(ApplicationDbContext db) => _db = db;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ApprovedVendorRequirement requirement)
    {
        if (!context.User.IsInRole(Roles.Vendor)) return;
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var approved = await _db.VendorProfiles
            .AnyAsync(v => v.UserId == userId && v.Status == VendorStatus.Approved);
        if (approved) context.Succeed(requirement);
    }
}
```

الـ Handler بيسأل الـ Database في كل Request. ده أمان أعلى: لو الـ Admin عمل `Suspend` للـ Vendor، الصلاحية بتتقطع فوراً حتى لو الـ Cookie لسه بتقول إنه Vendor.

### الخطوة 4: تطبيق الـ Policy على الـ Area كلها

```csharp
[Area("Vendor")]
[Authorize(Policy = "ApprovedVendor")]
public abstract class VendorBaseController : Controller
{
    protected readonly ICurrentVendorService CurrentVendor;
    protected VendorBaseController(ICurrentVendorService currentVendor) => CurrentVendor = currentVendor;
}
```

كل Controller في `Areas/Vendor` بيورّث من `VendorBaseController`، فمستحيل ننسى الـ `[Authorize]` في Controller جديد.

### الخطوة 5: الـ Role بتتضاف عند الموافقة

عند موافقة الـ Admin على الـ Vendor Application، بنعمل `userManager.AddToRoleAsync(user, Roles.Vendor)` ثم `userManager.UpdateSecurityStampAsync(user)`. الـ `SecurityStamp` بيخلّي الـ Cookie القديم يتعمل له Revalidation، فالـ Role الجديدة تظهر من غير ما المستخدم يعيد الدخول يدوياً [Assumption: راجعوا إعدادات `SecurityStampValidatorOptions.ValidationInterval`].

## 3.4 عزل الـ Vendors (Vendor Isolation)

الـ Vendor A **لازم** ما يقدرش يشوف أو يعدّل منتجات أو Inventory أو OrderItems أو Commissions بتاعة Vendor B. بنطبّق ده بثلاث طبقات (Defense in Depth):

1. **Policy `ApprovedVendor`** على مستوى الـ Area.
2. **`ICurrentVendorService`** بيحدد `VendorProfileId` للمستخدم الحالي من الـ Database/Claims، **مش من أي Form أو Query String**.
3. **كل Query في Vendor Services بتفلتر بـ `VendorProfileId`**، وبترجّع `NotFound` بدل `Forbidden` علشان منكشفش وجود بيانات الـ Vendor التاني.

```csharp
public async Task<Product?> GetOwnedProductAsync(int productId, int vendorId) =>
    await _db.Products
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == productId && p.VendorProfileId == vendorId);
```

وفي الـ Controller:

```csharp
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
{
    if (!ModelState.IsValid) return View(model);
    var vendorId = await CurrentVendor.GetVendorProfileIdAsync();
    var ok = await _productService.UpdateAsync(id, vendorId, model); // checks ownership inside
    return ok ? RedirectToAction(nameof(Index)) : NotFound();
}
```

لاحظ إن `ProductEditViewModel` **مفيهوش** `VendorProfileId` خالص، ده بيمنع Overposting.

## 3.5 ليه إخفاء الزرار أو القائمة مش كفاية؟

لو اكتفينا بإننا نخفي زرار "Edit" بتاع منتج Vendor تاني في الـ View، ده بيحسّن الـ UX بس **مش أمان**. أي شخص يقدر:

- يكتب الـ URL بإيده: `/Vendor/Products/Edit/17`.
- يبعت POST Request بـ Postman أو `curl` أو من Developer Tools من غير ما يمر بالـ UI أصلاً.
- يغيّر `id` المخفي في الـ Form (ده اسمه **IDOR: Insecure Direct Object Reference**).

الـ Browser مكان مش موثوق. القاعدة: **Hide in UI for convenience, enforce on the Server for security**. وده اللي بنختبره في Test Cases الفصل 12 (Vendor A يحاول يعدّل منتج Vendor B).

---pagebreak---
