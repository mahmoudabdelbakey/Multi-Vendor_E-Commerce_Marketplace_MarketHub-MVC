# 10. خطة التعاون للفريق (Five-Member Collaboration Plan)

## 10.1 الفلسفة: خمسة أشخاص، مشروع واحد مفهوم عند الكل

هدفنا مش بس نخلّص المشروع، هدفنا إن **كل واحد فينا يقدر يبني ويصلّح ويشرح أي جزء في النظام**. عشان كده مفيش 'مسؤول Backend' ثابت ولا 'مسؤول Frontend' ثابت. لو كل واحد اتخصص في طبقة، هتطلع النتيجة خمس موديولات منفصلة، وكل واحد مش فاهم باقي المشروع.

### الممارسات اللي هنلتزم بيها

- **Shared Planning:** كل Sprint يبدأ بجلسة واحدة للخمسة، وكل واحد بيقول فهمه للـ Stories.
- **Small Cross-Functional Tasks:** كل Task صغيرة (يوم إلى يومين) وبتمس طبقة واحدة، ولكن بتتجمع في Vertical Slice واحد.
- **Pair Programming:** جلستين Pair في كل Sprint بتتبدل الثنائيات (Driver/Navigator بيتبدلوا كل 20-30 دقيقة).
- **Rotating Leadership:** القيادة (مش الملكية) بتدور كل Sprint بين 5 أدوار.
- **Cross-Member Code Review:** كل PR بيتراجع من عضو غير الكاتب.
- **Collective Testing:** كل عضو بيجرب Feature كتبها عضو تاني (Fresh Eyes).
- **Knowledge Sharing:** 30 دقيقة في آخر كل Sprint: كل عضو يشرح جزء كتبه زميله.
- **Sprint Review وRetrospective** في آخر كل Sprint.

## 10.2 الأدوار الدوّارة (Rotating Leadership Roles)

| الدور | المسؤولية خلال الـ Sprint |
|---|---|
| **Facilitator** | يدير الـ Planning والـ Review والـ Retro ويتابع الـ Board |
| **Backend Lead** | يقود قرارات الـ Database والـ Services في الـ Sprint |
| **Frontend Lead** | يقود قرارات الـ Views والـ Layout والـ UX |
| **QA Lead** | يقود خطة الاختبار واختبارات الـ Authorization والـ Regression |
| **Integration and Docs Lead** | يقود ترتيب الـ Merges والـ Migrations وتحديث الـ Docs والـ Diagrams |

> **الـ Lead مش المالك.** الـ Backend Lead مثلاً بيدير النقاش ويقرر لو الفريق اختلف، لكن Code الـ Backend بيكتبه ويراجعه أكتر من واحد، وأي حد ممكن يسأل ويعدّل. القاعدة: **لو عضو لوحده بيفهم جزء، ده Risk لازم نعالجه في نفس الـ Sprint.**

## 10.3 جدول المسؤوليات الدوّارة (Responsibility Matrix)

الجدول بيبيّن دور كل عضو في كل Sprint (Fac = Facilitator، BE = Backend Lead، FE = Frontend Lead، QA = QA Lead، INT = Integration and Docs Lead). كل عضو بياخد كل دور مرتين على مدار الـ 10 Sprints.

| العضو | S0 | S1 | S2 | S3 | S4 | S5 | S6 | S7 | S8 | S9 |
|---|---|---|---|---|---|---|---|---|---|---|
| Member A | Fac | BE | FE | QA | INT | Fac | BE | FE | QA | INT |
| Member B | BE | FE | QA | INT | Fac | BE | FE | QA | INT | Fac |
| Member C | FE | QA | INT | Fac | BE | FE | QA | INT | Fac | BE |
| Member D | QA | INT | Fac | BE | FE | QA | INT | Fac | BE | FE |
| Member E | INT | Fac | BE | FE | QA | INT | Fac | BE | FE | QA |

### جدول الـ Pair Programming (Session 1 / Session 2)

في كل Sprint بنقسم الفريق مرتين: الـ Rover هو العضو الخامس اللي بينضم لثنائي ويشارك كـ Navigator تالت.

