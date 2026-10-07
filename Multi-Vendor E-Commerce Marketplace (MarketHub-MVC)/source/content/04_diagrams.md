# 4. الرسومات التوضيحية (Visual Diagrams)

كل Diagram في الفصل ده مرسوم فعلياً كصورة PNG/SVG، ومصدره القابل للتعديل موجود في فولدر `diagrams/src` (ملفات `.mmd` لـ Mermaid و `.dot` لـ Graphviz). **لو غيّرتوا التصميم، عدّلوا الـ Source وأعيدوا الـ Render** بالأمر `./diagrams/render.sh` علشان الرسومات والـ Code يفضلوا متطابقين.

> قاعدة الفريق: أي Pull Request بيغيّر الـ ERD أو الـ Flow الأساسي لازم يحدّث الـ Diagram المرتبط بيه في نفس الـ PR.

@@ 01_system_context | Diagram 1 | System Context Diagram
**الفكرة:** ده أكبر صورة للنظام: مين بيستخدمه ومين بيتواصل معاه من بره. `MarketHub-MVC` في النص، حواليه الأربع Actors (Guest, Customer, Vendor, Super Admin)، وعلى الجانب الأنظمة الخارجية: `SQL Server` لتخزين البيانات، و`Stripe` لاستقبال المدفوعات، والـ File Storage للصور.

**ليه محتاجينه؟** علشان نحدد حدود المشروع (System Boundary). Stripe بيبعتلنا Webhook Events، يعني فيه سهم داخل من Stripe للتطبيق، ولازم يكون عندنا Endpoint عام (Public HTTPS) يستقبله. ده بيأثر على الـ Deployment.

@@ 02_usecase_overall | Diagram 2 | Overall UML Use Case Diagram
**الفكرة:** بيعرض كل الـ Use Cases في صورة واحدة مقسمة لـ Packages حسب الـ Actor. الـ `Customer` بيرث كل اللي بيعمله الـ `Guest` (سهم الـ inherits). Stripe كـ Actor خارجي مرتبط بـ UC-31 (Process Stripe Webhook).

**ازاي نقراه؟** كل Actor متوصل بالـ Package بتاعه، يعني مسموح له ينفذ أي Use Case جواه. التفاصيل الدقيقة (include/extend) موجودة في الرسومات التلاتة بعده.

@@ 03_usecase_customer | Diagram 3 | Customer Use Case Diagram
**الشرح:** الـ Customer بيتصفح ويضيف للـ Cart ويعمل Checkout. العلاقة `«include»` بين UC-08 (Checkout) و UC-09 (Pay with Stripe) معناها إن الدفع جزء إجباري من الـ Checkout. وبرضه UC-09 بيعتمد على UC-31 (الـ Webhook) علشان التأكيد النهائي. UC-12 (Write Review) له شرط مسبق: لازم يكون فيه منتج اتسلّم.

@@ 04_usecase_vendor | Diagram 4 | Vendor Use Case Diagram
**الشرح:** كل الـ Use Cases دي مشروطة بإن الـ Vendor يكون `Approved`. UC-15 (Manage Products) بيشمل رفع الصور (UC-16) وإدارة المخزون (UC-17). UC-19 (Update Fulfillment Status) بيمتد من UC-18 (View My Order Items) لأن الـ Vendor مش هيعدّل حالة قبل ما يشوف الـ Item.

@@ 05_usecase_admin | Diagram 5 | Super Admin Use Case Diagram
**الشرح:** الـ Super Admin مسؤول عن الحوكمة: الموافقة على الـ Vendors (UC-23) وإيقافهم (UC-24)، الـ Categories (UC-25)، نسبة العمولة (UC-26)، ومراقبة الطلبات والتقارير (UC-27, UC-28). تعديل UC-26 بيضيف صف جديد في `CommissionSettings` ومبيغيّرش العمولات القديمة.

