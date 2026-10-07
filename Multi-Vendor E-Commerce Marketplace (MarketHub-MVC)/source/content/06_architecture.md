# 6. معمارية النظام وهيكل المشروع (System Architecture)

## 6.1 مسؤوليات كل مكوّن

| المكوّن | المسؤولية | مثال في مشروعنا |
|---|---|---|
| **Controller** | يستقبل الـ Request، يتحقق من `ModelState`، ينادي Service، ويرجّع View أو Redirect. **مفيهوش Business Logic.** | `CartController.Add` |
| **Entity (Model)** | Class بيمثل جدول في الـ Database. | `Product`, `Order` |
| **ViewModel** | شكل البيانات اللي الـ View محتاجها أو الفورم بيبعتها، ومعاه Validation Attributes. بيمنع Overposting. | `ProductEditViewModel` |
| **View (Razor)** | عرض HTML فقط. | `Views/Products/Details.cshtml` |
| **Service** | كل الـ Business Rules (التحقق من المخزون، حساب العمولة، الـ Transactions). | `CheckoutService` |
| **Repository** | **مش هنستخدمه.** `DbContext` كفاية (انظر 1.3). | n/a |
| **ApplicationDbContext** | بوابة EF Core للـ Database وبيمثل Unit of Work. | `Data/ApplicationDbContext.cs` |
| **ASP.NET Core Identity** | Users, Roles, Password Hashing, Cookies. | `UserManager<ApplicationUser>` |
| **Middleware** | مكونات في الـ Pipeline بتعالج كل Request (HTTPS, Auth, Exception Handling). | `UseAuthentication()` |
| **Dependency Injection** | الـ Framework بيحقن الـ Services في الـ Constructors بدل ما نعمل `new`. | `AddScoped<ICartService, CartService>()` |
| **Configuration** | `appsettings.json` + `user-secrets` + Environment Variables. | `Stripe:SecretKey` |
| **Validation** | Data Annotations + Server-Side Checks في الـ Services. | `[Range(0.01, 100000)]` |
| **Logging** | `ILogger<T>` لتسجيل الأحداث المهمة. | تسجيل رفض Webhook |
| **Error Handling** | `UseExceptionHandler` + صفحات Error مخصصة. | `/Home/Error` |

## 6.2 هيكل الفولدرات المقترح

```text
MarketHub/
├── MarketHub.sln
├── global.json
├── README.md
├── .gitignore
├── .editorconfig
├── docs/                         (this documentation package + diagrams)
├── .github/
│   ├── workflows/ci.yml
│   └── pull_request_template.md
├── src/
│   └── MarketHub.Web/
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Areas/
│       │   ├── Vendor/
│       │   │   ├── Controllers/  (VendorBaseController, DashboardController,
│       │   │   │                  ProductsController, OrdersController, StoreController)
│       │   │   ├── ViewModels/
│       │   │   └── Views/
│       │   └── Admin/
│       │       ├── Controllers/  (DashboardController, VendorApplicationsController,
│       │       │                  CategoriesController, CommissionController, OrdersController)
│       │       ├── ViewModels/
│       │       └── Views/
│       ├── Controllers/          (HomeController, ProductsController, AccountController,
│       │                          CartController, CheckoutController, OrdersController,
│       │                          StripeWebhookController)
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/   (one file per entity)
│       │   ├── Migrations/
│       │   └── DbSeeder.cs
│       ├── Models/
│       │   ├── Entities/         (Product.cs, Order.cs, ...)
│       │   └── Enums/            (OrderStatus.cs, VendorStatus.cs, ...)
│       ├── Services/
│       │   ├── Interfaces/       (ICartService.cs, ICheckoutService.cs, ...)
│       │   ├── Implementations/  (CartService.cs, ...)
│       │   └── Options/          (StripeOptions.cs)
│       ├── Authorization/        (ApprovedVendorRequirement.cs, ApprovedVendorHandler.cs)
│       ├── ViewModels/           (shared: ProductListViewModel, CartViewModel, ...)
│       ├── Views/
│       │   ├── Shared/           (_Layout.cshtml, _ValidationScriptsPartial.cshtml)
│       │   ├── Home/ Products/ Cart/ Checkout/ Orders/ Account/
│       ├── wwwroot/              (css, js, lib, uploads/)
│       └── Infrastructure/       (FileStorageService, GlobalExceptionHandler)
└── tests/
    └── MarketHub.Tests/
        ├── Unit/                 (CommissionServiceTests, CartServiceTests)
        ├── Integration/          (CheckoutIntegrationTests, WebhookTests, AuthorizationTests)
        └── TestHelpers/          (TestDbContextFactory)
```

الهيكل ده مطابق للـ Diagrams 7 و 8: الـ Controllers والـ Areas (Presentation)، `Services` (Application)، `Data` (Data Access).

## 6.3 ليه `Areas/Vendor` و `Areas/Admin`؟ (تقييم)

| المزايا | العيوب |
|---|---|
| فصل واضح للـ Controllers والـ Views لكل Role. | محتاجين نكتب `[Area("Vendor")]` وـ `asp-area` في الـ Links. |
| نقدر نطبّق `[Authorize]` على الـ Base Controller للـ Area كلها. | Routing أعقد شوية (نحتاج `MapAreaControllerRoute`). |
| أقل تعارض في Git: كل Role في فولدر مستقل. | Shared Layout لكل Area محتاج `_ViewStart` خاص. |