| Sprint | Session 1 (النصف الأول) | Session 2 (النصف التاني) |
|---|---|---|
| S0 | A+B ، C+D (Rover: E) | B+C ، D+E (Rover: A) |
| S1 | B+C ، D+E (Rover: A) | C+D ، E+A (Rover: B) |
| S2 | C+D ، E+A (Rover: B) | D+E ، A+B (Rover: C) |
| S3 | D+E ، A+B (Rover: C) | E+A ، B+C (Rover: D) |
| S4 | E+A ، B+C (Rover: D) | A+B ، C+D (Rover: E) |
| S5 | A+B ، C+D (Rover: E) | B+C ، D+E (Rover: A) |
| S6 | B+C ، D+E (Rover: A) | C+D ، E+A (Rover: B) |
| S7 | C+D ، E+A (Rover: B) | D+E ، A+B (Rover: C) |
| S8 | D+E ، A+B (Rover: C) | E+A ، B+C (Rover: D) |
| S9 | E+A ، B+C (Rover: D) | A+B ، C+D (Rover: E) |

### جدول الـ Code Review

كل صف بيوضح مين بيراجع PRs مين. الـ Reviewer الأول مطلوب لكل PR، والتاني مطلوب لـ PRs اللي بتمس الـ Database أو الدفع أو الـ Authorization.

| Sprint | PR بتاع A | PR بتاع B | PR بتاع C | PR بتاع D | PR بتاع E |
|---|---|---|---|---|---|
| S0 | B + C | C + D | D + E | E + A | A + B |
| S1 | C + D | D + E | E + A | A + B | B + C |
| S2 | D + E | E + A | A + B | B + C | C + D |
| S3 | E + B | A + C | B + D | C + E | D + A |
| S4 | B + C | C + D | D + E | E + A | A + B |
| S5 | C + D | D + E | E + A | A + B | B + C |
| S6 | D + E | E + A | A + B | B + C | C + D |
| S7 | E + B | A + C | B + D | C + E | D + A |
| S8 | B + C | C + D | D + E | E + A | A + B |
| S9 | C + D | D + E | E + A | A + B | B + C |

## 10.4 خطة كل Sprint بالتفصيل

المدة المقترحة 2 أسبوع لكل Sprint [Team Decision]. الـ Tasks الخمسة صغيرة ومتكاملة في نفس الـ Vertical Slice. توزيع الـ Tasks بيدور كل Sprint.

