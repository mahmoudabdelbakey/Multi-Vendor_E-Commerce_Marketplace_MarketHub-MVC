# 9. خارطة الطريق التفصيلية (Development Roadmap)

## 9.1 ترتيب التنفيذ ولماذا نبني بشكل تدريجي (Incremental)

الخطأ الشائع في المشاريع الطلابية إن الفريق يبني كل الـ Database أولاً، وبعدين كل الـ Backend، وبعدين كل الـ Frontend، وفي الآخر يحاول يوصّلهم. النتيجة: التكامل بيتأجل لآخر المشروع وبتظهر مشاكل كبيرة متأخر، ومحدش بيشوف حاجة شغالة لأسابيع.

إحنا هنبني **Vertical Slices**: كل Sprint بيطلّع Feature صغيرة شغالة من أول الـ Database لحد الـ View (مثلاً: Vendor يضيف منتج ويظهر في الـ Storefront). المزايا:

- الـ Feature بتتجرب فعلياً في نفس الـ Sprint، فالأخطاء بتظهر بدري.
- كل عضو بيتعلم كل الطبقات بدل ما يتخصص في طبقة واحدة.
- بنقدر نعرض Demo في آخر كل Sprint.
- لو الوقت خلص، عندنا نسخة شغالة (MVP) مش 70% من كل حاجة.

الترتيب اتحدد بالـ Dependencies: Identity ← Vendor Approval ← Products ← Cart ← Orders ← Stripe + Commission ← Dashboards ← Testing/Security ← Deployment.

## 9.2 ربط الـ Phases بالـ Sprints

| Sprint | الـ Phases | الناتج الرئيسي |
|---|---|---|
| Sprint 0 | 1-7 | Requirements، Diagrams، ERD، Repo جاهز |
| Sprint 1 | 8-10 | مشروع شغال، Database، Identity |
| Sprint 2 | 11 | Vendor Approval |
| Sprint 3 | 12-13 | Storefront + Vendor Products |
| Sprint 4 | 14 | Shopping Cart |
| Sprint 5 | 15 | Checkout + Orders |
| Sprint 6 | 16-17 | Stripe + Commission |
| Sprint 7 | 18-20 | Customer/Vendor/Admin Dashboards |
| Sprint 8 | 21-23 | Testing + Security + Integration |
| Sprint 9 | 24-26 | Deployment + Docs + Presentation |

> **ملاحظة:** الـ Phase 17 (Commission) بتتنفذ مع Phase 16 (Stripe) في Sprint 6 لأن العمولات بتتنشئ داخل Transaction الـ Webhook، فالاتنين متشابكين.

## 9.3 تفاصيل كل Phase

### Phase 1: Requirements Gathering (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نجمع ونفهم احتياجات المستخدمين والمشروع. |
| **Why it matters (ليه مهم؟)** | من غير فهم مشترك هنبني حاجات مختلفة. |
| **Prerequisites** | الـ Project Brief (الوثيقة الأصلية). |
| **Detailed Tasks** | جلسة ورشة (Workshop) لكتابة قائمة الـ Actors والمشاكل والـ Features، وتحديد الـ MVP. |
| **Expected Deliverables** | قائمة Requirements أولية + أسئلة مفتوحة. |
| **Team Collaboration** | كل الفريق يحضر ويكتب؛ كل عضو يعرض فهمه للمشروع في دقيقتين. |
| **Testing Criteria** | كل عضو يقدر يشرح المشروع بنفس المعنى. |
| **Common Mistakes** | الدخول في الحلول التقنية قبل فهم المشكلة. |
| **Definition of Done** | اتفاق الفريق على قائمة الـ Requirements والـ MVP. |
| **Gate (شرط الانتقال)** | لا نبدأ التحليل إلا بعد موافقة الخمسة على القائمة. |

