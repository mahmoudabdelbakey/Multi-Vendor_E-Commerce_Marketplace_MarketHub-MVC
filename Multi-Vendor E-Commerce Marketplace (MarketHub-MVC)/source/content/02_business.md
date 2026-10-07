# 2. التحليل التجاري ومتطلبات النظام (Business Analysis)

الـ Business Analysis هو المرحلة اللي بنجاوب فيها: **إحنا بنبني إيه؟ ولمين؟ وليه؟** قبل ما نكتب سطر Code واحد. لو اتخطينا المرحلة دي هنكتشف في نص المشروع إن كل واحد فينا فاهم المشروع بشكل مختلف. كل الفريق (الخمسة) لازم يحضر جلسات التحليل دي مع بعض.

## 2.1 Executive Summary

MarketHub-MVC منصة ويب بتجمع أكتر من بائع (Vendor) في مكان واحد. العميل بيشتري من كذا Vendor في عملية دفع واحدة، والمنصة بتسجّل عمولتها على كل عنصر في الطلب، والـ Vendor بيشوف الطلبات الخاصة بيه بس. الـ MVP بيركز على تجربة الشراء الكاملة (من التصفح لحد الدفع في Stripe Test Mode) ولوحات تحكم بسيطة للـ Vendor والـ Admin.

## 2.2 Problem Statement

البائعين الصغار مش دايماً عندهم موقع خاص بيهم، وبناء متجر مستقل مكلف ومحتاج مهارات. والعملاء بيفضلوا يلاقوا كذا بائع في مكان واحد بدل ما يدخلوا على مواقع كتير. وعلى مستوى المطور، مفيش مشروع تعليمي أفضل من الـ Marketplace لأنه بيجمع Authentication, Authorization, Relational Design, Transactions, Payments.

## 2.3 Proposed Solution

منصة `ASP.NET Core MVC` فيها ثلاث بوابات: Storefront عام، Vendor Area، و Admin Area. كل الصلاحيات بتتفرض على الـ Server، وكل الحسابات المالية بتتحسب على الـ Server من الـ Database، ومش من اللي جاي من الـ Browser.

## 2.4 Project Objectives

| # | الهدف | مقياس النجاح |
|---|---|---|
| O1 | تجربة شراء كاملة Multi-Vendor | عميل يشتري منتجين من Vendor مختلفين في Checkout واحد ويدفع بـ Stripe Test Mode. |
| O2 | عزل بيانات الـ Vendors | مفيش Test Case في Authorization بيسمح لـ Vendor A بالوصول لبيانات Vendor B. |
| O3 | دفع آمن وموثوق | الـ Order بيتحوّل لـ Paid من الـ Webhook الموقّع فقط، ومفيش تكرار لو الـ Webhook اتبعت مرتين. |
| O4 | حساب عمولة صحيح وقابل للتتبع | كل `OrderItem` مدفوع له `CommissionRecord` فيه نسبة العمولة وقت الشراء. |
| O5 | تعلّم جماعي | كل عضو شارك في كل Sprint وعمل Code Review وفهم الـ Flow كامل. |

## 2.5 Target Users

- **Guest/Customer:** مشتري عادي بيدور على منتجات ويحب الدفع بسرعة.
- **Vendor:** بائع صغير محتاج يعرض منتجاته ويتابع طلباته ومبيعاته.
- **Super Admin:** مدير المنصة المسؤول عن الجودة والعمولات.

## 2.6 Business Value

المنصة بتكسب من العمولة على كل عملية بيع (Commission)، وبتوفر للـ Vendor قاعدة عملاء جاهزة. بالنسبة لينا كفريق، القيمة هي مشروع Portfolio حقيقي بيثبت إننا نعرف نبني Full-Stack .NET.

## 2.7 Project Scope و MVP Scope

**داخل الـ MVP (Must Have):** Registration/Login، Vendor Application + Approval، Categories، Product CRUD + صور، Search/Filter/Sort/Pagination، Shopping Cart، Checkout، Order + OrderItems، Stripe Checkout (Test Mode) + Webhook، Commission Records، Vendor Dashboard، Admin Dashboard.