### Sprint 0: Discovery and Design (Phases 1-7)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | وصول الفريق لفهم مشترك: Requirements معتمدة، Diagrams، ERD، وRepository جاهز. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=Fac, B=BE, C=FE, D=QA, E=INT). |
| **3. Shared Requirements and Design** | Workshop للـ Requirements، كتابة User Stories، مراجعة الـ ERD سطر بسطر، رسم الـ Wireframes. |
| **4. Small Tasks لكل عضو** | Member A: تحديث الـ ERD والـ Data Dictionary<br>Member B: كتابة Stories وAcceptance Criteria لـ Cart/Orders<br>Member C: رسم Wireframes للـ Storefront وصفحات Vendor<br>Member D: إعداد GitHub (Protection, Templates, CI)<br>Member E: تجهيز مجلد docs وتحديث الـ Diagrams |
| **5. Pair Programming** | Session 1: A+B ، C+D (Rover E). Session 2: B+C ، D+E (Rover A). |
| **6. Code Review** | PR A ← B وC ، PR B ← C وD ، PR C ← D وE ، PR D ← E وA ، PR E ← A وB |
| **7. Integration Tasks** | دمج الـ Docs والـ Diagrams عبر PR واحد مراجع من اتنين. (المسؤول: E كـ INT Lead). |
| **8. Testing Tasks** | مراجعة قابلية اختبار كل Acceptance Criteria. الكل يختبر Feature زميله يدوياً، وQA Lead (D) يجمّع النتائج. |
| **9. Documentation Tasks** | Docs v1 + CONTRIBUTING.md. (E يتأكد من التحديث). |
| **10. Sprint Review** | Demo: عرض الـ Diagrams والـ Backlog. بتقديم عضو مختلف لكل جزء (A يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 1: Foundation (Phases 8-10)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | المشروع شغال عند الخمسة، Database وIdentity جاهزين. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=BE, B=FE, C=QA, D=INT, E=Fac). |
| **3. Shared Requirements and Design** | جلسة على هيكل الفولدرات وقواعد الـ Migrations وسياسة الـ Roles. |
| **4. Small Tasks لكل عضو** | Member A: Layout وNavbar حسب الـ Role<br>Member B: CI + مشروع الـ Tests + TestDbContext<br>Member C: Authorization Tests + خطوات الـ README<br>Member D: Entities والـ Configurations وأول Migration<br>Member E: إعداد Identity والـ Seeder |
| **5. Pair Programming** | Session 1: B+C ، D+E (Rover A). Session 2: C+D ، E+A (Rover B). |
| **6. Code Review** | PR A ← C وD ، PR B ← D وE ، PR C ← E وA ، PR D ← A وB ، PR E ← B وC |
| **7. Integration Tasks** | Merge الـ Migration الأولى أولاً ثم باقي الفروع تعمل Rebase. (المسؤول: D كـ INT Lead). |
| **8. Testing Tasks** | Test Cases 1 و14. الكل يختبر Feature زميله يدوياً، وQA Lead (C) يجمّع النتائج. |
| **9. Documentation Tasks** | README: خطوات التشغيل. (D يتأكد من التحديث). |
| **10. Sprint Review** | Demo: تسجيل دخول بـ 3 Roles. بتقديم عضو مختلف لكل جزء (E يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 2: Vendor Onboarding (Phases 11)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | Vendor يقدّم طلب والـ Admin يوافق أو يرفض. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=FE, B=QA, C=INT, D=Fac, E=BE). |
| **3. Shared Requirements and Design** | مراجعة Diagram 11 وقواعد الانتقال بين الحالات. |
| **4. Small Tasks لكل عضو** | Member A: Tests: Vendor غير معتمد وState Transitions<br>Member B: VendorProfile: Entity وMigration<br>Member C: VendorService: Apply/Approve/Reject<br>Member D: فورم التقديم وقائمة Pending<br>Member E: Policy ApprovedVendor وVendorBaseController |
| **5. Pair Programming** | Session 1: C+D ، E+A (Rover B). Session 2: D+E ، A+B (Rover C). |
| **6. Code Review** | PR A ← D وE ، PR B ← E وA ، PR C ← A وB ، PR D ← B وC ، PR E ← C وD |
| **7. Integration Tasks** | ترتيب: Entity ثم Service ثم Views. (المسؤول: C كـ INT Lead). |
| **8. Testing Tasks** | Test Case 4. الكل يختبر Feature زميله يدوياً، وQA Lead (B) يجمّع النتائج. |
| **9. Documentation Tasks** | تحديث Diagram 11 لو اتغير. (C يتأكد من التحديث). |
| **10. Sprint Review** | Demo: Apply ثم Approve ثم دخول Vendor Area. بتقديم عضو مختلف لكل جزء (D يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 3: Catalog (Phases 12-13)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | Storefront للعامة وإدارة منتجات Vendor مع عزل كامل. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=QA, B=INT, C=Fac, D=BE, E=FE). |
| **3. Shared Requirements and Design** | مراجعة ERD للـ Product وقواعد الصور (BR-09) وDiagram 12 و22. |
| **4. Small Tasks لكل عضو** | Member A: ProductService: Search/Paging وVendor CRUD بالملكية<br>Member B: Views الـ Storefront (Listing/Details)<br>Member C: Vendor Product Views وFileStorageService<br>Member D: Tests: IDOR وSearch + تحديث ERD<br>Member E: Category/Product/ProductImage: Entities وMigration وSeed |
| **5. Pair Programming** | Session 1: D+E ، A+B (Rover C). Session 2: E+A ، B+C (Rover D). |
| **6. Code Review** | PR A ← E وB ، PR B ← A وC ، PR C ← B وD ، PR D ← C وE ، PR E ← D وA |
| **7. Integration Tasks** | Merge الـ Migration أولاً، ثم الـ Services، ثم الـ Views. (المسؤول: B كـ INT Lead). |
| **8. Testing Tasks** | Test Cases 2 و5 و13. الكل يختبر Feature زميله يدوياً، وQA Lead (A) يجمّع النتائج. |
| **9. Documentation Tasks** | تحديث Diagram 6B. (B يتأكد من التحديث). |
| **10. Sprint Review** | Demo: Vendor يضيف منتج ويظهر في الـ Storefront. بتقديم عضو مختلف لكل جزء (C يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 4: Shopping Cart (Phases 14)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | Cart كامل بحسابات من الـ Database. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=INT, B=Fac, C=BE, D=FE, E=QA). |
| **3. Shared Requirements and Design** | مراجعة Diagram 13 وقواعد BR-03 و BR-04. |
| **4. Small Tasks لكل عضو** | Member A: CartController وAntiForgery ورسائل الأخطاء<br>Member B: Tests: مخزون/كمية/تلاعب بالسعر<br>Member C: CartItem: Entity وUnique Index وMigration<br>Member D: CartService: Add/Update/Remove/Get<br>Member E: Cart Views وعداد الـ Navbar |
| **5. Pair Programming** | Session 1: E+A ، B+C (Rover D). Session 2: A+B ، C+D (Rover E). |
| **6. Code Review** | PR A ← B وC ، PR B ← C وD ، PR C ← D وE ، PR D ← E وA ، PR E ← A وB |
| **7. Integration Tasks** | ربط عداد الـ Navbar بالـ Layout المشترك بالتنسيق مع Frontend Lead. (المسؤول: A كـ INT Lead). |
| **8. Testing Tasks** | Test Cases 6-8. الكل يختبر Feature زميله يدوياً، وQA Lead (E) يجمّع النتائج. |
| **9. Documentation Tasks** | README: Seed Demo Accounts. (A يتأكد من التحديث). |
| **10. Sprint Review** | Demo: Cart من 3 Vendors. بتقديم عضو مختلف لكل جزء (B يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 5: Orders and Checkout (Phases 15)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | Checkout بـ Transaction وحجز المخزون وشاشات الطلبات. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=Fac, B=BE, C=FE, D=QA, E=INT). |
| **3. Shared Requirements and Design** | Design Session للـ Transaction ورسم Diagram 14 على السبورة قبل الكود. |
| **4. Small Tasks لكل عضو** | Member A: Order/OrderItem/Payment: Entities وConstraints وMigration<br>Member B: CheckoutService: Transaction وConditional UPDATE<br>Member C: Checkout Views وOrder History<br>Member D: Vendor Orders وOrderService.UpdateFulfillment<br>Member E: Tests: سباق آخر قطعة وState Transitions |
| **5. Pair Programming** | Session 1: A+B ، C+D (Rover E). Session 2: B+C ، D+E (Rover A). |
| **6. Code Review** | PR A ← C وD ، PR B ← D وE ، PR C ← E وA ، PR D ← A وB ، PR E ← B وC |
| **7. Integration Tasks** | Migration واحدة بس لكل الجداول المالية، يكتبها Integration Lead بعد مراجعة الـ Entities. (المسؤول: E كـ INT Lead). |
| **8. Testing Tasks** | Test Cases 9-12 و15. الكل يختبر Feature زميله يدوياً، وQA Lead (D) يجمّع النتائج. |
| **9. Documentation Tasks** | Diagram 20A/20B. (E يتأكد من التحديث). |
| **10. Sprint Review** | Demo: Order مقسوم بين Vendorين. بتقديم عضو مختلف لكل جزء (A يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 6: Payments and Commission (Phases 16-17)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | دفع Stripe Test Mode وWebhook Idempotent وعمولات صحيحة. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=BE, B=FE, C=QA, D=INT, E=Fac). |
| **3. Shared Requirements and Design** | الفريق كله يقرأ صفحات Stripe المعتمدة (Checkout Fulfillment وWebhooks) ويرسم Diagram 17 بإيده. |
| **4. Small Tasks لكل عضو** | Member A: CommissionSetting/Record وCommissionService<br>Member B: Success/Cancel/Retry وAdmin Commission Form<br>Member C: Tests: Duplicate/Signature/Commission وStripe CLI<br>Member D: StripePaymentService: Create Session<br>Member E: Webhook Controller/Service: Signature وIdempotency |
| **5. Pair Programming** | Session 1: B+C ، D+E (Rover A). Session 2: C+D ، E+A (Rover B). |
| **6. Code Review** | PR A ← D وE ، PR B ← E وA ، PR C ← A وB ، PR D ← B وC ، PR E ← C وD |
| **7. Integration Tasks** | الـ Webhook والـ Commission يتدمجوا في Branch مشترك قبل الـ Merge (Pair). (المسؤول: D كـ INT Lead). |
| **8. Testing Tasks** | Test Cases 9-11. الكل يختبر Feature زميله يدوياً، وQA Lead (C) يجمّع النتائج. |
| **9. Documentation Tasks** | Stripe CLI Guide. (D يتأكد من التحديث). |
| **10. Sprint Review** | Demo: دفع تجريبي كامل حتى ظهور Commission. بتقديم عضو مختلف لكل جزء (E يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 7: Dashboards (Phases 18-20)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | Customer وVendor وAdmin Dashboards. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=FE, B=QA, C=INT, D=Fac, E=BE). |
| **3. Shared Requirements and Design** | مراجعة الأرقام المطلوبة في كل Dashboard مع الـ Stories. |
| **4. Small Tasks لكل عضو** | Member A: Tests: الأرقام مقابل الـ Database والـ Authorization<br>Member B: Aggregates للـ Vendor<br>Member C: Aggregates وOrders Monitoring للـ Admin<br>Member D: تفاصيل الطلب وCancel للـ Customer<br>Member E: Views وCharts |
| **5. Pair Programming** | Session 1: C+D ، E+A (Rover B). Session 2: D+E ، A+B (Rover C). |
| **6. Code Review** | PR A ← E وB ، PR B ← A وC ، PR C ← B وD ، PR D ← C وE ، PR E ← D وA |
| **7. Integration Tasks** | توحيد الـ Layout بين الـ Areas. (المسؤول: C كـ INT Lead). |
| **8. Testing Tasks** | Test Case 3. الكل يختبر Feature زميله يدوياً، وQA Lead (B) يجمّع النتائج. |
| **9. Documentation Tasks** | Screenshots أولية. (C يتأكد من التحديث). |
| **10. Sprint Review** | Demo: لوحة Vendor وAdmin بأرقام حقيقية. بتقديم عضو مختلف لكل جزء (D يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 8: Hardening (Phases 21-23)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | اختبار شامل وأمان وإصلاح الأخطاء. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=QA, B=INT, C=Fac, D=BE, E=FE). |
| **3. Shared Requirements and Design** | مراجعة Checklist الأمان وTest Matrix كاملة. |
| **4. Small Tasks لكل عضو** | Member A: إصلاح Bugs من Test Report<br>Member B: مراجعة Indexes والـ Queries<br>Member C: Regression Automation في الـ CI<br>Member D: Test Report وتحديث Docs<br>Member E: Security Review (Secrets/CSRF/Uploads) |
| **5. Pair Programming** | Session 1: D+E ، A+B (Rover C). Session 2: E+A ، B+C (Rover D). |
| **6. Code Review** | PR A ← B وC ، PR B ← C وD ، PR C ← D وE ، PR D ← E وA ، PR E ← A وB |
| **7. Integration Tasks** | Freeze للـ Features الجديدة؛ Bug Fixes فقط. (المسؤول: B كـ INT Lead). |
| **8. Testing Tasks** | كل الحالات الـ 15. الكل يختبر Feature زميله يدوياً، وQA Lead (A) يجمّع النتائج. |
| **9. Documentation Tasks** | Test Report. (B يتأكد من التحديث). |
| **10. Sprint Review** | Demo: Regression كامل. بتقديم عضو مختلف لكل جزء (C يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

### Sprint 9: Release (Phases 24-26)

| البند | التفاصيل |
|---|---|
| **1. Sprint Goal** | نشر Demo وWebhook حي وتجهيز العرض. |
| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار (A=INT, B=Fac, C=BE, D=FE, E=QA). |
| **3. Shared Requirements and Design** | مراجعة Deployment Plan وسيناريو الـ Demo. |
| **4. Small Tasks لكل عضو** | Member A: العرض التقديمي وسكريبت الـ Demo<br>Member B: تحديث نهائي للـ Diagrams وDry Runs<br>Member C: Deployment Config وMigrations Script<br>Member D: Smoke Tests على Production وWebhook Endpoint<br>Member E: README وScreenshots وDemo Accounts |
| **5. Pair Programming** | Session 1: E+A ، B+C (Rover D). Session 2: A+B ، C+D (Rover E). |
| **6. Code Review** | PR A ← C وD ، PR B ← D وE ، PR C ← E وA ، PR D ← A وB ، PR E ← B وC |
| **7. Integration Tasks** | Tag `v1.0.0` بعد Smoke Test. (المسؤول: A كـ INT Lead). |
| **8. Testing Tasks** | Smoke + Dry Run. الكل يختبر Feature زميله يدوياً، وQA Lead (E) يجمّع النتائج. |
| **9. Documentation Tasks** | Docs النهائية. (A يتأكد من التحديث). |
| **10. Sprint Review** | Demo: العرض النهائي. بتقديم عضو مختلف لكل جزء (B يدير الجلسة). |
| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |

## 10.5 مثال عملي: الفريق كله على Feature واحدة (Vendor Product Management، Sprint 3)

| اليوم | النشاط | مين؟ |
|---|---|---|
| 1 | Planning: مراجعة US-14 وDiagrams 12 و22 وERD الخاص بـ Product. كل عضو يشرح جزء بصوته. | الخمسة |
| 2 | Design Session: نكتب على السبورة شكل `ProductService` وحالات الخطأ وقواعد الملكية. | الخمسة |
| 2-3 | Entities وConfigurations وMigration (Pair). | عضوين + Rover |
| 3-5 | ProductService (Create/Update/Deactivate) بالملكية (Pair بالتبادل Driver/Navigator). | عضوين |
| 3-5 | Views الـ Vendor (Index/Create/Edit) و`FileStorageService`. | عضوين |
| 5 | Integration: Merge الـ Migration، ثم الـ Service، ثم الـ Views، مع Rebase للفروع. | INT Lead + الباقي |
| 6 | Collective Testing: كل عضو يحاول يكسر الصلاحيات (Vendor A ← منتج Vendor B) ويكتب Test. | الخمسة |
| 7 | Code Review متبادل (حسب الجدول)، وتصحيح الملاحظات. | كل عضو بيراجع PR |
| 8 | Docs: تحديث Diagram 6B وـ README. | INT Lead + Reviewer |
| 9 | Sprint Review: Demo من عضو، والباقي يجاوب أسئلة. Knowledge Share لمدة 30 دقيقة. | الخمسة |

## 10.6 تنسيق التعديلات على الملفات المشتركة

| الملف المشترك | المشكلة | القاعدة |
|---|---|---|
| `ApplicationDbContext` | كل Feature بتضيف `DbSet`، فيحصل Conflict على نفس السطور. | نضيف الـ `DbSet` في سطر جديد بترتيب أبجدي، ونحط الـ Fluent Config في ملفات `Configurations/` منفصلة (`ApplyConfigurationsFromAssembly`). |
| `Program.cs` | كل Feature بتسجل Services. | نقسّم التسجيل لـ Extension Methods (`AddCartServices()` ...) في `Infrastructure/ServiceRegistration.cs`، وكل Feature تعدّل ملف خاص بيها قدر الإمكان. |
| Shared ViewModels | تعديلات متضاربة على نفس الـ Class. | لا نعدّل ViewModel مشترك إلا بعد إعلام الفريق، والتغيير يتعمل في PR صغير مستقل. |
| Shared Layouts (`_Layout.cshtml`) | تغييرات في الـ Navbar من أكتر من عضو. | Frontend Lead في الـ Sprint هو بوابة التغيير، ونستخدم Partial Views (`_NavbarCart.cshtml`) لتقليل التضارب. |
| Entity Configuration | تعديل نفس الـ Configuration. | ملف لكل Entity، وأي تغيير في Entity موجودة بيمر على PR من الـ Backend Lead. |
| DI Configuration | ترتيب التسجيل. | الترتيب مش مهم للـ Scoped Services العادية، لكن لازم نتجنب تسجيل نفس الـ Interface مرتين. |
| Migrations | Snapshot واحد، لو اتنين عملوا Migration هيتعارضوا. | القواعد في الفصل 11. |

## 10.7 ازاي نقلل الـ Merge Conflicts ونضمن إن الكل فاهم؟

1. **Branches قصيرة العمر** (1-3 أيام) و`git fetch` + Rebase على `main` كل يوم.
2. **Small PRs** (أقل من 400 سطر قدر الإمكان).
3. **ملفات صغيرة** (Configuration لكل Entity، Controller لكل مجال).
4. **Merge Order** يحدده INT Lead في الـ Integration: Migrations ثم Services ثم Controllers ثم Views.
5. **Code Review كوسيلة تعلم:** الـ Reviewer لازم يكتب في الـ PR سطرين بيشرح فيهم ماذا فهم من الـ Change، وإن ما فهمش يسأل.
6. **'Explain it back':** في الـ Sprint Review أي عضو ممكن يتسأل عن أي Feature، وليس فقط اللي كتبها.
7. **Rotating Tasks:** مفيش حد ياخد نفس نوع الـ Task في Sprintين متتاليين.
8. **Bus Factor ≥ 3:** أي جزء في النظام لازم يكون على الأقل 3 أشخاص يعرفوه.

---pagebreak---