@@ 06a_erd_overview | Diagram 6A | ERD Overview (Relationships Only)
**الشرح:** صورة مبسّطة للعلاقات بين الجداول من غير الأعمدة. القراءة: `ApplicationUser ||--o| VendorProfile` معناها إن المستخدم الواحد عنده صفر أو VendorProfile واحد. `Order ||--|{ OrderItem` معناها إن كل Order لازم يكون فيه OrderItem واحد على الأقل. `OrderItem ||--o| CommissionRecord` معناها إن كل OrderItem مدفوع له CommissionRecord واحد بالظبط. جدول `StripeWebhookEvent` مستقل علشان بيستخدم فقط لضمان الـ Idempotency.

@@ 06b_erd_catalog | Diagram 6B | ERD: Identity, Vendors and Catalog
**الشرح:** الجزء الخاص بالمستخدمين والمنتجات. `Product.VendorProfileId` هو اللي بيربط كل منتج بمتجره، وبيعتمد عليه عزل البيانات. `CartItem` فيه Unique Index على `(CustomerId, ProductId)`. `Product.RowVersion` عمود `rowversion` بيستخدم كـ Concurrency Token. تفاصيل الأنواع والقيود في الـ Data Dictionary (الفصل 5).

@@ 06c_erd_orders_payments | Diagram 6C | ERD: Orders, Payments and Commission
**الشرح:** الجزء المالي. `OrderItem` بيحتفظ بـ `ProductNameSnapshot` و`UnitPrice` و`VendorProfileId` وقت الشراء علشان تغيير سعر المنتج بعدين مايغيّرش تاريخ الطلبات. `Payment` فيه `StripeCheckoutSessionId` بـ Unique Index. `CommissionRecord` بيخزّن `RatePercentSnapshot` علشان لو الـ Admin غيّر النسبة، السجلات القديمة تفضل زي ما هي.

@@ 07_architecture | Diagram 7 | High-Level System Architecture
**الشرح:** الـ Request بيدخل من الـ Browser على الـ Middleware Pipeline (HTTPS، Static Files، Routing، Authentication، Authorization)، بعدين الـ Controller (أو Area Controller)، اللي بينادي على Service. الـ Service بتتعامل مع `ApplicationDbContext` اللي بيتكلم مع SQL Server، ومع Stripe عند الدفع. Webhook من Stripe بيدخل على `WebhookController` مباشرة. التقسيم ده مطابق تماماً لهيكل الفولدرات في الفصل 6.

@@ 08_components | Diagram 8 | Application Component Diagram
**الشرح:** بيوضّح مين بيعتمد على مين. الـ Presentation بتعتمد على الـ Services، والـ Services بتعتمد على الـ Data Access، ومفيش عكس ده (الـ Service مبتعرفش حاجة عن الـ Controller). مجموعة `Money and vendor services` هي اللي بتتعامل مع Stripe SDK. الـ Cross-Cutting (Identity, Authorization, Logging, Exception Handler) بتخدم الطبقات كلها.

@@ 09_class_diagram | Diagram 9 | UML Class Diagram
**الشرح:** الـ Entities والـ Enums الأساسية مع علاقاتها: `Order` بيحتوي (Composition) على `OrderItem`، و`Product` متربط بـ `VendorProfile` و`Category`. الـ Enums (`VendorStatus`, `OrderStatus`, `PaymentStatus`, `FulfillmentStatus`) بتتخزن في الـ Database كـ string باستخدام `HasConversion<string>()` علشان تكون مقروءة في الـ SQL. ظهرت كمان Interfaces أساسية (`ICheckoutService`, `IStripeWebhookService`, `ICommissionService`) علشان تفهم مين بيخلق إيه.

@@ 10_deployment | Diagram 10 | Deployment Architecture
**الشرح:** التمييز بين تلات بيئات: (1) Local Development: SQL Server محلي، `user-secrets`، و Stripe CLI بيعمل `listen --forward-to` لتجربة الـ Webhooks. (2) GitHub: الـ Repository وGitHub Actions للـ Build والـ Tests. (3) Production/Demo: Web Host + SQL Server مستضاف + Stripe Test Mode Webhook Endpoint. **اختيار مزوّد الاستضافة قرار الفريق [Team Decision]**، والرسم مكتوب بشكل عام عمداً.

