# 7. مواصفات الـ Features (Feature Specifications)

كل Feature هنا بتتبني كـ **Vertical Slice**: Database + Service + Controller + View + Tests مع بعض، وبمشاركة كل الفريق (الفصل 10). الجداول التالية هي الـ Checklist اللي بنراجع عليها قبل ما نقول إن الـ Feature خلصت.

## 7.1 A. Public Storefront (Home, Listing, Details, Categories, Search, Filter, Sort, Pagination)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | الـ Guest والـ Customer يتصفحوا منتجات نشطة من متاجر Approved بس، ويلاقوا اللي عايزينه بسرعة. |
| **User Stories** | US-01, US-05. |
| **Dependencies** | Categories + Products + ProductImages + VendorProfile (Seed Data). |
| **Database Changes** | لا جداول جديدة. Indexes: `(CategoryId, IsActive, Price)` و`Name`. |
| **Backend Tasks** | `IProductService.SearchAsync(ProductQuery)` يبني Query بـ `IQueryable` (شرط Active + Approved، بعدها Search/Category/Price/Sort)، ويرجّع `PagedResult<T>` بـ `Skip/Take`. استخدموا `AsNoTracking()` للقراءة. |
| **Frontend Tasks** | Home بـ Featured + Categories، Listing بـ Grid (Bootstrap Cards) وفورم فلترة (GET)، Pagination Component، Details بصور وزرار Add to Cart. |
| **Authorization Rules** | Public (`[AllowAnonymous]`). المنتجات غير النشطة أو الخاصة بـ Vendor غير معتمد ترجع 404. |
| **Validation** | `page ≥ 1`، `pageSize` في نطاق مسموح (12-48)، قيم Sort من قائمة ثابتة (Whitelist). |
| **Error Scenarios** | Category غير موجودة، صفحة خارج النطاق، Search فاضي. |
| **Testing Requirements** | Unit: فلترة وترتيب ومنتجات Inactive مخفية. Manual: Pagination على موبايل. |
| **Definition of Done** | كل المنتجات المعروضة Active + Approved، الصفحات سريعة، Responsive. |

## 7.2 B. Authentication (Registration, Login, Logout, Roles)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | تسجيل المستخدمين ودخولهم، وتحديد الـ Roles. |
| **User Stories** | US-02, US-03. |
| **Dependencies** | ASP.NET Core Identity + `DbSeeder` للـ Roles والـ Super Admin. |
| **Database Changes** | Identity Migration + أعمدة `FullName`, `IsActive`, `CreatedAt` على `ApplicationUser`. |
| **Backend Tasks** | `AccountController` (Register, Login, Logout) باستخدام `SignInManager` و`UserManager`، إضافة Role `Customer` تلقائياً، Lockout، منع دخول `IsActive = false`. |
| **Frontend Tasks** | فورم Register/Login بـ Validation Summary، تغيير شكل الـ Navbar حسب الـ Role. |
| **Authorization Rules** | `[Authorize]` على صفحات الـ Customer والـ Areas، `[AllowAnonymous]` على Account. |
| **Validation** | Email Format وتفرد، Password Policy، AntiForgery على كل POST، `returnUrl` يتفحص بـ `Url.IsLocalUrl`. |
| **Error Scenarios** | Email مكرر، Password ضعيف، حساب مغلق، Lockout. |
| **Testing Requirements** | Integration: دخول بـ Role صح/غلط. Manual: Open Redirect. |
| **Definition of Done** | المستخدم يسجل ويدخل ويخرج، والـ Roles محمية على السيرفر. |

## 7.3 C. Vendor Management (Application, Approval, Dashboard, Store Profile, Order Items, Summaries)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | فتح متجر جديد بموافقة الـ Admin وإدارته. |
| **User Stories** | US-03, US-04, US-13, US-16, US-17. |
| **Dependencies** | Identity، `VendorProfile`، Orders/OrderItems/CommissionRecords (لشاشات الـ Dashboard). |
| **Database Changes** | `VendorProfiles` + Unique على `UserId` و`StoreName`. |
| **Backend Tasks** | `VendorService` (Apply, Approve, Reject, Suspend)، `ICurrentVendorService`، Policy `ApprovedVendor`، Dashboard بـ Aggregates (`SUM(LineTotal)`, `SUM(CommissionAmount)`) مفلترة بالـ Vendor. |
| **Frontend Tasks** | فورم Vendor Application، صفحة حالة الطلب (Pending/Rejected + السبب)، Vendor Dashboard (Cards + جدول آخر الطلبات)، تعديل Store Profile. |
| **Authorization Rules** | Vendor Area: Policy `ApprovedVendor`. Admin Area: `SuperAdminOnly`. |
| **Validation** | StoreName فريد، سبب الرفض إجباري، الانتقالات: `Pending→Approved/Rejected`، `Approved↔Suspended`. |
| **Error Scenarios** | تقديم مرتين، Approve لطلب اتراجع، Vendor Suspended بيحاول الدخول. |
| **Testing Requirements** | Authorization Tests (Vendor A/B + غير معتمد)، Unit للـ State Transitions. |
| **Definition of Done** | الدورة الكاملة (Apply → Approve → Dashboard) شغالة، والعزل مختبر. |