**Should Have (لو الوقت سمح):** Reviews، Customer Cancel Order، Product Moderation، Basic Analytics Charts.

## 2.8 Out-of-Scope و Future Enhancements

| Out-of-Scope (MVP) | Future Enhancement |
|---|---|
| تحويل فلوس حقيقية للـ Vendors | Stripe Connect (Connected Accounts + Transfers) |
| Shipping Fees وحساب التوصيل | Shipping Providers API |
| Refunds تلقائية | Refund Workflow + `CommissionRecord.Status = Reversed` |
| Coupons وخصومات | Promotions Engine |
| Product Variants (مقاسات/ألوان) | `ProductVariant` entity |
| Email Notifications | SMTP/SendGrid + Background Jobs |
| Multi-Currency | Currency conversion service |
| Mobile App / API | Web API + JWT |

## 2.9 Functional Requirements (FR)

| ID | المتطلب | الأولوية |
|---|---|---|
| FR-01 | الـ Guest يقدر يتصفح المنتجات ويبحث ويفلتر ويرتب مع Pagination. | Must |
| FR-02 | المستخدم يقدر يسجّل حساب Customer ويدخل ويخرج. | Must |
| FR-03 | المستخدم يقدر يقدّم Vendor Application بـ Store Name فريد. | Must |
| FR-04 | الـ Super Admin يوافق أو يرفض الـ Vendor Application مع سبب للرفض. | Must |
| FR-05 | الـ Vendor المعتمد يدير Product CRUD وصور وActive/Inactive وStock. | Must |
| FR-06 | الـ Customer يدير الـ Cart (إضافة، حذف، تعديل كمية). | Must |
| FR-07 | الـ Checkout يتحقق من الأسعار والمخزون من الـ Database وينشئ Order. | Must |
| FR-08 | الدفع بـ Stripe Checkout ومعالجة الـ Webhook بشكل Idempotent. | Must |
| FR-09 | حساب `CommissionRecord` لكل OrderItem بعد نجاح الدفع. | Must |
| FR-10 | الـ Vendor يشوف OrderItems بتاعته فقط ويحدّث Fulfillment Status. | Must |
| FR-11 | الـ Customer يشوف تاريخ طلباته وحالاتها. | Must |
| FR-12 | الـ Admin يدير Categories ويضبط نسبة العمولة ويراقب الـ Orders. | Must |
| FR-13 | Vendor Dashboard: ملخص مبيعات وعمولات. | Must |
| FR-14 | Admin Dashboard: تقارير عامة للمنصة. | Should |
| FR-15 | الـ Customer يكتب Review على منتج استلمه (Delivered). | Should |
| FR-16 | الـ Customer يلغي Order لسه `PendingPayment`. | Should |
| FR-17 | الـ Admin يوقف (Suspend) Vendor أو Product مخالف. | Should |

## 2.10 Non-Functional Requirements (NFR)

| ID | الفئة | المتطلب |
|---|---|---|
| NFR-01 | Security | كل الـ Authorization بيتفرض على الـ Server، وكل الـ POST محمي بـ AntiForgery. |
| NFR-02 | Security | الـ Secrets (Stripe keys, connection strings) مبتتحطش في GitHub أبداً. |
| NFR-03 | Reliability | معالجة الـ Webhook والـ Inventory داخل Database Transaction. |
| NFR-04 | Performance | صفحة Product Listing بترجع بـ Pagination (12-24 منتج) وبـ Index على الأعمدة المستخدمة في الفلترة. |
| NFR-05 | Usability | الموقع Responsive على الموبايل (Bootstrap Grid). |
| NFR-06 | Maintainability | كل Feature ليها Service واضحة وTests أساسية. |
| NFR-07 | Auditability | الأسعار والعمولات بتتسجّل كـ Snapshot وقت الشراء. |
| NFR-08 | Compatibility | بيشتغل على آخر نسخة من Chrome و Edge و Firefox. |