@@ 11_vendor_registration_activity | Diagram 11 | Vendor Registration and Approval (Activity)
**الشرح:** الألوان بتبيّن مين المسؤول: أصفر للمتقدّم، أزرق للـ System، أخضر للـ Admin. الـ System بيتحقق من صحة البيانات وتفرّد الـ Email والـ StoreName، بعدين بينشئ `ApplicationUser` **من غير Role Vendor** و`VendorProfile` بحالة `Pending`. الـ Role بتتضاف فقط عند الموافقة، ومعاها `UpdateSecurityStampAsync`. عند الرفض بنحفظ `RejectionReason` والمستخدم يفضل Customer عادي.

@@ 12_product_creation_activity | Diagram 12 | Product Creation (Activity)
**الشرح:** أول بوابة: هل المستخدم Approved Vendor؟ لو لأ، 403 أو Login. بعدها الفورم، ثم Validation على الـ Server (السعر أكبر من صفر، المخزون صفر أو أكتر، الـ Category موجودة). الصور بتتفحص (الامتداد، الحجم، نوع المحتوى). النقطة الأهم: **الـ Service هي اللي بتحدد `VendorProfileId` من المستخدم الحالي**، والـ Form مفيهوش الحقل ده.

@@ 13_cart_activity | Diagram 13 | Shopping Cart (Activity)
**الشرح:** الـ Guest بيتحوّل لـ Login. بعدين الـ `CartService` بيحمّل المنتج من الـ Database (السعر والمخزون الحاليين) ويتأكد إنه Active وإن الـ Vendor Approved وإن الكمية صحيحة. بنعمل Insert أو Update على `CartItem` (Unique على العميل والمنتج). صفحة الـ Cart بتعيد حساب المجاميع من أسعار الـ Database في كل مرة، علشان لو السعر اتغير العميل يشوف الجديد.

@@ 14_checkout_activity | Diagram 14 | Checkout (Activity)
**الشرح:** عند الضغط على Pay، بنعيد تحميل كل منتج من الـ Database ونتأكد تاني (Active, Approved, Stock). لو كله تمام نبدأ Transaction: نعمل `Order` بحالة `PendingPayment` و`OrderItems` بـ Price Snapshot، ونحجز المخزون بـ Conditional UPDATE، وننشئ `Payment` بحالة `Pending`. لو الحجز فشل لأي منتج نعمل Rollback. بعد الـ COMMIT بنكلّم Stripe لإنشاء الـ Checkout Session ونحول العميل لصفحة الدفع.

@@ 15_payment_sequence | Diagram 15 | Payment Processing (Sequence)
**الشرح خطوة بخطوة:** (1) العميل يضغط Pay (POST + AntiForgery). (2-4) الـ Service تحمّل الـ Cart وتعمل الـ Transaction. (6-7) إنشاء Checkout Session عند Stripe مع `metadata` فيها `OrderId` و`expires_at`. (8) حفظ `StripeCheckoutSessionId`. (10-12) العميل بيدفع بكارت تجريبي وبيرجع لـ `/Checkout/Success`. (14-15) صفحة النجاح **بتعرض الحالة فقط** وبتقول "الدفع قيد التأكيد". **الـ Redirect عمره ما بيغيّر الـ Order لـ Paid**: لأن العميل ممكن يقفل المتصفح أو يعدّل الـ URL. التأكيد الحقيقي من الـ Webhook (Diagram 17).

@@ 16_order_processing_sequence | Diagram 16 | Order Processing and Fulfillment (Sequence)
**الشرح:** بعد ما الـ Order يبقى `Paid`، الـ Vendor بيفتح `/Vendor/Orders` والـ Service بتجيب الـ OrderItems اللي `VendorProfileId` بتاعها = الـ Vendor الحالي بس. عند Mark Shipped بنحمّل الـ Item بشرط الملكية، ولو مش موجود بترجع NotFound (من غير ما نكشف وجوده). التحويلات المسموحة: `Pending → Shipped → Delivered`. لما كل الـ Items غير الملغية تبقى `Delivered` الـ Order بيتحول لـ `Completed`. العميل بيشوف طلبه مقسّم حسب الـ Vendor.

