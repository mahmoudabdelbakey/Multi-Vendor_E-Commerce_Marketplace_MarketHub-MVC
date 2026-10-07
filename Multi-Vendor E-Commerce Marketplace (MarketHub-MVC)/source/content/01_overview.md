# 1. نظرة عامة على المشروع (Project Overview)

## 1.1 فكرة MarketHub-MVC

`MarketHub-MVC` هو موقع **Multi-Vendor E-Commerce Marketplace**. يعني بدل ما يبقى فيه متجر واحد بيبيع منتجاته، عندنا منصة بيسجل فيها أكتر من `Vendor`، وكل Vendor عنده متجره ومنتجاته ومخزونه. الـ `Customer` بيتصفح منتجات من متاجر مختلفة، يحطها في `Shopping Cart` واحد، ويدفع مرة واحدة بـ `Stripe` (Test Mode). والـ `Super Admin` هو اللي بيدير المنصة: يوافق على الـ Vendors، يدير الـ Categories، ويحدد نسبة العمولة (Commission).

الفرق الجوهري بين المتجر العادي والـ Marketplace إن **كل صف في الـ Order ممكن يتبع Vendor مختلف**، وده بيأثر على: الـ Database Design، الـ Authorization، الـ Inventory، حساب العمولة، وحتى شكل الـ Order Status. فهم النقطة دي هو مفتاح المشروع كله.

## 1.2 الـ Technology Stack والقرارات التقنية

| المكوّن | القرار | ليه اخترناه؟ |
|---|---|---|
| Language | C# | اللغة الرسمية لـ .NET وكل الكورس بتاعنا عليها. |
| .NET Version | **.NET 10 (LTS)** | Microsoft Support Policy: .NET 10 اتصدر في November 11, 2025 وهو LTS ودعمه لحد November 14, 2028. أما .NET 8 فدعمه بينتهي November 10, 2026، يعني قبل ما نخلّص المشروع بشهور. |
| Web Framework | ASP.NET Core MVC | بيفصل الـ Controllers عن الـ Views، وبيدّينا Server-Side Rendering سهل للمبتدئين، ومناسب لـ Razor Views. |
| Database | SQL Server | Relational وده الأنسب لبيانات مالية (Orders, Payments) محتاجة Transactions و Constraints. |
| ORM | Entity Framework Core | بيحوّل الـ LINQ لـ SQL، وبيدير الـ Migrations، وبيحمينا من SQL Injection لأنه بيستخدم Parameters. |
| Auth | ASP.NET Core Identity | Authentication + Roles + Password Hashing جاهزين ومجرّبين. |
| Frontend | HTML, CSS, JavaScript, Bootstrap 5 | سريع ومعروف، ومش محتاجين Single Page Application. |
| Payments | Stripe Checkout (Test Mode) + Stripe.net | الـ Hosted Checkout بيقلل مخاطر الـ PCI لأن بيانات الكارت مبتعديش على سيرفرنا. |
| Source Control | Git + GitHub | Pull Requests و Code Reviews و Issues. |

> **معلومة مهمة:** رقم الـ patch بتاع .NET بيتغير شهرياً. استخدموا آخر SDK متاح من الموقع الرسمي، ودوّروا على `global.json` لتثبيت الـ SDK عند كل الفريق.

## 1.3 ليه Architecture بسيطة؟

فريقنا 5 طلاب بيتعلموا. عشان كده اخترنا **Single Web Project (Modular Monolith بسيط)** بدل Microservices أو Clean Architecture بـ 5 projects:

- Project واحد اسمه `MarketHub.Web` فيه الـ Controllers والـ Services والـ Data.
- Project تاني للـ Tests اسمه `MarketHub.Tests`.
- **مفيش Repository Pattern**: الـ `DbContext` في EF Core هو بالفعل Unit of Work وبيشتغل كـ Repository. لو لفّيناه في Repository تاني هنزود Code من غير فايدة.
- الـ Services عندها Interfaces لسببين واضحين: الـ Dependency Injection والـ Unit Testing (نقدر نعمل Mock).