### Phase 2: Business Analysis (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نكتب Scope وFR وNFR وBusiness Rules وRisks. |
| **Why it matters (ليه مهم؟)** | الـ Business Rules هي اللي بتحدد منطق الـ Services بعدين. |
| **Prerequisites** | Phase 1. |
| **Detailed Tasks** | نراجع الفصل 2 ونعدّله بما يناسب الفريق، ونحدد Out-of-Scope. |
| **Expected Deliverables** | وثيقة BA معتمدة (الفصل 2). |
| **Team Collaboration** | كل عضو يراجع قسم ويعرضه، ونصوّت على الـ MVP. |
| **Testing Criteria** | كل Rule قابلة للاختبار ولها رقم (BR-xx). |
| **Common Mistakes** | كتابة Rules غامضة مثل 'يجب أن يكون سريعاً'. |
| **Definition of Done** | FR/NFR/BR واضحة وبأرقام. |
| **Gate (شرط الانتقال)** | مفيش Feature في الـ Backlog بدون Requirement. |

### Phase 3: User Stories and Acceptance Criteria (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نحوّل الـ Requirements لـ Stories صغيرة قابلة للاختبار. |
| **Why it matters (ليه مهم؟)** | الـ Acceptance Criteria هي أساس الاختبار وتعريف 'خلصنا'. |
| **Prerequisites** | Phase 2. |
| **Detailed Tasks** | نكتب Stories بصيغة As a/I want/So that ونضيف Acceptance Criteria ونحطها كـ GitHub Issues. |
| **Expected Deliverables** | Backlog في GitHub Project Board. |
| **Team Collaboration** | كل عضو يكتب 4 Stories ويراجعها عضو تاني. |
| **Testing Criteria** | كل Story لها 3 Acceptance Criteria على الأقل، ومفيش Story أكبر من Sprint. |
| **Common Mistakes** | Stories ضخمة (Epics) من غير تقسيم. |
| **Definition of Done** | Backlog مرتب بالأولوية ومرتبط بالـ Milestones. |
| **Gate (شرط الانتقال)** | كل Story لها Acceptance Criteria مراجعة. |

### Phase 4: Use Case Modeling (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نرسم الـ Actors والـ Use Cases ونحدد العلاقات. |
| **Why it matters (ليه مهم؟)** | بتكشف حالات منسية (مثل Vendor Suspended). |
| **Prerequisites** | Phase 3. |
| **Detailed Tasks** | نكتب UC-01..31، ونرسم Diagrams 2-5 من الـ Source. |
| **Expected Deliverables** | Diagrams 2-5 + جدول Use Cases. |
| **Team Collaboration** | Whiteboard جماعي، وواحد يرسم والباقي يراجع (بيتبدّلوا). |
| **Testing Criteria** | كل Use Case مربوط بـ Story. |
| **Common Mistakes** | رسم Use Cases كلها كأنها Features تقنية. |
| **Definition of Done** | الـ Diagrams معتمدة ومتسقة مع الـ Stories. |
| **Gate (شرط الانتقال)** | كل UC له Actor واضح وPre/Post Conditions. |

### Phase 5: ERD and Database Design (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نصمم الجداول والعلاقات والقيود. |
| **Why it matters (ليه مهم؟)** | تغيير الـ Schema بعد البرمجة مكلف جداً. |
| **Prerequisites** | Phases 2-4. |
| **Detailed Tasks** | نراجع الفصل 5 سطر بسطر، نتأكد من الـ Cardinality والـ Delete Behavior، ونعدّل Diagrams 6A-6C. |
| **Expected Deliverables** | ERD + Data Dictionary معتمدين. |
| **Team Collaboration** | Design Session مشتركة؛ كل عضو يشرح جدولين. |
| **Testing Criteria** | كل علاقة لها FK وسلوك حذف، ولا فيه Multiple Cascade Paths. |
| **Common Mistakes** | استخدام `float` للفلوس أو نسيان الـ Snapshots. |
| **Definition of Done** | ERD متوافق مع Class Diagram والـ Stories. |
| **Gate (شرط الانتقال)** | كل Entity لها Config مخطط وكل عضو شرح جدول. |