**القرار [Recommendation]:** نستخدم Areas، لأن الأمان (Authorize على مستوى Area) والتنظيم أهم من البساطة اللحظية. الـ Customer Area مش محتاجة Area لأنها هي الـ Storefront الرئيسية.

## 6.4 دورة حياة الـ Request كاملة

(Diagram 21): نستخدم مثال `POST /Vendor/Products/Create`:

1. الـ **Browser** يبعت Request فيه الـ Cookie وفورم الـ Product وـ AntiForgery Token.
2. **Middleware Pipeline:** `UseHttpsRedirection` ثم `UseStaticFiles` ثم `UseRouting` ثم `UseAuthentication` (يقرأ الـ Cookie ويعرف المستخدم) ثم `UseAuthorization` (يطبق Policy `ApprovedVendor`).
3. **Routing** يوصّل الـ Request لـ `Areas/Vendor/ProductsController.Create`.
4. **Model Binding + Validation:** البيانات بتتحوّل لـ `ProductCreateViewModel`، و`ModelState` بيتملي بالأخطاء لو فيه.
5. الـ **Controller** يتأكد من `ModelState.IsValid`، وينادي `IProductService.CreateAsync`.
6. الـ **Service** يطبق Business Rules ويستخدم `ApplicationDbContext` (EF Core) اللي بيولّد SQL بـ Parameters ويرسله لـ **SQL Server**.
7. الـ **Controller** يرجّع `RedirectToAction("Index")` (نمط Post-Redirect-Get علشان منكررش الإرسال بالـ Refresh).
8. الـ **View** (في الـ GET التالي) بيتحوّل لـ HTML ويرجع للـ Browser.

## 6.5 أمثلة كود

### Entity

```csharp
public class Product
{
    public int Id { get; set; }
    public int VendorProfileId { get; set; }
    public VendorProfile Vendor { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
```

### ViewModel (من غير VendorProfileId)

```csharp
public class ProductCreateViewModel
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, 100_000)]
    public int StockQuantity { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public List<IFormFile> Images { get; set; } = new();
}
```

### Service Interface و Implementation

```csharp
public interface ICartService
{
    Task<Result> AddAsync(string customerId, int productId, int quantity);
    Task<CartViewModel> GetCartAsync(string customerId);
}

public class CartService : ICartService
{
    private readonly ApplicationDbContext _db;
    public CartService(ApplicationDbContext db) => _db = db;

    public async Task<Result> AddAsync(string customerId, int productId, int quantity)
    {
        if (quantity < 1) return Result.Fail("Quantity must be at least 1.");

        // Price and stock are always read from the database, never from the browser.
        var product = await _db.Products
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == productId
                && p.IsActive && p.Vendor.Status == VendorStatus.Approved);
        if (product is null) return Result.Fail("Product is not available.");

        var item = await _db.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);
        var newQty = (item?.Quantity ?? 0) + quantity;
        if (newQty > product.StockQuantity) return Result.Fail("Not enough stock.");

        if (item is null)
            _db.CartItems.Add(new CartItem { CustomerId = customerId, ProductId = productId, Quantity = newQty });
        else
            item.Quantity = newQty;

        await _db.SaveChangesAsync();
        return Result.Ok();
    }
}
```

### Controller

```csharp
[Authorize(Roles = Roles.Customer)]
public class CartController : Controller
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _cart.AddAsync(userId, productId, quantity);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}
```

### Program.cs (الهيكل العام)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(/* options */)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<ICurrentVendorService, CurrentVendorService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

await DbSeeder.SeedAsync(app.Services);
app.Run();
```

> **[Team Decision]:** ترتيب الـ Middleware مهم جداً (Authentication قبل Authorization). راجعوا Microsoft Learn: ASP.NET Core Middleware للترتيب الموصى به في النسخة المستخدمة، وجرّبوا الـ Template الرسمي بتاع MVC كمرجع.

### ملفات الإعدادات (Configuration)

```json
{
  "ConnectionStrings": { "DefaultConnection": "" },
  "Stripe": { "SecretKey": "", "WebhookSecret": "", "PublishableKey": "" },
  "Seed": { "AdminEmail": "", "AdminPassword": "" }
}
```

القيم الحقيقية **بتتحط** في `dotnet user-secrets` محلياً، وفي Environment Variables على الاستضافة (مثلاً `Stripe__SecretKey`). الملف المرفوع على GitHub فيه مفاتيح فاضية بس.

## 6.6 Validation و Logging و Error Handling

- **Validation طبقتين:** Data Annotations على الـ ViewModel (شكل البيانات) + Business Validation في الـ Service (المخزون، الملكية). الـ Client-Side Validation تحسين للـ UX فقط، ومش بديل.
- **Logging:** `ILogger<T>` لتسجيل: رفض توقيع Webhook، فشل Transaction، محاولات الوصول المرفوضة. **ممنوع** نسجّل Secrets أو بيانات كارت أو Passwords.
- **Error Handling:** `UseExceptionHandler` في Production، وصفحة `Error` بتعرض رسالة عامة ومعاها Request Id. الـ Services بترجّع `Result` للأخطاء المتوقعة (مخزون ناقص) وبترمي Exceptions للأخطاء غير المتوقعة فقط.

---pagebreak---