## 7.4 D. Product Management (Create, Edit, Images, Categories, Price/Stock Validation, Active/Inactive)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | الـ Vendor يدير كتالوج منتجاته. |
| **User Stories** | US-14, US-15. |
| **Dependencies** | Vendor Approval + Categories. |
| **Database Changes** | `Products`, `ProductImages` + Check Constraints على `Price` و`StockQuantity`. |
| **Backend Tasks** | `ProductService` (Create/Update/Deactivate/Adjust Stock) بتفلتر دايماً بـ `VendorProfileId`. `FileStorageService` يحفظ الصور بأسماء GUID، ويتأكد من الامتداد (jpg/png/webp)، والحجم، والـ Content Type. |
| **Frontend Tasks** | Index/Create/Edit بـ ViewModels، رفع صور متعددة مع معاينة، زرار Activate/Deactivate. |
| **Authorization Rules** | `ApprovedVendor` + ملكية المنتج في كل Query. |
| **Validation** | `Price > 0`، `Stock ≥ 0`، Category نشطة، حجم الصورة (مثلاً ≤ 2MB)، عدد الصور (≤ 5). |
| **Error Scenarios** | منتج Vendor تاني (404)، ملف غير صورة، Overposting، تعارض Concurrency على `RowVersion`. |
| **Testing Requirements** | Unit للـ Validation، Integration للـ Create/Edit، Authorization (A يعدّل منتج B). |
| **Definition of Done** | CRUD كامل ومحمي، الصور آمنة، المنتجات المرتبطة بطلبات بتتعطّل مش بتتحذف. |

## 7.5 E. Shopping Cart (Add, Remove, Update, Stock Validation, Server-Side Price, Unavailable Items)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | تجميع المشتريات من كذا Vendor في مكان واحد. |
| **User Stories** | US-06, US-07. |
| **Dependencies** | Products + Authentication. |
| **Database Changes** | `CartItems` + Unique `(CustomerId, ProductId)`. |
| **Backend Tasks** | `CartService` (Add/Update/Remove/Get/Clear)، `GetCartAsync` يعيد حساب الأسعار من `Products` ويعلّم العناصر غير المتاحة (`IsAvailable=false`) مع رسالة. |
| **Frontend Tasks** | صفحة Cart بجدول، تعديل كمية (Input + زرار)، عداد في الـ Navbar، تحذير للعناصر غير المتاحة. |
| **Authorization Rules** | `[Authorize(Roles=Customer)]` والـ Cart الخاص بالمستخدم الحالي فقط (الـ `CustomerId` من الـ Claims). |
| **Validation** | `Quantity` من 1 للمخزون، `productId` موجود. |
| **Error Scenarios** | منتج اتعطّل بعد الإضافة، المخزون قلّ، سعر اتغير، منتج اتحذف. |
| **Testing Requirements** | Unit: إضافة، تكرار، تخطي المخزون، تلاعب بالسعر (مفيش سعر في الـ Request). |
| **Definition of Done** | الحسابات من الـ Database دايماً، والحالات الشاذة متعاملة. |

## 7.6 F. Orders and Checkout (Validation, Order Creation, OrderItems, Statuses, History, Vendor Views, Cancellation)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | تحويل الـ Cart لطلب مع حجز المخزون. |
| **User Stories** | US-08, US-10, US-11, US-16. |
| **Dependencies** | Cart + Products + Payment (الفصل 8). |
| **Database Changes** | `Orders`, `OrderItems`, `Payments` + Check Constraint على `LineTotal`. |
| **Backend Tasks** | `CheckoutService.CreateOrderAndSessionAsync` داخل Transaction: التحقق، إنشاء Order/Items بـ Snapshots، `ExecuteUpdateAsync` شرطي لخصم المخزون (`WHERE StockQuantity >= qty`)، ومسح الـ Cart بعد نجاح الدفع. `OrderService` لتاريخ العميل ولشاشة الـ Vendor ولتحديث الـ Fulfillment. |
| **Frontend Tasks** | صفحة Shipping، مراجعة الطلب، تاريخ الطلبات، تفاصيل طلب مجمّعة بالـ Vendor، شاشة Vendor Orders بأزرار Shipped/Delivered. |
| **Authorization Rules** | العميل يشوف طلباته فقط (`CustomerId == currentUser`)، الـ Vendor يشوف `OrderItems` بتاعته فقط. |
| **Validation** | عنوان الشحن إجباري، الانتقالات المسموحة (BR-10)، الإلغاء فقط لـ `PendingPayment`. |
| **Error Scenarios** | مخزون ناقص أثناء الحجز (Rollback)، Order ID لشخص تاني (404)، تحويل حالة غير مسموح. |
| **Testing Requirements** | Integration للـ Transaction (فشل جزئي)، Order Status Transitions، سباق شراء آخر قطعة. |
| **Definition of Done** | مفيش Overselling، الطلب مقسم بالـ Vendors، الحالات مضبوطة. |