### Phase 6: Architecture Design (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نحدد الطبقات وهيكل الفولدرات والـ Dependencies. |
| **Why it matters (ليه مهم؟)** | بيمنع الفوضى لما 5 أشخاص يكتبوا Code. |
| **Prerequisites** | Phase 5. |
| **Detailed Tasks** | نراجع الفصل 6، نقرر Areas وServices، ونكتب `CONTRIBUTING.md` فيه قواعد التسمية. |
| **Expected Deliverables** | Diagrams 7-10 + هيكل فولدرات + Coding Conventions. |
| **Team Collaboration** | كل عضو يشرح طبقة ويجاوب أسئلة الفريق. |
| **Testing Criteria** | الـ Diagrams متسقة مع الفولدرات. |
| **Common Mistakes** | Over-engineering (Repositories وMicroservices). |
| **Definition of Done** | اتفاق على الهيكل والـ Conventions. |
| **Gate (شرط الانتقال)** | كل عضو يقدر يقول 'الـ Code ده يروح فين؟'. |

### Phase 7: GitHub Setup (Sprint 0)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نجهز الـ Repository وقواعد العمل. |
| **Why it matters (ليه مهم؟)** | الأساس لأي تعاون ناجح. |
| **Prerequisites** | Phase 6. |
| **Detailed Tasks** | إنشاء Repo، Branch Protection على `main`، Issue/PR Templates، Labels، Milestones، Project Board، `.gitignore`، CI بسيط. |
| **Expected Deliverables** | Repository جاهز + CI شغال. |
| **Team Collaboration** | كل عضو يعمل Clone ويفتح PR تجريبي ويراجع PR زميله. |
| **Testing Criteria** | PR تجريبي اتعمله Merge بعد Review وCI أخضر. |
| **Common Mistakes** | Push مباشر على `main` أو رفع Secrets. |
| **Definition of Done** | `main` محمي والـ CI شغال. |
| **Gate (شرط الانتقال)** | الخمسة عملوا PR واحد على الأقل. |

### Phase 8: ASP.NET Core MVC Setup (Sprint 1)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | ننشئ الـ Solution والـ Projects والـ Layout الأساسي. |
| **Why it matters (ليه مهم؟)** | كل باقي الـ Features بتتبني عليه. |
| **Prerequisites** | Phase 7 + .NET 10 SDK. |
| **Detailed Tasks** | إنشاء `MarketHub.Web` و`MarketHub.Tests`، `global.json`، Bootstrap Layout، Areas، DI Skeleton، صفحة Home. |
| **Expected Deliverables** | مشروع بيشتغل محلياً. |
| **Team Collaboration** | Pair Programming لأول Setup؛ باقي الأعضاء يكرروا على أجهزتهم. |
| **Testing Criteria** | `dotnet build` و`dotnet test` ينجحوا عند الخمسة. |
| **Common Mistakes** | كل واحد يستخدم SDK مختلف. |
| **Definition of Done** | المشروع يعمل عند الخمسة بنفس الخطوات. |
| **Gate (شرط الانتقال)** | README فيه خطوات التشغيل ومجرّبة. |

### Phase 9: SQL Server and EF Core Configuration (Sprint 1)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نربط الـ DbContext بـ SQL Server ونعمل أول Migration. |
| **Why it matters (ليه مهم؟)** | كل البيانات هتعدي من هنا. |
| **Prerequisites** | Phase 8. |
| **Detailed Tasks** | الـ Entities الأساسية، Configurations، `ApplicationDbContext`، أول Migration، `DbSeeder`، Connection String في user-secrets. |
| **Expected Deliverables** | Database بتتعمل بأمر واحد. |
| **Team Collaboration** | جلسة مشتركة لقواعد الـ Migrations (الفصل 11). |
| **Testing Criteria** | `dotnet ef database update` ينجح عند الجميع. |
| **Common Mistakes** | Connection String مرفوع على GitHub. |
| **Definition of Done** | الـ Schema مطابق للـ ERD. |
| **Gate (شرط الانتقال)** | Seed بيشتغل Idempotent. |

