# 5. تصميم قاعدة البيانات (Database Design)

الـ ERD الكامل في الفصل 4 (Diagram 6A و 6B و 6C). الفصل ده بيشرح كل جدول: ليه موجود، أعمدته، قيوده، وإزاي بيتصرف عند الحذف.

## 5.1 ازاي جداول ASP.NET Core Identity بتتوافق مع تصميمنا

لما نستخدم `Identity` مع EF Core بيتعمل تلقائياً جداول: `AspNetUsers` و`AspNetRoles` و`AspNetUserRoles` و`AspNetUserClaims` و`AspNetUserLogins` و`AspNetUserTokens` و`AspNetRoleClaims`. إحنا مش بنعيد اختراع المستخدمين: بنعمل Class اسمه `ApplicationUser : IdentityUser` فيه أعمدة إضافية (`FullName`, `IsActive`, `CreatedAt`)، وبنربط باقي جداولنا بـ `AspNetUsers.Id` (من نوع `nvarchar(450)`) كـ Foreign Key. الـ Roles (`Customer`, `Vendor`, `SuperAdmin`) بتتخزن في `AspNetRoles` والربط في `AspNetUserRoles`.

كل جداول المشروع بتستخدم `int IDENTITY` كـ Primary Key **ما عدا** جداول Identity (string GUID).

## 5.2 قرار نمذجة الـ Multi-Vendor Order

| المعيار | Approach A: Order + OrderItems (كل Item فيه VendorId) | Approach B: Order + VendorOrders + OrderItems |
|---|---|---|
| عدد الجداول | أقل (جدول Order واحد) | أكتر (`VendorOrders` زيادة) |
| صعوبة التنفيذ للمبتدئين | **سهلة** | متوسطة إلى صعبة |
| Vendor-specific visibility | `WHERE OrderItems.VendorProfileId = @vendor` | `WHERE VendorOrders.VendorProfileId = @vendor` أسهل في الاستعلام |
| Order Status | حالة واحدة للـ Order وحالة Fulfillment لكل Item | حالة لكل VendorOrder + حالة للـ Parent، وده بيحتاج قواعد تجميع |
| Inventory | واحد في الحالتين (على `Products`) | نفس الشيء |
| Commission | `CommissionRecord` لكل OrderItem | ممكن لكل VendorOrder أو Item |
| Shipping per vendor | محتاج حقول على الـ Item | طبيعي (Tracking per VendorOrder) |
| Future extensibility | ممكن ترقيته لـ B لاحقاً | مناسب لـ Stripe Connect Transfers وشحن لكل Vendor |

### التوصية [Recommendation]: **Approach A**

للـ MVP هنستخدم Approach A لأنها أبسط، وكل متطلباتنا بتتغطي: الـ Vendor يشوف `OrderItems` بتاعته بس، والمخزون على `Products`، والعمولة لكل Item، والـ Status للـ Order الكامل بيتحدد من `Paid` للدفع (الدفع واحد في الحالتين) مع `FulfillmentStatus` على كل Item للشحن. **الـ `VendorOrder` اتقيّم ورُفض للـ MVP** لأنه بيضيف جدول وقواعد تجميع حالات (إيه حالة الـ Order لو Vendor شحن و Vendor لأ؟) من غير فايدة فورية. لو احتجنا لاحقاً Shipping لكل Vendor أو Stripe Connect Transfers هنضيف `VendorOrder` ونملأه من الـ `OrderItems` بـ Migration (Data Migration)، لأن `OrderItem.VendorProfileId` موجود بالفعل.

## 5.3 قاموس البيانات (Data Dictionary)

### ملاحظات عامة على الأنواع

| الاستخدام | نوع SQL Server | C# | السبب |
|---|---|---|---|
| المبالغ المالية | `decimal(18,2)` | `decimal` | دقة ثابتة، **مفيش `float` أو `double` للفلوس أبداً** (أخطاء تقريب). |
| النسب المئوية | `decimal(5,2)` | `decimal` | يغطي 0.00 إلى 100.00. |
| التواريخ | `datetime2` | `DateTime` (UTC) | دقة أعلى ونطاق أوسع، وبنخزن UTC دايماً. |
| النصوص | `nvarchar(n)` | `string` | Unicode للعربي والإنجليزي، وبنحدد الطول. |
| الـ Enums | `nvarchar(30)` | enum بـ `HasConversion<string>()` | مقروءة في الـ SQL ومبتتكسرش لو غيّرنا ترتيب الـ Enum. |
| Concurrency | `rowversion` | `byte[]` | بيتغير تلقائياً مع كل Update. |