## 2.11 Business Rules (BR)

| ID | القاعدة |
|---|---|
| BR-01 | الـ Vendor لازم يكون `Approved` علشان يضيف منتجات أو تظهر منتجاته للعملاء. |
| BR-02 | الـ Vendor يشوف ويعدّل بياناته هو فقط (Products, OrderItems, Commissions). |
| BR-03 | السعر الأساسي هو اللي في `Products.Price` وقت الـ Checkout، ومبنثقش في أي سعر جاي من الـ Browser. |
| BR-04 | الكمية في الـ Cart والـ Order لازم تكون من 1 لحد المخزون المتاح. |
| BR-05 | المخزون بيتحجز (Reserve) لحظة إنشاء الـ Order ويتحرر لو الـ Order اتلغى أو الـ Session انتهت. |
| BR-06 | الـ Order بيبقى `Paid` فقط بعد Webhook موقّع، ومش بعد الـ Redirect. |
| BR-07 | العمولة = `LineTotal × RatePercent / 100` بتقريب لأقرب سنت، ونسبة العمولة بتتسجّل في `CommissionRecord` وقت الدفع. |
| BR-08 | `VendorNetAmount = GrossAmount - CommissionAmount`. |
| BR-09 | لا يمكن حذف Product له OrderItems؛ بنعمل Deactivate بدل الحذف. |
| BR-10 | الـ Fulfillment Status مسموح بس بالترتيب `Pending → Shipped → Delivered`، وبعد ما الـ Order يبقى `Paid`. |
| BR-11 | الـ Order بيبقى `Completed` لما كل الـ OrderItems غير الملغية تبقى `Delivered`. |
| BR-12 | Review مسموح بس لـ Customer اشترى المنتج واستلمه، وبواقع Review واحد لكل (Customer, Product). |

## 2.12 Constraints و Risks

**Constraints:** مدة المشروع محدودة، الفريق مبتدئ، Stripe في Test Mode فقط، ومفيش Budget للاستضافة.

| الخطر (Risk) | الاحتمال | التأثير | التخفيف (Mitigation) |
|---|---|---|---|
| تعارض الـ Migrations بين الأعضاء | عالي | متوسط | قواعد Migration في الفصل 11 (عضو واحد في كل مرة + Rebase). |
| Overselling (بيع مخزون أكتر من الموجود) | متوسط | عالي | Reserve بـ Conditional UPDATE داخل Transaction + `RowVersion`. |
| Webhook مكرر أو متأخر | عالي | عالي | جدول `StripeWebhookEvent` بـ Unique على `StripeEventId`. |
| تسريب Secrets | متوسط | عالي | `dotnet user-secrets` + `.gitignore` + Secret Scanning. |
| Scope Creep | عالي | متوسط | قائمة Out-of-Scope + مراجعة Backlog في كل Sprint Planning. |
| عضو مش فاهم جزء من النظام | متوسط | عالي | Pair Programming + Rotating Leadership + Knowledge Sessions. |

## 2.13 User Stories مع Acceptance Criteria

### US-06: Add products from different vendors to cart

> **As a** Customer, **I want to** add products from different vendors to my cart **so that** I can purchase them through the marketplace.

**الشرح بالعربي:** العميل لازم يقدر يحط منتجات من أكتر من متجر في نفس الـ Cart. بنخزّن في `CartItems` مين العميل وأنهي منتج وكام قطعة، لكن **مش بنخزّن السعر**، لأن السعر بيتجاب من `Products` كل مرة.