### Phase 10: Identity and Authorization (Sprint 1)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Register/Login/Logout والـ Roles والـ Policies. |
| **Why it matters (ليه مهم؟)** | الأمان لازم يتبني من البداية مش يتحط في الآخر. |
| **Prerequisites** | Phase 9. |
| **Detailed Tasks** | `ApplicationUser`، Identity، Roles، Seeder للـ Admin، Policies، `VendorBaseController`، Navbar حسب الـ Role. |
| **Expected Deliverables** | Auth كامل مع Tests للصلاحيات. |
| **Team Collaboration** | كل عضو يكتب Authorization Test واحد. |
| **Testing Criteria** | 403/401 صحيحين، والـ Role Claims شغالة. |
| **Common Mistakes** | الاعتماد على إخفاء الأزرار. |
| **Definition of Done** | كل Route محمي على السيرفر. |
| **Gate (شرط الانتقال)** | Test Cases 1 و14 (الفصل 12) نجحوا. |

### Phase 11: Vendor Approval (Sprint 2)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Vendor Application ومراجعة الـ Admin. |
| **Why it matters (ليه مهم؟)** | أول Flow Multi-Role كامل. |
| **Prerequisites** | Phase 10. |
| **Detailed Tasks** | `VendorProfile`، فورم التقديم، قائمة Pending، Approve/Reject، `SecurityStamp`. |
| **Expected Deliverables** | Flow Apply → Approve → Dashboard فاضي. |
| **Team Collaboration** | Pair: واحد Vendor وواحد Admin أثناء الاختبار. |
| **Testing Criteria** | الانتقالات الصحيحة فقط. |
| **Common Mistakes** | إضافة Role عند التقديم بدل الموافقة. |
| **Definition of Done** | Test Case 4 (Unapproved vendor) نجح. |
| **Gate (شرط الانتقال)** | Vendor مرفوض ما يقدرش يوصل لـ `/Vendor`. |

### Phase 12: Categories and Products (Sprint 3)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Seed Categories وStorefront للقراءة. |
| **Why it matters (ليه مهم؟)** | محتاجين كتالوج قبل أي حاجة تانية. |
| **Prerequisites** | Phase 11. |
| **Detailed Tasks** | Category CRUD للـ Admin، Product Listing/Details/Search/Filter/Sort/Pagination للعامة. |
| **Expected Deliverables** | Storefront بيعرض Seed Data. |
| **Team Collaboration** | كل عضو يطور جزء (Search / Filter / Sort / Pagination) في Pair مع زميل. |
| **Testing Criteria** | Unit Tests للـ Search والـ Pagination. |
| **Common Mistakes** | جلب كل المنتجات بدون Pagination. |
| **Definition of Done** | منتجات Inactive أو Vendor غير معتمد مخفية. |
| **Gate (شرط الانتقال)** | Performance مقبول مع 1000 منتج Seed. |

### Phase 13: Vendor Product Management (Sprint 3)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | CRUD للمنتجات بصور وعزل كامل. |
| **Why it matters (ليه مهم؟)** | قلب الـ Marketplace وأول اختبار للـ Isolation. |
| **Prerequisites** | Phases 10-12. |
| **Detailed Tasks** | `ProductService`، `FileStorageService`، Create/Edit/Deactivate، Stock Adjust، Concurrency. |
| **Expected Deliverables** | Vendor يدير منتجاته. |
| **Team Collaboration** | Pair Programming + Cross Review، وكل عضو يحاول يكسر العزل. |
| **Testing Criteria** | Test Cases 2 و5 و13 نجحوا. |
| **Common Mistakes** | VendorProfileId في الـ ViewModel. |
| **Definition of Done** | Vendor A مستحيل يوصل لمنتج Vendor B. |
| **Gate (شرط الانتقال)** | رفع ملف خطر مرفوض. |