@@ 17_webhook_sequence | Diagram 17 | Stripe Webhook Processing (Sequence)
**الشرح:** ده أهم Diagram في المشروع. (1-2) Stripe بيبعت POST على `/stripe/webhook` بـ JSON خام وهيدر `Stripe-Signature`، والـ Controller بيقرأ الـ **raw body** (مش Model Binding). (3) التحقق من التوقيع بسر الـ Webhook. لو فاشل: 400. (4) بنبدأ Transaction وندخل صف في `StripeWebhookEvent` بـ Unique على `StripeEventId`. لو الصف موجود بالفعل: يبقى الـ Event اتعالج قبل كده، نرجّع 200 من غير أي تغيير. (5) لو `checkout.session.completed` و`payment_status = paid` (أو `async_payment_succeeded`): نحدّث `Payment = Succeeded` و`Order = Paid` وننشئ `CommissionRecords`. (6) لو `expired` أو `async_payment_failed`: `Order = Cancelled` ونرجّع المخزون المحجوز. (7) COMMIT ثم 200. لأن تسجيل الـ Event جوه نفس الـ Transaction، لو حصل فشل في النص كل حاجة بتتراجع والـ Stripe هيعيد المحاولة، ومفيش حالة نص-نص.

@@ 18_github_workflow | Diagram 18 | GitHub Collaboration Workflow
**الشرح:** كل شغل يبدأ من GitHub Issue، وبعدين Branch من `main`، وCommits صغيرة، ومزامنة مع `main` قبل الـ Push، وPull Request بقالب جاهز. الـ GitHub Actions بتبني وتشغّل الـ Tests. المراجعة بتتم من عضو غير الكاتب. الـ Merge بيكون Squash علشان تاريخ `main` يفضل نظيف. الفصل 11 فيه الأوامر العملية.

@@ 19_development_lifecycle | Diagram 19 | Development Lifecycle (Sprint Loop)
**الشرح:** الدورة بتكرر كل Sprint: Backlog، Planning مشترك، Design Session مشتركة، تنفيذ Vertical Slice كامل (Database + Service + Controller + View)، اختبار جماعي، Code Review بين الأعضاء، Integration مع `main`، Sprint Review (Demo)، ثم Retrospective. الـ Deployment للـ Demo بيبدأ من Sprint 9.

@@ 20_state_machines | Diagram 20A | Order Status State Machine
**الشرح:** `Order.Status` له أربع حالات: `PendingPayment` (بعد الـ Checkout)، `Paid` (بعد الـ Webhook)، `Completed` (كل الـ Items اتسلّمت)، `Cancelled` (انتهت الـ Session أو فشلت أو العميل ألغى). **مفيش انتقال من Paid إلى Cancelled في الـ MVP** (ده بيحتاج Refund وهو خارج الـ Scope).

@@ 20b_orderitem_payment_states | Diagram 20B | OrderItem Fulfillment State Machine
**الشرح:** الـ Vendor هو اللي بيحرّك `FulfillmentStatus` من `Pending` لـ `Shipped` لـ `Delivered`، وبس بعد ما الـ Order يبقى `Paid`. حالة `Cancelled` بتتحط تلقائياً لما الـ Order كله يتلغي.

@@ 21_request_lifecycle | Diagram 21 | Request Lifecycle (Browser to View)
**الشرح:** رحلة Request واحد: الـ Browser يبعت Request، الـ Middleware بيعمل HTTPS Redirect وStatic Files وRouting وAuthentication وAuthorization (لو فشل: 401 أو 403). بعدها Model Binding والـ Validation على الـ ViewModel، ثم الـ Controller بينادي الـ Service، والـ Service تستخدم EF Core اللي بيولّد SQL بـ Parameters وبيرجّع Entities، والـ Controller يمررها لـ View على شكل ViewModel والـ Razor بيطلّع HTML.

@@ 22_vendor_isolation | Diagram 22 | Vendor Isolation Authorization Flow
**الشرح:** مسار Request واحد على `POST /Vendor/Products/Edit/17`: التحقق من الـ Login، ثم Policy `ApprovedVendor`، ثم تحديد `VendorProfileId` من `ICurrentVendorService`، ثم Query بشرط `Id = 17 AND VendorProfileId = currentVendorId`. لو الصف مش موجود (سواء المنتج مش موجود أو بتاع Vendor تاني) الرد دايماً 404 بنفس الشكل، علشان المهاجم ميعرفش إن المنتج موجود.

---pagebreak---