## 1.4 تصنيف المعلومات في الوثيقة

علشان نفرّق بين الحقائق والتوصيات، هنستخدم الوسوم دي:

| الوسم | المعنى |
|---|---|
| **[Verified]** | اتأكدنا منها من Official Documentation وقت كتابة الوثيقة (October 2026). |
| **[Recommendation]** | تصميم بنوصي بيه، والفريق ممكن يغيّره بسبب مقنع. |
| **[Assumption]** | افتراض احنا حطيناه، لازم الفريق يتأكد منه. |
| **[Team Decision]** | قرار لازم الفريق ياخده بنفسه (مفيش إجابة صح واحدة). |

### الحقائق اللي اتحققنا منها [Verified]

- .NET 10 هو LTS وفي Active support وينتهي دعمه November 14, 2028، و.NET 8 في Maintenance وينتهي November 10, 2026 (Microsoft .NET Support Policy).
- في Stripe Checkout، الحدث `checkout.session.completed` بيتبعت لما العميل يدفع، والطرق المتأخرة (Delayed payment methods) بتبعت `checkout.session.async_payment_succeeded` لما الدفع ينجح بعدين، وممكن نسمع كمان لـ `checkout.session.async_payment_failed` (Stripe docs: Fulfill orders).
- لازم نتحقق من توقيع الـ Webhook (Signature Verification) باستخدام الـ **raw request body** وهيدر `Stripe-Signature` وسر الـ Endpoint (`whsec_...`)، ولازم الـ Handler يتعامل صح لو اتنادى أكتر من مرة لنفس الـ Checkout Session.
- الـ Stripe CLI بيوفر الأمر `stripe listen --forward-to localhost:PORT/path` لتجربة الـ Webhooks محلياً.

### أشياء لازم الفريق يتأكد منها قبل الاعتماد عليها [Team Decision]

- الاسم الدقيق لدوال `Stripe.net` في النسخة اللي هتتثبّت (مثلاً `EventUtility.ConstructEvent` و`Stripe.Checkout.SessionService`): راجعوا الـ API Reference الرسمي لأن الأسماء والـ Signatures ممكن تتغير بين الإصدارات.
- أقل وأقصى مدة لـ `expires_at` في Checkout Session (بنفترض إن الحد الأدنى 30 دقيقة، [Assumption] راجعوها في الـ Stripe API docs).
- الدول والعملات المدعومة لحساب Stripe بتاعكم، وهل Stripe Connect متاح في بلدكم (مهم للمرحلة المستقبلية فقط).
- مزوّد الاستضافة (Hosting) ونوع SQL Server المستضاف.

## 1.5 الافتراضات الأساسية (Assumptions)

1. العملة الوحيدة في الـ MVP هي `USD` (Stripe Test Mode)، والعمود `Currency` موجود للتوسع المستقبلي.
2. مفيش Shipping Fee ولا ضرايب في الـ MVP: `TotalAmount` = مجموع `LineTotal`.
3. الـ Cart بيتخزن في الـ Database للـ Customer المسجّل فقط، والـ Guest لازم يسجّل دخول قبل الإضافة للـ Cart.
4. كل Customer عنده Role واحدة (`Customer`)، والـ Vendor عنده `Vendor`، والـ Admin عنده `SuperAdmin`.
5. Vendor Application بتتعمل مع إنشاء الحساب في نفس الخطوة؛ الـ Role `Vendor` بتتضاف **بعد** موافقة الـ Admin فقط.
6. الصور بتتخزن على الـ File System داخل `wwwroot/uploads` في الـ MVP.
7. الفريق عنده 5 أعضاء، كل Sprint أسبوعين [Team Decision]، ولو الوقت ضيق ممكن يتضغط لأسبوع مع تقليل الـ Scope.

---pagebreak---