| العنصر | التفاصيل |
|---|---|
| Acceptance Criteria | (1) المنتج لازم يكون `IsActive` ومتجره `Approved`. (2) الكمية الكلية للمنتج في الـ Cart ≤ `StockQuantity`. (3) لو المنتج موجود بالفعل، الكمية تزيد ومفيش صف مكرر. (4) الـ Cart بيعرض السعر الحالي من الـ Database. |
| Main Flow | Customer يضغط Add to Cart ← `CartController.Add` ← `CartService.AddAsync` يحمّل المنتج من الـ DB ويتحقق ← Insert/Update في `CartItems` ← Redirect لصفحة الـ Cart. |
| Alternative Flows | Guest ← Redirect لـ Login ثم الرجوع. المنتج موجود ← زيادة الكمية. |
| Error Scenarios | منتج غير موجود (404)، Inactive، متجره Suspended، كمية صفر أو سالبة، تخطي المخزون. |
| Entities | `CartItem`, `Product`, `VendorProfile`, `ApplicationUser` |
| Components | `CartController`, `ICartService`, `ApplicationDbContext`, `CartViewModel` |

### US-09: Pay for my order using Stripe

> **As a** Customer, **I want to** pay for my order securely online **so that** my purchase is confirmed without sharing my card with the marketplace.

**الشرح:** احنا مبنستقبلش بيانات الكارت. العميل بيتحوّل لصفحة Stripe المستضافة، وبعد الدفع Stripe هو اللي بيبلغنا بالـ Webhook.

| العنصر | التفاصيل |
|---|---|
| Acceptance Criteria | (1) Order بيتعمل `PendingPayment` قبل التحويل. (2) بعد الدفع الناجح والـ Webhook يتحول لـ `Paid`. (3) الـ Redirect لوحده لا يغيّر الحالة. (4) لو الـ Webhook اتكرر مفيش تغيير تاني. |
| Main Flow | Checkout ← Create Order + Reserve Stock ← Create Stripe Session ← Redirect ← Payment ← Webhook ← Order Paid + CommissionRecords. |
| Alternative Flows | العميل يقفل صفحة Stripe: الـ Session بتنتهي وبيجي `checkout.session.expired` ← الـ Order بيتلغى والمخزون يتحرر. |
| Error Scenarios | كارت مرفوض (العميل يحاول تاني داخل نفس الـ Session)، Webhook توقيعه غلط (400)، Stripe مش متاح وقت إنشاء الـ Session (Rollback أو إلغاء الـ Order). |
| Entities | `Order`, `OrderItem`, `Payment`, `StripeWebhookEvent`, `CommissionRecord` |
| Components | `CheckoutController`, `ICheckoutService`, `IPaymentService`, `StripeWebhookController`, `IStripeWebhookService` |

### US-14: Vendor creates a product

> **As a** Vendor, **I want to** create and manage my own products **so that** I can sell them in the marketplace.

| العنصر | التفاصيل |
|---|---|
| Acceptance Criteria | (1) Approved Vendor فقط. (2) `Price > 0` و `StockQuantity ≥ 0`. (3) Category موجودة ونشطة. (4) الـ `VendorProfileId` بيتحدد من المستخدم الحالي مش من الفورم. (5) الصور: امتدادات مسموحة وحجم محدود. |
| Main Flow | Vendor يفتح Create ← يملأ `ProductViewModel` ← POST ← Validation ← الـ Service يحفظ المنتج والصور ← Redirect لـ My Products. |
| Alternative Flows | حفظ كمسودة (`IsActive = false`). |
| Error Scenarios | Vendor غير معتمد (403)، سعر سالب، ملف صورة خطر، محاولة Overposting على `VendorProfileId`. |
| Entities | `Product`, `ProductImage`, `Category`, `VendorProfile` |
| Components | `Areas/Vendor/ProductsController`, `IProductService`, `ICurrentVendorService`, `ProductViewModel` |

### US-04: Super Admin approves vendor

> **As a** Super Admin, **I want to** review vendor applications **so that** only legitimate stores can sell on the platform.