### Phase 14: Shopping Cart (Sprint 4)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | إضافة/حذف/تعديل مع حسابات من الـ DB. |
| **Why it matters (ليه مهم؟)** | أول Flow فيه Business Rules تجارية. |
| **Prerequisites** | Phases 9, 12. |
| **Detailed Tasks** | `CartService`، `CartController`، Views، Navbar Counter، التعامل مع منتج غير متاح. |
| **Expected Deliverables** | Cart كامل. |
| **Team Collaboration** | Pair: واحد يكتب Test والتاني Code (TDD Ping-Pong). |
| **Testing Criteria** | Test Cases 6-8 نجحوا. |
| **Common Mistakes** | تخزين السعر في الـ Cart. |
| **Definition of Done** | السعر من الـ DB دائماً. |
| **Gate (شرط الانتقال)** | Cart بيعيد الحساب بعد تغيير السعر. |

### Phase 15: Order Management (Sprint 5)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Checkout وإنشاء Order وحجز المخزون وشاشات الطلبات. |
| **Why it matters (ليه مهم؟)** | المنطق المالي والاتساق. |
| **Prerequisites** | Phase 14. |
| **Detailed Tasks** | `CheckoutService` بـ Transaction، Conditional UPDATE، Order History، Vendor Orders، Fulfillment، Cancel. |
| **Expected Deliverables** | Orders تتعمل `PendingPayment`. |
| **Team Collaboration** | Design Session للـ Transaction، وPair على الحجز، وكل الفريق يختبر سباق الشراء. |
| **Testing Criteria** | Test Cases 9-12 و15 نجحوا. |
| **Common Mistakes** | خصم المخزون بـ Read-then-Write بدل Conditional UPDATE. |
| **Definition of Done** | مفيش Overselling. |
| **Gate (شرط الانتقال)** | الطلب يظهر مقسّم بالـ Vendor. |

### Phase 16: Stripe Integration (Sprint 6)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Checkout Session وWebhook آمن. |
| **Why it matters (ليه مهم؟)** | أعقد وأخطر Feature. |
| **Prerequisites** | Phase 15 + Stripe Test Account. |
| **Detailed Tasks** | `StripePaymentService`، `StripeWebhookController`/`Service`، Idempotency، Retry Payment، Background Cleanup. |
| **Expected Deliverables** | الدفع التجريبي شغال end-to-end. |
| **Team Collaboration** | كل الفريق يقرأ Stripe Docs المعتمدة ويشارك في Design Session، وPair على الـ Webhook. |
| **Testing Criteria** | Test Cases 9-11 + `stripe trigger`. |
| **Common Mistakes** | Redirect = Paid، أو قراءة الـ Body بعد Model Binding. |
| **Definition of Done** | Order بيتحول Paid من الـ Webhook فقط. |
| **Gate (شرط الانتقال)** | Webhook مكرر مبيغيّرش شيء. |

### Phase 17: Commission Calculation (Sprint 6)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | حساب وتسجيل العمولة وإعداد النسبة. |
| **Why it matters (ليه مهم؟)** | القيمة التجارية للمنصة. |
| **Prerequisites** | Phase 16 (نفس الـ Sprint). |
| **Detailed Tasks** | `CommissionSetting`، `CommissionService`، دمج في Transaction الـ Webhook، تقارير. |
| **Expected Deliverables** | CommissionRecords صحيحة. |
| **Team Collaboration** | Pair على الـ Calculation مع جدول أمثلة محسوبة يدوياً. |
| **Testing Criteria** | Test Cases 11-12 نجحوا. |
| **Common Mistakes** | تغيير النسبة بأثر رجعي. |
| **Definition of Done** | كل Item مدفوع له سجل واحد. |
| **Gate (شرط الانتقال)** | التقريب والمجاميع مطابقة للجدول اليدوي. |