## 7.7 G. Payment Processing (Stripe Test Mode, Webhooks, Idempotency, Failed Payments)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | استقبال المدفوعات بأمان وتأكيد الطلب. |
| **User Stories** | US-09. |
| **Dependencies** | Orders + Stripe Account (Test Mode) + Stripe CLI. |
| **Database Changes** | `Payments`, `StripeWebhookEvent`. |
| **Backend Tasks** | `StripePaymentService` (إنشاء Checkout Session)، `StripeWebhookController` (قراءة الـ raw body)، `StripeWebhookService` (Signature + Idempotency + Transaction)، معالجة الأحداث الأربعة (completed, async succeeded, async failed, expired). |
| **Frontend Tasks** | صفحتا Success وCancel (بيعرضوا الحالة فقط)، زرار Retry Payment لـ Order `PendingPayment` لسه صالح. |
| **Authorization Rules** | الـ Webhook Endpoint عام (`[AllowAnonymous]`) ومحميه بالتوقيع فقط، وبدون AntiForgery (`[IgnoreAntiforgeryToken]`). |
| **Validation** | التوقيع إجباري، `payment_status == paid`، مطابقة `amount_total` و`currency` مع الـ Order. |
| **Error Scenarios** | توقيع خاطئ (400)، Event مكرر (200)، Event متأخر بعد انتهاء الـ Session، Stripe بطيء. |
| **Testing Requirements** | Duplicate Webhook، Failed Payment، Signature غلط، Stripe CLI (`stripe listen`, `stripe trigger`). |
| **Definition of Done** | الدفع ينجح فقط من الـ Webhook، التكرار مبيكررش آثار، المفاتيح خارج الـ Repo. |

## 7.8 H. Commission System (Settings, Calculation, Net Amount, Records, Snapshots, Reporting)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | حساب عمولة المنصة وتتبعها. |
| **User Stories** | US-17, US-19. |
| **Dependencies** | Payment Webhook (لإنشاء السجلات). |
| **Database Changes** | `CommissionSettings`, `CommissionRecords` + Filtered Unique على الصف النشط. |
| **Backend Tasks** | `CommissionService.CreateForOrderAsync` داخل Transaction الـ Webhook: لكل Item يحسب `Commission = Round(LineTotal × Rate / 100, 2, AwayFromZero)` و`Net = Gross − Commission`. `SetRateAsync` للـ Admin: يطفّي الصف القديم ويضيف جديد في Transaction. استعلامات تقارير مجمّعة. |
| **Frontend Tasks** | فورم تغيير النسبة مع عرض السجل التاريخي، جدول عمولات للـ Vendor، ملخص للـ Admin. |
| **Authorization Rules** | تغيير النسبة: `SuperAdminOnly`. عرض العمولات: الـ Vendor يشوف بتاعته فقط. |
| **Validation** | النسبة 0–100، منع تغييرها بأثر رجعي، Unique على `OrderItemId`. |
| **Error Scenarios** | عدم وجود Setting نشط (يفشل الدفع ويُسجّل Log ويظهر Alert للـ Admin)، تقريب خاطئ. |
| **Testing Requirements** | Unit: حسابات بأرقام محددة، تغيير النسبة لا يغيّر القديم. |
| **Definition of Done** | الحسابات صحيحة ومسجّلة كـ Snapshot والتقارير مطابقة للمجاميع. |

## 7.9 I. Super Admin Dashboard (Approvals, Categories, Orders, Commission, Analytics, Moderation)

| البند | التفاصيل |
|---|---|
| **Purpose (الغرض)** | إدارة المنصة بالكامل. |
| **User Stories** | US-04, US-18, US-19, US-20. |
| **Dependencies** | كل الـ Features السابقة. |
| **Database Changes** | لا جداول جديدة (Aggregates). |
| **Backend Tasks** | `AdminDashboardService` (عدد Vendors/Orders/إيرادات/عمولات)، CRUD للـ Categories، قائمة Orders بفلترة وPagination، Deactivate لمنتج مخالف. |
| **Frontend Tasks** | Dashboard بـ Cards، جداول بـ Pagination، فورم Categories وCommission. |
| **Authorization Rules** | `SuperAdminOnly` على الـ Area كلها. |
| **Validation** | اسم Category فريد، منع تعطيل Category فيها منتجات نشطة بدون تأكيد. |
| **Error Scenarios** | محاولة وصول Customer/Vendor (403)، تعديل Category مكرر. |
| **Testing Requirements** | Authorization على كل Action، Integration للأرقام. |
| **Definition of Done** | الأرقام صحيحة، كل الشاشات محمية. |

---pagebreak---