| العنصر | التفاصيل |
|---|---|
| Acceptance Criteria | (1) قائمة Pending. (2) عند الموافقة: `Status = Approved` + إضافة Role `Vendor` + تحديث `SecurityStamp`. (3) عند الرفض: `RejectionReason` إجباري. (4) تسجيل `ReviewedByUserId` و`ReviewedAt`. |
| Main Flow | Admin يفتح Pending ← يراجع ← Approve ← `VendorService.ApproveAsync` ← Role + Status. |
| Error Scenarios | تطبيق اتراجع قبل كده، المستخدم مش Admin، Vendor حذف حسابه. |
| Entities | `VendorProfile`, `ApplicationUser` (+ Identity Roles) |
| Components | `Areas/Admin/VendorApplicationsController`, `IVendorService`, `UserManager` |

### قائمة باقي الـ User Stories

| ID | As a | I want to | So that |
|---|---|---|---|
| US-01 | Guest | browse and search products | I can find what I want before registering. |
| US-02 | Guest | register and log in | I can buy products. |
| US-03 | Guest | apply as a vendor | I can open my own store. |
| US-05 | Customer | view product details with images | I can decide to buy. |
| US-07 | Customer | update quantities and remove cart items | my cart matches what I want. |
| US-08 | Customer | enter shipping details at checkout | the order can be delivered. |
| US-10 | Customer | view my order history with statuses per vendor | I can track my purchases. |
| US-11 | Customer | cancel an unpaid order | stock is released if I change my mind. |
| US-12 | Customer | review a product I received | other customers get honest feedback. |
| US-13 | Vendor | manage my store profile | customers recognize my brand. |
| US-15 | Vendor | adjust inventory | the stock shown is accurate. |
| US-16 | Vendor | see only my order items and mark them shipped/delivered | I can fulfill orders. |
| US-17 | Vendor | see my sales and commission summary | I understand my earnings. |
| US-18 | Super Admin | manage categories | products are organized. |
| US-19 | Super Admin | configure the commission rate | the platform earns revenue. |
| US-20 | Super Admin | monitor all orders and see platform reports | I can supervise the marketplace. |

## 2.14 Use Cases

| UC | الاسم | Actor | الوصف المختصر |
|---|---|---|---|
| UC-01..03 | Browse / Search / View Details | Guest | تصفح المنتجات النشطة فقط من متاجر Approved. |
| UC-04 | Register Account | Guest | إنشاء حساب Customer. |
| UC-05 | Login / Logout | Guest, Customer, Vendor, Admin | Authentication. |
| UC-06 | Apply as Vendor | Guest | حساب + VendorProfile بحالة Pending. |
| UC-07 | Manage Cart | Customer | إضافة/حذف/تعديل. |
| UC-08 | Checkout | Customer | التحقق + إنشاء Order + حجز المخزون. |
| UC-09 | Pay with Stripe | Customer, Stripe | دفع Hosted Checkout. |
| UC-10 | View Order History | Customer | طلباتي فقط. |
| UC-11 | Cancel Pending Order | Customer | يحرر المخزون. |
| UC-12 | Write Review | Customer | بعد التسليم. |
| UC-13 | Manage Profile | Customer | تعديل بيانات الحساب. |
| UC-14..22 | Vendor operations | Vendor | Store, Products, Images, Inventory, Order Items, Fulfillment, Sales, Commission, Dashboard. |
| UC-23..29 | Admin operations | Super Admin | Applications, Suspend, Categories, Commission, Orders, Reports, Moderation. |
| UC-31 | Process Stripe Webhook | Stripe | Signature + Idempotency + Update Order/Payment. |

## 2.15 Definition of Done (على مستوى الـ Feature)

الـ Feature مش بتتحسب خلصانة إلا لو:

1. كل Acceptance Criteria اتحققت واتجربت يدوياً.
2. فيه Unit/Integration Tests للـ Service Logic والـ Authorization.
3. الـ Validation شغالة على الـ Server (وعلى الـ Client كتحسين فقط).
4. الـ Pull Request اتراجع من عضو غير الكاتب واتعمله Approve والـ CI أخضر.
5. مفيش Secrets ولا Warnings جديدة.
6. اتحدّثت الـ Documentation (README / الـ Diagrams لو التصميم اتغير).
7. اتعمل Demo في الـ Sprint Review وكل الفريق فاهم الـ Flow.

---pagebreak---