### ApplicationUser (جدول AspNetUsers)

**الغرض:** هوية المستخدم. الأعمدة الإضافية بتتضاف على جدول Identity.

| العمود | النوع | Null | قيود وملاحظات |
|---|---|---|---|
| Id | nvarchar(450) | لا | PK |
| Email / UserName | nvarchar(256) | لا | Unique (من Identity) |
| FullName | nvarchar(100) | لا | Required, MaxLength(100) |
| IsActive | bit | لا | Default 1، للحظر بدل الحذف |
| CreatedAt | datetime2 | لا | Default `SYSUTCDATETIME()` |

**العلاقات:** 1 إلى 0..1 مع `VendorProfile`، 1 إلى كثير مع `Order` و`CartItem` و`Review`. **Delete:** مفيش حذف مستخدمين في الـ MVP (Deactivate بدلاً منه) لحماية التاريخ المالي.

### VendorProfile

**الغرض:** بيانات المتجر وحالة الموافقة. فصل جدول عن المستخدم لأن مش كل مستخدم Vendor.

| العمود | النوع | Null | قيود وملاحظات |
|---|---|---|---|
| Id | int | لا | PK, IDENTITY |
| UserId | nvarchar(450) | لا | FK إلى AspNetUsers، **Unique** (1 إلى 1) |
| StoreName | nvarchar(100) | لا | **Unique**، Required |
| Description | nvarchar(1000) | نعم | |
| LogoPath | nvarchar(260) | نعم | مسار نسبي |
| Status | nvarchar(20) | لا | `Pending/Approved/Rejected/Suspended`، Default Pending |
| RejectionReason | nvarchar(500) | نعم | إجباري عند Rejected (Validation في الـ Service) |
| ReviewedByUserId | nvarchar(450) | نعم | FK إلى AspNetUsers (الـ Admin) |
| ReviewedAt | datetime2 | نعم | |
| CreatedAt | datetime2 | لا | |

**Delete:** `Restrict` من User. **Indexes:** Unique(`UserId`)، Unique(`StoreName`)، Index(`Status`) لقائمة Pending.

### Category

**الغرض:** تصنيف المنتجات (Flat بدون Parent/Child في الـ MVP لتبسيط الـ UI).

| العمود | النوع | Null | قيود |
|---|---|---|---|
| Id | int | لا | PK |
| Name | nvarchar(80) | لا | **Unique** |
| Slug | nvarchar(100) | لا | **Unique**، للـ URLs |
| IsActive | bit | لا | Default 1 |

**Delete:** `Restrict` (لو فيه Products)؛ بنعمل Deactivate. الـ Admin فقط يدير الـ Categories.

### Product

**الغرض:** المنتج اللي بيبيعه Vendor واحد.

| العمود | النوع | Null | قيود وملاحظات |
|---|---|---|---|
| Id | int | لا | PK |
| VendorProfileId | int | لا | FK، **أساس العزل** |
| CategoryId | int | لا | FK |
| Name | nvarchar(150) | لا | Required |
| Description | nvarchar(4000) | نعم | |
| Price | decimal(18,2) | لا | `CHECK (Price > 0)` |
| StockQuantity | int | لا | `CHECK (StockQuantity >= 0)` |
| IsActive | bit | لا | Default 1 |
| RowVersion | rowversion | لا | Concurrency Token |
| CreatedAt / UpdatedAt | datetime2 | لا / نعم | |

**Indexes:** `(VendorProfileId, IsActive)` لصفحة المنتجات عند الـ Vendor، `(CategoryId, IsActive, Price)` للفلترة، `Name` للبحث. **Delete:** `Restrict` من Vendor ومن Category. **قاعدة:** لا حذف لمنتج له OrderItems (BR-09).

### ProductImage