### Phase 18: Customer Dashboard (Sprint 7)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | Order History وProfile. |
| **Why it matters (ليه مهم؟)** | تجربة العميل بعد الشراء. |
| **Prerequisites** | Phases 15-16. |
| **Detailed Tasks** | قائمة الطلبات، تفاصيل مقسمة بالـ Vendor، Cancel، Review (Should). |
| **Expected Deliverables** | Customer Area. |
| **Team Collaboration** | Cross-review لكل Page. |
| **Testing Criteria** | العميل يشوف طلباته فقط. |
| **Common Mistakes** | عرض Orders بدون فلترة بالـ Customer. |
| **Definition of Done** | IDOR على Orders مرفوض. |
| **Gate (شرط الانتقال)** | Responsive. |

### Phase 19: Vendor Dashboard (Sprint 7)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | ملخص المبيعات والعمولات والـ Order Items. |
| **Why it matters (ليه مهم؟)** | الـ Vendor محتاج يتابع فلوسه. |
| **Prerequisites** | Phases 15-17. |
| **Detailed Tasks** | Aggregates، Charts بسيطة (اختياري)، فلتر تاريخ. |
| **Expected Deliverables** | Dashboard كامل. |
| **Team Collaboration** | Pair: واحد Query والتاني View. |
| **Testing Criteria** | Vendor يشوف أرقامه فقط. |
| **Common Mistakes** | Aggregates من غير فلتر Vendor. |
| **Definition of Done** | الأرقام = مجاميع الـ Database. |
| **Gate (شرط الانتقال)** | Test Case 3 نجح. |

### Phase 20: Super Admin Dashboard (Sprint 7)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | إحصائيات ومراقبة الـ Orders وCommission وModeration. |
| **Why it matters (ليه مهم؟)** | الرقابة على المنصة. |
| **Prerequisites** | Phases 11, 16-17. |
| **Detailed Tasks** | `AdminDashboardService`، Orders Monitoring، Commission Settings، Moderation. |
| **Expected Deliverables** | Admin Area. |
| **Team Collaboration** | Cross-review. |
| **Testing Criteria** | غير الـ Admin = 403. |
| **Common Mistakes** | Dashboard بدون Pagination. |
| **Definition of Done** | كل Action محمي. |
| **Gate (شرط الانتقال)** | الأرقام صحيحة. |

### Phase 21: Testing (Sprint 8)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | اختبار شامل: Unit/Integration/Manual/Regression. |
| **Why it matters (ليه مهم؟)** | Features كتير هتتكسر بالتغييرات. |
| **Prerequisites** | كل الـ Features. |
| **Detailed Tasks** | تنفيذ الفصل 12 كله، Test Matrix، Bug Triage، Regression Run. |
| **Expected Deliverables** | Test Report. |
| **Team Collaboration** | كل عضو ينفذ سيناريوهات عضو تاني (Fresh Eyes). |
| **Testing Criteria** | تغطية الحالات الـ 15 الأساسية. |
| **Common Mistakes** | اختبار الـ Happy Path فقط. |
| **Definition of Done** | لا Bugs Critical مفتوحة. |
| **Gate (شرط الانتقال)** | Test Report معتمد. |

### Phase 22: Security Review (Sprint 8)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | مراجعة الثغرات حسب الفصل 13. |
| **Why it matters (ليه مهم؟)** | الحماية قبل العرض. |
| **Prerequisites** | Phase 21. |
| **Detailed Tasks** | Checklist OWASP-style، مراجعة Secrets، File Upload، CSRF، Headers، Logging. |
| **Expected Deliverables** | Security Checklist موقّعة. |
| **Team Collaboration** | Peer Review: كل عضو يراجع مجال. |
| **Testing Criteria** | كل بند Pass أو له Ticket. |
| **Common Mistakes** | اعتبار HTTPS محلي كأنه Production. |
| **Definition of Done** | لا Secrets ولا IDOR. |
| **Gate (شرط الانتقال)** | Checklist مكتملة. |

