::: {align="center"}
# MarketHub --- Multi-Vendor E-Commerce Marketplace

### منصة سوق إلكتروني متعددة البائعين

**C# · ASP.NET Core MVC · Entity Framework Core · SQL Server**

A team project designed to bring multiple vendors and customers together
in one online marketplace.\
مشروع جماعي يهدف إلى جمع عدة بائعين وعملاء داخل منصة تجارة إلكترونية
واحدة.
:::

------------------------------------------------------------------------

## Contents \| المحتويات

-   [English](#english)
-   [العربية](#العربية)
-   [Architecture & Project
    Structure](#architecture--project-structure--هيكل-المشروع)
-   [Setup](#getting-started--تشغيل-المشروع)
-   [Security](#security-notes--ملاحظات-الأمان)

# English

## 1. Overview

**MarketHub** is a multi-vendor e-commerce marketplace built with
ASP.NET Core MVC and organized into three main projects:

-   **MarketHub.MVC** --- presentation layer: controllers, Razor views,
    ViewModels, and user interaction.
-   **MarketHub.BLL** --- business logic layer: service interfaces,
    service implementations, and business rules.
-   **MarketHub.DAL** --- data access layer: Entity Framework Core,
    database context, entities, configurations, and migrations.

> **Project status:** This README describes the architecture and
> intended scope. A listed feature may be implemented, in progress, or
> planned. Check the source code and project board for its current
> status.

## 2. Goals

-   Bring multiple vendors into one marketplace.
-   Separate presentation, business logic, and data access.
-   Apply role-based authorization and vendor data isolation.
-   Maintain consistent product, inventory, order, payment, and
    commission data.
-   Support maintainable code and collaborative development.

## 3. User Roles

-   **Guest:** Browse public pages, categories, and available products.
-   **Customer:** Register/sign in, browse products, manage a cart,
    check out, and review orders where implemented.
-   **Vendor:** Apply for vendor access; after approval, manage store
    information, products, stock, and related order items.
-   **Super Admin:** Review vendor applications, manage categories,
    oversee orders, commissions, reports, and moderation according to
    implemented functionality.

## 4. Feature Scope

  -----------------------------------------------------------------------
  Area                                Intended capabilities
  ----------------------------------- -----------------------------------
  Public storefront                   Home page, categories, product
                                      listing/details, search, filtering,
                                      sorting, pagination

  Authentication                      Registration, login/logout,
                                      role-based authorization

  Vendor management                   Application, approval/rejection,
                                      store profile, dashboard

  Product catalog                     Create/edit products, category
                                      assignment, images, price/stock
                                      validation, activation/deactivation

  Shopping cart                       Add/remove items, update quantity,
                                      validate current price and stock

  Orders & checkout                   Checkout validation, order
                                      creation, order history, order
                                      status and vendor fulfillment

  Payments                            Stripe Checkout test-mode workflow
                                      and webhook confirmation, subject
                                      to implementation

  Commissions                         Commission settings, calculations,
                                      records, and reporting, subject to
                                      implementation

  Administration                      Vendor approvals, categories, order
                                      oversight, commissions,
                                      analytics/moderation
  -----------------------------------------------------------------------

The list represents the project scope, not a guarantee that all features
are complete.

## 5. Technology Stack

  -----------------------------------------------------------------------
  Technology                          Purpose
  ----------------------------------- -----------------------------------
  C#                                  Main language

  ASP.NET Core MVC                    Web application and MVC flow

  Razor Views (`.cshtml`)             Server-rendered pages

  Entity Framework Core               ORM and database access

  Microsoft SQL Server                Relational database

  Fluent API                          Entity mapping, relationships, and
                                      constraints

  Dependency Injection                Service registration and dependency
                                      management

  Bootstrap, CSS, JavaScript          UI and client-side interactions

  Stripe                              Planned/test-mode payment
                                      integration, depending on
                                      implementation

  Git & GitHub                        Version control and collaboration
  -----------------------------------------------------------------------

Use the .NET SDK specified by the project files (`TargetFramework` in
the `.csproj` files).

## 6. Architecture

``` text
Browser
   |
   v
MarketHub.MVC  (Presentation)
Controllers -> ViewModels -> Razor Views
   |
   v
MarketHub.BLL  (Business Logic)
Interfaces -> Services -> Business Rules
   |
   v
MarketHub.DAL  (Data Access)
AppDbContext -> EF Core Configurations -> Entities / Migrations
   |
   v
SQL Server
```

### Request flow

1.  The browser sends a request to an MVC controller.
2.  The controller calls a service through an interface.
3.  The service applies business rules and accesses data through the
    DAL.
4.  The controller prepares a ViewModel and returns a View or redirect.
5.  The View renders the response.

### Why interfaces and services?

`ICategoryService` defines category-related operations, while
`CategoryService` implements them. A controller depends on the
interface, and Dependency Injection provides the concrete
implementation. This allows multiple controllers to reuse category
operations without duplicating database-query logic.

## 7. Project Structure

The solution contains three main projects. This is a logical overview;
exact subfolders and filenames may evolve.

``` text
MarketHub/
├── MarketHub.slnx
├── MarketHub.BLL/
│   ├── Interfaces/
│   │   └── ICategoryService.cs
│   └── Services/
│       └── CategoryService.cs
├── MarketHub.DAL/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   ├── Enums/
│   └── Models/
│       ├── ApplicationUser.cs
│       ├── CartItem.cs
│       ├── Category.cs
│       ├── CommissionRecord.cs
│       ├── CommissionSetting.cs
│       ├── Order.cs
│       ├── OrderItem.cs
│       ├── Payment.cs
│       ├── Product.cs
│       ├── ProductImage.cs
│       └── VendorProfile.cs
└── MarketHub.MVC/
    ├── Controllers/
    ├── ViewModels/
    ├── Views/
    │   ├── Home/
    │   └── Shared/
    ├── wwwroot/
    │   ├── css/
    │   ├── js/
    │   └── images/
    ├── appsettings.json
    └── Program.cs
```

> This tree is a guide to the main responsibilities, not a generated
> inventory of every file. Keep it synchronized with the actual
> repository.

### Main data concepts

-   `ApplicationUser` --- user account.
-   `VendorProfile` --- vendor/store profile and approval state.
-   `Category` --- product category.
-   `Product` / `ProductImage` --- product data and images.
-   `CartItem` --- product and quantity in a cart.
-   `Order` / `OrderItem` --- order and purchased items.
-   `Payment` --- payment-related records.
-   `CommissionSetting` / `CommissionRecord` --- commission settings and
    records.

Actual columns, relationships, and constraints are defined by the entity
classes, EF Core configurations, and migrations.

## 8. Entity Framework Core

-   `AppDbContext` is the EF Core entry point to the database.
-   Configuration classes define mappings and relationships.
-   Migrations track database schema changes.
-   Use `AsNoTracking()` for read-only queries where appropriate.
-   Validate input and enforce business rules before saving changes.

## 9. Getting Started

### Prerequisites

1.  Git.
2.  The .NET SDK required by the project.
3.  SQL Server or an accessible development database.
4.  Visual Studio 2022 or another compatible editor.

### Clone

``` bash
git clone https://github.com/mahmoudabdelbakey/Multi-Vendor_E-Commerce_Marketplace_MarketHub-MVC.git
cd "Multi-Vendor E-Commerce Marketplace (MarketHub-MVC)"
```

If the folder name differs, enter the directory containing
`MarketHub.slnx`.

### Configure the database

1.  Locate the connection-string configuration used by `MarketHub.MVC`.
2.  Set it to your local SQL Server instance.
3.  Keep credentials and secrets out of source control.
4.  Confirm SQL Server is running.

The connection-string key must match what the current `Program.cs` /
`AppDbContext` reads.

### Restore and build

Run from the directory containing the solution file:

``` bash
dotnet restore
dotnet build
```

### Apply migrations

Depending on the actual migration and startup-project configuration, a
common command is:

``` bash
dotnet ef database update --project MarketHub.DAL --startup-project MarketHub.MVC
```

If `dotnet ef` is unavailable, install the CLI tool and ensure its
version is compatible with the project's EF Core packages:

``` bash
dotnet tool install --global dotnet-ef
```

If the command fails, verify project paths, startup configuration,
migrations assembly, and SDK version. Do not create a migration blindly
to solve a connection or startup error.

### Run

``` bash
dotnet run --project MarketHub.MVC
```

Open the local URL printed in the terminal. In Visual Studio, open
`MarketHub.slnx`, set `MarketHub.MVC` as the startup project, and run
it.

> These commands are starting points; verify project references and the
> repository's actual configuration before running them unchanged.

## 10. Security Notes

-   Never commit database passwords, Stripe keys, or other secrets.
-   Use .NET User Secrets or environment variables for local
    credentials.
-   Validate input on the server.
-   Enforce authorization on the server; hiding a UI button is not
    access control.
-   Check vendor ownership before accessing or changing vendor-specific
    products and orders.
-   Protect state-changing forms against CSRF where applicable.
-   Confirm payments through the trusted server-side payment/webhook
    flow, not only a browser redirect.
-   Use Stripe test mode during development.

## 11. Team Workflow

1.  Pull the latest shared branch.
2.  Create a focused feature or bug-fix branch.
3.  Implement the change and relevant tests.
4.  Build and test locally.
5.  Open a Pull Request with a summary and test steps.
6.  Review changes and resolve conflicts carefully.
7.  Update documentation when behavior or architecture changes.

Example branches:

``` text
feature/category-list
feature/home-page
fix/cart-stock-validation
docs/update-readme
```

Example commits:

``` text
feat(catalog): add category listing
fix(cart): validate stock before checkout
docs: update project README
```

## 12. Testing

If a test project is configured, run:

``` bash
dotnet test
```

Important scenarios include unauthorized access, vendor data isolation,
product/stock validation, server-side order totals, and safe webhook
processing. Document actual test projects and coverage after verifying
them in the repository.

## 13. Documentation

The project documentation package covers requirements, user stories, use
cases, database design/ERD, architecture and class diagrams, vendor
workflows, checkout/payment/webhook flows, and team planning. Refer to
the documentation and diagrams committed in the repository for detailed
design decisions.

## 14. Roadmap

Track actual status in GitHub Issues, the project board, and source
code.

-   [ ] Solution and database setup
-   [ ] Authentication and role-based authorization
-   [ ] Vendor applications and admin approval
-   [ ] Categories and public catalog
-   [ ] Product management and inventory validation
-   [ ] Shopping cart
-   [ ] Checkout and order lifecycle
-   [ ] Stripe test-mode flow and webhooks
-   [ ] Commission calculations and reports
-   [ ] Vendor/admin dashboards
-   [ ] Tests, security review, and deployment/demo preparation

Do not mark an item complete until it is implemented and verified.

## 15. License

No license is specified here. Add a `LICENSE` file and update this
section after the project owners agree on a license.

------------------------------------------------------------------------

# العربية

## 1. نبذة عن المشروع

**MarketHub** هو مشروع منصة تجارة إلكترونية متعددة البائعين باستخدام
**ASP.NET Core MVC**. الهدف هو جمع بائعين متعددين في مكان واحد، بحيث
يتصفح العميل المنتجات، بينما يدير كل بائع منتجاته ومخزونه والطلبات
المرتبطة به حسب الصلاحيات.

المشروع مقسّم إلى ثلاثة مشاريع رئيسية: - **MarketHub.MVC:** طبقة العرض،
وتشمل Controllers وViews وViewModels. - **MarketHub.BLL:** طبقة منطق
الأعمال، وتشمل Interfaces وServices وقواعد العمل. - **MarketHub.DAL:**
طبقة الوصول إلى البيانات، وتشمل Entities و`AppDbContext` وإعدادات EF
Core والـ Migrations.

> **حالة المشروع:** الملف يوضح التصميم ونطاق العمل. وجود ميزة في القائمة
> لا يعني أنها اكتملت؛ قد تكون منفذة أو قيد التنفيذ أو مخططة. راجع الكود
> والـ Project Board لمعرفة الحالة الفعلية.

## 2. أهداف المشروع

-   جمع عدة بائعين في منصة واحدة.
-   فصل الواجهة عن منطق الأعمال والوصول إلى البيانات.
-   تطبيق الصلاحيات حسب دور المستخدم وعزل بيانات البائعين.
-   الحفاظ على اتساق بيانات المنتجات والمخزون والطلبات والمدفوعات
    والعمولات.
-   تنظيم الكود لتسهيل الصيانة والتطوير الجماعي.

## 3. أنواع المستخدمين

-   **الزائر (Guest):** تصفح الصفحات العامة والتصنيفات والمنتجات
    المتاحة.
-   **العميل (Customer):** إنشاء حساب، تسجيل الدخول، تصفح المنتجات،
    استخدام السلة، وإتمام الشراء ومراجعة الطلبات حسب التنفيذ.
-   **البائع (Vendor):** تقديم طلب للانضمام، وبعد الموافقة إدارة المتجر
    والمنتجات والمخزون وعناصر الطلبات الخاصة به.
-   **المدير الرئيسي (Super Admin):** مراجعة طلبات البائعين وإدارة
    التصنيفات ومتابعة الطلبات والعمولات والتقارير حسب المميزات المنفذة.

## 4. نطاق المميزات

  -----------------------------------------------------------------------
  الجزء                               المميزات المستهدفة
  ----------------------------------- -----------------------------------
  واجهة المتجر                        الصفحة الرئيسية، التصنيفات، قائمة
                                      المنتجات وتفاصيلها، البحث والتصفية
                                      والترتيب وتقسيم الصفحات

  الحسابات والصلاحيات                 التسجيل وتسجيل الدخول والخروج
                                      والصلاحيات حسب الدور

  إدارة البائعين                      طلب الانضمام، الموافقة أو الرفض،
                                      بيانات المتجر ولوحة التحكم

  إدارة المنتجات                      الإضافة والتعديل والتصنيفات والصور
                                      والتحقق من السعر والمخزون والتفعيل
                                      والإيقاف

  سلة المشتريات                       إضافة المنتجات وحذفها وتعديل الكمية
                                      والتحقق من السعر والمخزون

  الطلبات والشراء                     التحقق أثناء الشراء وإنشاء الطلبات
                                      وسجل الطلبات وإدارة الحالة

  المدفوعات                           Stripe Checkout بوضع الاختبار
                                      ومعالجة Webhooks، حسب التنفيذ

  العمولات                            إعدادات العمولة وحسابها وسجلاتها
                                      وتقاريرها، حسب التنفيذ

  الإدارة                             مراجعة البائعين وإدارة التصنيفات
                                      والطلبات والعمولات والتقارير
  -----------------------------------------------------------------------

هذه قائمة بنطاق المشروع وليست تأكيدًا على اكتمال كل المميزات.

## 5. التقنيات

-   **C#:** لغة البرمجة الأساسية.
-   **ASP.NET Core MVC:** بناء تطبيق الويب.
-   **Razor Views:** عرض صفحات `.cshtml`.
-   **Entity Framework Core:** التعامل مع قاعدة البيانات.
-   **SQL Server:** قاعدة البيانات العلائقية.
-   **Fluent API:** إعداد العلاقات والقيود.
-   **Dependency Injection:** توفير الخدمات واعتمادياتها.
-   **Bootstrap / CSS / JavaScript:** تصميم الواجهة.
-   **Stripe:** تكامل المدفوعات المستهدف أو وضع الاختبار، حسب التنفيذ.
-   **Git & GitHub:** إدارة الإصدارات والتعاون.

استخدم إصدار .NET SDK المحدد في ملفات المشروع.

## 6. معمارية المشروع

``` text
المتصفح
   |
   v
MarketHub.MVC  (طبقة العرض)
Controllers -> ViewModels -> Razor Views
   |
   v
MarketHub.BLL  (منطق الأعمال)
Interfaces -> Services -> Business Rules
   |
   v
MarketHub.DAL  (الوصول للبيانات)
AppDbContext -> EF Core Configurations -> Entities / Migrations
   |
   v
SQL Server
```

### دورة الطلب

1.  المتصفح يرسل Request إلى Controller.
2.  الـ Controller يستدعي Service من خلال Interface.
3.  الـ Service تنفذ قواعد العمل وتتواصل مع DAL للوصول إلى البيانات.
4.  الـ Controller يجهز ViewModel ويرجع View أو Redirect.
5.  الـ View تعرض النتيجة للمستخدم.

### دور الـ Interfaces والـ Services

`ICategoryService` بتحدد العمليات الخاصة بالتصنيفات، بينما
`CategoryService` بتنّفذها. الـ Controller يعتمد على الـ Interface، والـ
Dependency Injection توفر التنفيذ المناسب. بذلك يمكن استخدام الخدمة
نفسها في أكثر من Controller دون تكرار كود الاستعلامات.

## 7. هيكل المشروع

الشجرة التالية توضح التنظيم المنطقي للمشاريع الرئيسية؛ وقد تتغير بعض
الملفات أو المجلدات مع تطور الكود:

``` text
MarketHub/
├── MarketHub.slnx
├── MarketHub.BLL/
│   ├── Interfaces/
│   │   └── ICategoryService.cs
│   └── Services/
│       └── CategoryService.cs
├── MarketHub.DAL/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   ├── Enums/
│   └── Models/
│       ├── ApplicationUser.cs
│       ├── CartItem.cs
│       ├── Category.cs
│       ├── CommissionRecord.cs
│       ├── CommissionSetting.cs
│       ├── Order.cs
│       ├── OrderItem.cs
│       ├── Payment.cs
│       ├── Product.cs
│       ├── ProductImage.cs
│       └── VendorProfile.cs
└── MarketHub.MVC/
    ├── Controllers/
    ├── ViewModels/
    ├── Views/
    │   ├── Home/
    │   └── Shared/
    ├── wwwroot/
    │   ├── css/
    │   ├── js/
    │   └── images/
    ├── appsettings.json
    └── Program.cs
```

> هذه الشجرة ملخص للهيكل الرئيسي وليست حصرًا آليًا لكل الملفات. حدّثها لو
> تغيّر الهيكل الفعلي.

### أهم مفاهيم البيانات

-   `ApplicationUser`: حساب المستخدم.
-   `VendorProfile`: بيانات البائع أو المتجر وحالة الموافقة.
-   `Category`: تصنيف المنتج.
-   `Product` و`ProductImage`: المنتج وصوره.
-   `CartItem`: المنتج وكميته داخل السلة.
-   `Order` و`OrderItem`: الطلب والعناصر التي تم شراؤها.
-   `Payment`: سجلات الدفع.
-   `CommissionSetting` و`CommissionRecord`: إعدادات العمولة وسجلاتها.

الأعمدة والعلاقات والقيود الفعلية موجودة في الـ Entities وإعدادات EF
Core والـ Migrations.

## 8. قاعدة البيانات وEF Core

-   `AppDbContext` هو نقطة التعامل مع قاعدة البيانات.
-   ملفات Configurations تضبط العلاقات والـ Mapping.
-   الـ Migrations تتبع تغييرات تصميم قاعدة البيانات.
-   يمكن استخدام `AsNoTracking()` لاستعلامات القراءة فقط عندما يكون
    مناسبًا.
-   يجب التحقق من المدخلات وقواعد العمل قبل حفظ التغييرات.

## 9. تشغيل المشروع

### المتطلبات

1.  تثبيت Git.
2.  تثبيت إصدار .NET SDK المطلوب في ملفات المشروع.
3.  تجهيز SQL Server.
4.  استخدام Visual Studio 2022 أو محرر متوافق.

### تحميل المشروع

``` bash
git clone https://github.com/mahmoudabdelbakey/Multi-Vendor_E-Commerce_Marketplace_MarketHub-MVC.git
cd "Multi-Vendor E-Commerce Marketplace (MarketHub-MVC)"
```

إذا كان اسم المجلد مختلفًا، ادخل إلى المجلد الذي يحتوي على
`MarketHub.slnx`.

### إعداد قاعدة البيانات

1.  افتح إعدادات Connection String التي يستخدمها `MarketHub.MVC`.
2.  ضع اتصال SQL Server المحلي.
3.  لا ترفع كلمات المرور أو مفاتيح الخدمات إلى GitHub.
4.  تأكد أن SQL Server يعمل.

اسم مفتاح الاتصال يجب أن يطابق ما يقرأه `Program.cs` أو `AppDbContext`.

### Restore وBuild

من المجلد الذي يحتوي على ملف الحل:

``` bash
dotnet restore
dotnet build
```

### تطبيق الـ Migrations

حسب إعدادات المشروع، قد يكون الأمر بالشكل التالي:

``` bash
dotnet ef database update --project MarketHub.DAL --startup-project MarketHub.MVC
```

إذا لم يتوفر `dotnet ef`:

``` bash
dotnet tool install --global dotnet-ef
```

تأكد من توافق إصدار الأداة مع حزم EF Core. لو الأمر فشل، راجع مسارات
المشاريع وإعدادات Startup Project والـ Migrations وإصدار SDK؛ لا تنشئ
Migration عشوائيًا لحل مشكلة اتصال.

### تشغيل التطبيق

``` bash
dotnet run --project MarketHub.MVC
```

افتح الرابط الذي يظهر في Terminal. في Visual Studio افتح
`MarketHub.slnx` واجعل `MarketHub.MVC` مشروع التشغيل.

> الأوامر نقطة بداية؛ راجع إعدادات المشروع والمراجع بين المشاريع قبل
> استخدامها دون تعديل.

## 10. ملاحظات الأمان

-   لا ترفع كلمات مرور قاعدة البيانات أو مفاتيح Stripe أو أي Secrets إلى
    GitHub.
-   استخدم .NET User Secrets أو Environment Variables محليًا.
-   تحقق من المدخلات على الخادم.
-   طبّق الصلاحيات على الخادم؛ إخفاء زر من الواجهة لا يكفي.
-   تحقق من ملكية البائع قبل الوصول إلى منتجاته أو طلباته.
-   احمِ عمليات تعديل البيانات من CSRF عند الحاجة.
-   تأكد من الدفع من خلال التدفق الموثوق على الخادم والـ Webhook، وليس
    من إعادة التوجيه في المتصفح فقط.
-   استخدم Stripe Test Mode أثناء التطوير.

## 11. التعاون بين أعضاء الفريق

1.  اسحب آخر تغييرات الفرع المشترك.
2.  أنشئ Branch صغيرة للميزة أو الإصلاح.
3.  نفّذ التغيير والاختبارات المرتبطة به.
4.  نفّذ Build واختبر محليًا.
5.  افتح Pull Request مع شرح وخطوات الاختبار.
6.  راجع التغييرات وحل التعارضات قبل الدمج.
7.  حدّث التوثيق عند تغيير التصميم أو السلوك.

أمثلة Branches:

``` text
feature/category-list
feature/home-page
fix/cart-stock-validation
docs/update-readme
```

أمثلة Commit Messages:

``` text
feat(catalog): add category listing
fix(cart): validate stock before checkout
docs: update project README
```

## 12. الاختبارات

إذا كان المشروع يحتوي على Test Project:

``` bash
dotnet test
```

من الحالات المهمة: منع الوصول غير المصرح، عزل بيانات البائعين، التحقق من
السعر والمخزون، حساب إجمالي الطلب على الخادم، ومعالجة الـ Webhooks
بأمان. وثّق مشاريع الاختبارات والتغطية بعد التحقق منها فعليًا.

## 13. التوثيق

تغطي حزمة التوثيق المتطلبات وUser Stories وUse Cases وتصميم قاعدة
البيانات وERD ومخططات المعمارية والـ Classes وتدفقات عمل البائع والشراء
والدفع والـ Webhooks وخطة التعاون. راجع ملفات التوثيق والرسومات داخل
المستودع للتفاصيل.

## 14. خارطة الطريق

تابع الحالة الفعلية من خلال GitHub Issues والـ Project Board والكود.

-   [ ] تجهيز الـ Solution وقاعدة البيانات
-   [ ] تسجيل الدخول والصلاحيات
-   [ ] طلبات البائعين وموافقة الإدارة
-   [ ] التصنيفات وقائمة المنتجات العامة
-   [ ] إدارة المنتجات والتحقق من المخزون
-   [ ] سلة المشتريات
-   [ ] Checkout ودورة حياة الطلب
-   [ ] Stripe Test Mode والـ Webhooks
-   [ ] حساب العمولات والتقارير
-   [ ] لوحات تحكم البائع والإدارة
-   [ ] الاختبارات ومراجعة الأمان والتجهيز للعرض أو النشر

لا تعتبر أي مرحلة مكتملة إلا بعد تنفيذها والتحقق منها.

## 15. الترخيص

لا يحدد هذا الملف ترخيصًا للمشروع. أضف ملف `LICENSE` وحدّث هذا القسم بعد
اتفاق أصحاب المشروع على الترخيص.

------------------------------------------------------------------------

::: {align="center"}
**MarketHub --- One Marketplace, Multiple Vendors.**\
**MarketHub --- منصة واحدة، وبائعون متعددون.**
:::