| العمود | النوع | Null | قيود |
|---|---|---|---|
| Id | int | لا | PK |
| ProductId | int | لا | FK، **Cascade** (الصور ملهاش معنى من غير المنتج) |
| ImagePath | nvarchar(260) | لا | اسم ملف يولّده السيرفر (GUID) |
| IsPrimary | bit | لا | صورة رئيسية واحدة (Validation في الـ Service) |
| SortOrder | int | لا | Default 0 |

### CartItem

**الغرض:** محتويات الـ Cart لكل Customer. **مفيش عمود سعر** (BR-03).

| العمود | النوع | Null | قيود |
|---|---|---|---|
| Id | int | لا | PK |
| CustomerId | nvarchar(450) | لا | FK، Cascade |
| ProductId | int | لا | FK، Cascade |
| Quantity | int | لا | `CHECK (Quantity > 0)` |
| AddedAt | datetime2 | لا | |

**Unique Index** على `(CustomerId, ProductId)`: يمنع تكرار الصف ويجبر الكود يزوّد الكمية.

### Order

**الغرض:** طلب العميل الواحد (حتى لو من كذا Vendor).

| العمود | النوع | Null | قيود وملاحظات |
|---|---|---|---|
| Id | int | لا | PK |
| OrderNumber | nvarchar(30) | لا | **Unique**، مثل `MH-20261007-483920` |
| CustomerId | nvarchar(450) | لا | FK، Restrict |
| Status | nvarchar(20) | لا | `PendingPayment/Paid/Completed/Cancelled` |
| TotalAmount | decimal(18,2) | لا | مجموع `LineTotal` (محفوظ للمطابقة مع Stripe) |
| Currency | char(3) | لا | Default `USD` |
| ShippingFullName / Phone / Address / City / Country | nvarchar | لا | **Snapshot** وقت الطلب |
| CreatedAt | datetime2 | لا | |
| PaidAt | datetime2 | نعم | |

**Indexes:** Unique(`OrderNumber`)، `(CustomerId, CreatedAt DESC)`، Index(`Status`).

### OrderItem

**الغرض:** صف في الطلب، تابع لـ Vendor واحد. ده قلب الـ Marketplace.

| العمود | النوع | Null | قيود وملاحظات |
|---|---|---|---|
| Id | int | لا | PK |
| OrderId | int | لا | FK، Cascade |
| ProductId | int | لا | FK، Restrict |
| VendorProfileId | int | لا | FK، Restrict، **Snapshot وقت الشراء** |
| ProductNameSnapshot | nvarchar(150) | لا | |
| UnitPrice | decimal(18,2) | لا | سعر وقت الشراء |
| Quantity | int | لا | `CHECK (Quantity > 0)` |
| LineTotal | decimal(18,2) | لا | `CHECK (LineTotal = UnitPrice * Quantity)` |
| FulfillmentStatus | nvarchar(20) | لا | `Pending/Shipped/Delivered/Cancelled` |
| ShippedAt / DeliveredAt | datetime2 | نعم | |

**Indexes:** `(VendorProfileId, OrderId)` لشاشة الـ Vendor، `(OrderId)`، `(ProductId)`.

### Payment

**الغرض:** محاولات الدفع للـ Order (1 إلى كثير لأن العميل ممكن يعيد المحاولة بعد انتهاء Session).

| العمود | النوع | Null | قيود |
|---|---|---|---|
| Id | int | لا | PK |
| OrderId | int | لا | FK، Restrict |
| Provider | nvarchar(20) | لا | `Stripe` |
| StripeCheckoutSessionId | nvarchar(255) | نعم حتى الإنشاء | **Unique** (Filtered `IS NOT NULL`) |
| StripePaymentIntentId | nvarchar(255) | نعم | Index |
| Amount / Currency | decimal(18,2) / char(3) | لا | |
| Status | nvarchar(20) | لا | `Pending/Succeeded/Failed/Expired` |
| FailureMessage | nvarchar(500) | نعم | |
| CreatedAt / CompletedAt | datetime2 | لا / نعم | |

**مهم:** مفيش أي بيانات كارت هنا. بنخزّن Stripe IDs فقط.

### StripeWebhookEvent

**الغرض:** ضمان الـ Idempotency. كل Event بيوصل بيتسجّل هنا مرة واحدة.

| العمود | النوع | قيود |
|---|---|---|
| Id | int | PK |
| StripeEventId | nvarchar(255) | **Unique** (مثل `evt_...`) |
| EventType | nvarchar(100) | |
| ProcessedAt | datetime2 | |