### Phase 23: Integration (Sprint 8)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | دمج نهائي وحل التعارضات وتنظيف. |
| **Why it matters (ليه مهم؟)** | نضمن إن المشروع بيشتغل ككل. |
| **Prerequisites** | Phases 21-22. |
| **Detailed Tasks** | Rebase/Merge، Full Regression، تنظيف Code، حل Warnings، Final Migration Check. |
| **Expected Deliverables** | Release Candidate على `main`. |
| **Team Collaboration** | Pair Debugging جماعي. |
| **Testing Criteria** | Build + Tests خضر والـ Demo Flow كامل. |
| **Common Mistakes** | Merge في آخر لحظة بدون اختبار. |
| **Definition of Done** | RC موسوم بـ Tag. |
| **Gate (شرط الانتقال)** | `v1.0.0-rc` Tag. |

### Phase 24: Deployment (Sprint 9)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | نشر نسخة Demo. |
| **Why it matters (ليه مهم؟)** | العرض الحي. |
| **Prerequisites** | Phase 23 + قرار الاستضافة. |
| **Detailed Tasks** | اختيار الـ Host، SQL Server مستضاف، Env Variables، HTTPS، Webhook Endpoint، Migrations Script. |
| **Expected Deliverables** | Demo Online. |
| **Team Collaboration** | كل عضو ينفذ خطوة Deployment (Rotation). |
| **Testing Criteria** | Smoke Test على Production. |
| **Common Mistakes** | استخدام Secrets مرفوعة أو Test Keys بدون Webhook. |
| **Definition of Done** | Checkout تجريبي ناجح Online. |
| **Gate (شرط الانتقال)** | Rollback Plan مكتوب. |

### Phase 25: Documentation (Sprint 9)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | README وScreenshots وتحديث الـ Diagrams. |
| **Why it matters (ليه مهم؟)** | استمرارية المشروع. |
| **Prerequisites** | Phase 24. |
| **Detailed Tasks** | README، دليل التشغيل، Demo Accounts، Screenshots، تحديث الـ Diagrams، ملاحظات التعلم. |
| **Expected Deliverables** | Docs مكتملة. |
| **Team Collaboration** | كل عضو يكتب قسم ويراجع قسم. |
| **Testing Criteria** | شخص جديد يشغل المشروع بالـ README فقط. |
| **Common Mistakes** | Docs قديمة مش متسقة مع الـ Code. |
| **Definition of Done** | الـ Docs مطابقة للنسخة الأخيرة. |
| **Gate (شرط الانتقال)** | جُرّبت على جهاز نظيف. |

### Phase 26: Final Presentation (Sprint 9)

| البند | التفاصيل |
|---|---|
| **Objective (الهدف)** | إعداد العرض والـ Demo. |
| **Why it matters (ليه مهم؟)** | نقدّم الشغل بشكل مقنع. |
| **Prerequisites** | Phase 25. |
| **Detailed Tasks** | نكتب السيناريو (الفصل 15)، نتدرب 3 مرات، نجهز Backup (فيديو). |
| **Expected Deliverables** | عرض + Demo. |
| **Team Collaboration** | كل عضو يقدم جزء ويجاوب أسئلة الأجزاء التانية. |
| **Testing Criteria** | Dry Run ناجح بالوقت المحدد. |
| **Common Mistakes** | Demo حي بدون Plan B. |
| **Definition of Done** | كل عضو يقدر يشرح أي جزء. |
| **Gate (شرط الانتقال)** | Dry Run مسجّل. |

---pagebreak---