دي Entity إضافية بسبب واضح: من غيرها مفيش طريقة آمنة لمنع تنفيذ نفس الـ Event مرتين.

### CommissionSetting

**الغرض:** تاريخ نسب العمولة. الـ Admin مبيعدّلش الصف القديم، بيضيف صف جديد.

| العمود | النوع | قيود |
|---|---|---|
| Id | int | PK |
| RatePercent | decimal(5,2) | `CHECK (RatePercent BETWEEN 0 AND 100)` |
| EffectiveFrom | datetime2 | |
| CreatedByUserId | nvarchar(450) | FK (Admin) |
| IsActive | bit | **Filtered Unique Index** على `IsActive = 1`: صف نشط واحد فقط |

### CommissionRecord

**الغرض:** سجل مالي لا يتغير: كل OrderItem مدفوع له سجل واحد.

| العمود | النوع | قيود وملاحظات |
|---|---|---|
| Id | int | PK |
| OrderItemId | int | FK، **Unique** (1 إلى 1) |
| VendorProfileId | int | FK |
| CommissionSettingId | int | FK |
| GrossAmount | decimal(18,2) | = `OrderItem.LineTotal` |
| RatePercentSnapshot | decimal(5,2) | النسبة وقت الدفع |
| CommissionAmount | decimal(18,2) | `ROUND(Gross × Rate / 100, 2)` |
| VendorNetAmount | decimal(18,2) | `Gross − CommissionAmount`، `CHECK (VendorNetAmount = GrossAmount - CommissionAmount)` |
| Status | nvarchar(20) | `Recorded/Reversed` (Reversed للمستقبل مع Refunds) |
| CreatedAt | datetime2 | |

### Review (Should Have)

| العمود | النوع | قيود |
|---|---|---|
| Id | int | PK |
| ProductId | int | FK، Cascade |
| CustomerId | nvarchar(450) | FK، Restrict |
| Rating | int | `CHECK (Rating BETWEEN 1 AND 5)` |
| Comment | nvarchar(1000) | نعم |
| CreatedAt | datetime2 | |

**Unique** على `(ProductId, CustomerId)`. القاعدة BR-12 (لازم الـ Customer يكون استلم المنتج) بتتفرض في الـ Service.

### ليه SQL Server بيمنع Multiple Cascade Paths؟

لو جدول له أكتر من طريق Cascade لنفس الصف (مثلاً `OrderItem` من `Order` ومن `VendorProfile`)، SQL Server بيرفض الـ Migration. عشان كده بنخلي `Cascade` فقط في العلاقات اللي الابن مالوش معنى بدون الأب (`ProductImage`, `CartItem`, `OrderItem→Order`, `Review→Product`) و`Restrict` في الباقي.

## 5.4 تحليل الـ Normalization

- **1NF:** كل عمود قيمة واحدة (مفيش قوائم في عمود). الصور في جدول `ProductImages` مش في عمود.
- **2NF:** كل الجداول بـ PK مفردة (`Id`) فمفيش Partial Dependency.
- **3NF:** مفيش Transitive Dependency: اسم الـ Category في `Categories` وليس في `Products`.

**Denormalization مقصودة (Snapshots)** بتخدم دقة التاريخ المالي:

| العمود | ليه مكرر؟ |
|---|---|
| `OrderItem.UnitPrice`, `ProductNameSnapshot` | سعر واسم المنتج ممكن يتغيروا بعدين، والطلب القديم لازم يفضل زي ما اتدفع. |
| `OrderItem.VendorProfileId` | مشتق من `Product`، بس بنثبّته علشان استعلامات الـ Vendor سريعة وعلشان الـ Isolation مايعتمدش على Join. |
| `Order.TotalAmount` | يتحسب من الـ Items، بس بنخزّنه للمطابقة مع Stripe. |
| `CommissionRecord.RatePercentSnapshot`, `GrossAmount` | تغيير النسبة أو السعر ميغيّرش سجلات الماضي. |
| `Order.Shipping*` | العنوان وقت الطلب، حتى لو الـ Customer غيّر بياناته. |

## 5.5 إعداد EF Core (Fluent API)

بنحط كل Entity Configuration في Class لوحده تحت `Data/Configurations` علشان `ApplicationDbContext` يفضل صغير، وكمان علشان الأعضاء ميتعارضوش في نفس الملف.

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products", t =>
        {
            t.HasCheckConstraint("CK_Products_Price", "[Price] > 0");
            t.HasCheckConstraint("CK_Products_Stock", "[StockQuantity] >= 0");
        });
        b.Property(p => p.Name).HasMaxLength(150).IsRequired();
        b.Property(p => p.Price).HasColumnType("decimal(18,2)");
        b.Property(p => p.RowVersion).IsRowVersion();
        b.HasOne(p => p.Vendor).WithMany(v => v.Products)
            .HasForeignKey(p => p.VendorProfileId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(p => new { p.VendorProfileId, p.IsActive });
        b.HasIndex(p => new { p.CategoryId, p.IsActive, p.Price });
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems", t =>
            t.HasCheckConstraint("CK_OrderItems_LineTotal", "[LineTotal] = [UnitPrice] * [Quantity]"));
        b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(i => i.LineTotal).HasColumnType("decimal(18,2)");
        b.Property(i => i.FulfillmentStatus).HasConversion<string>().HasMaxLength(20);
        b.HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(i => i.Vendor).WithMany().HasForeignKey(i => i.VendorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(i => new { i.VendorProfileId, i.OrderId });
    }
}

public class StripeWebhookEventConfiguration : IEntityTypeConfiguration<StripeWebhookEvent>
{
    public void Configure(EntityTypeBuilder<StripeWebhookEvent> b)
    {
        b.Property(e => e.StripeEventId).HasMaxLength(255).IsRequired();
        b.HasIndex(e => e.StripeEventId).IsUnique();
    }
}
```

وفي `ApplicationDbContext`:

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder); // IMPORTANT: configures Identity tables
    builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
}
```

## 5.6 استراتيجية الـ Migrations

1. **Code-First:** نغيّر الـ Entity والـ Configuration، وبعدين `dotnet ef migrations add AddVendorProfile`، ثم `dotnet ef database update`.
2. اسم الـ Migration يوصف الـ Feature (`AddOrdersAndOrderItems`)، ومتتسماش `Update1`.
3. **ممنوع تعديل Migration اتعمله Merge في `main`.** لو فيه غلط، نعمل Migration جديدة.
4. قواعد التنسيق بين الأعضاء في الفصل 11 (عضو واحد بيعمل Migration لكل Pull Request، وبنعمل Rebase قبل إنشائها).
5. على Production: نطبّق الـ Migrations بـ Script (`dotnet ef migrations script --idempotent`) أو خطوة منفصلة في الـ Deployment، **مش تلقائياً عند بدء التطبيق** ([Team Decision]، التلقائي مقبول للـ Demo فقط).

## 5.7 استراتيجية الـ Seed Data

| البيانات | البيئة | الطريقة |
|---|---|---|
| Roles (Customer, Vendor, SuperAdmin) | كل البيئات | `DbSeeder` يتأكد من وجودها (Idempotent) |
| Super Admin (Email/Password من Configuration) | كل البيئات | من `user-secrets` أو Environment Variables |
| `CommissionSetting` الافتراضي (10%) | كل البيئات | يتزرع لو مفيش صف نشط |
| Categories أساسية | كل البيئات | `HasData` أو الـ `DbSeeder` |
| Demo Vendors + Products + Customers | Development / Demo فقط | `DbSeeder` بشرط `env.IsDevelopment()` أو Flag واضح |

## 5.8 قرارات التصميم المهمة (ملخص)

1. **Approach A** للـ Orders (شرح 5.2).
2. **Reserve Stock** عند إنشاء الـ Order بدل الخصم بعد الدفع، علشان نمنع Overselling ونبسّط الـ Webhook.
3. **Snapshots** للأسعار والعناوين والعمولة.
4. **جدول `StripeWebhookEvent`** للـ Idempotency.
5. **`CommissionSetting` Append-Only** مع Filtered Unique Index.
6. **Enums كـ string** للقراءة.
7. **لا حذف فيزيائي** للـ Users والـ Products المرتبطة بـ Orders.
8. **Cart في الـ Database** مش Session، علشان يفضل بعد إعادة تشغيل السيرفر ويشتغل على كذا جهاز.

---pagebreak---
