# دليل Member A: إيه اللي اتعمل ومكانه فين وإزاي تتأكد منه

> **تنبيه صريح:** الكود ده **ما اتعملوش Build** هنا (الـ NuGet بيرجّع 403 في البيئة اللي شغال فيها). عملت فحص Syntax بـ Roslyn ومفيش أخطاء كتابة، لكن الـ Types بتاعة EF Core وIdentity ما اتفحصتش. أول خطوة عندك: `dotnet build` وابعتلي أي خطأ يظهر.

## 1) الأهم: لقيت في الـ DAL بتاعك نقص

الـ DAL اللي بعتّه فيه الـ Entities والـ Enums والـ DbContext والـ Migration، **لكن مفيش ولا Configuration** (دالة `ApplyConfigurationsFromAssembly` بتدوّر على Configurations ومش لاقية حاجة). ده معناه إن الـ Migration اللي عندك اتعملت بالإعدادات الافتراضية بس:

| المشكلة في الـ Migration القديمة | النتيجة |
|---|---|
| `StripeEventId` نوعه `nvarchar(max)` وبدون Unique | الـ Webhook ممكن يتنفّذ مرتين = دفع مكرر |
| `RowVersion` نوعه `varbinary(max)` عادي | التعديل المتزامن (RowVersion) مش بيشتغل |
| `StoreName` وSlug وOrderNumber مفيش عليهم Unique | اسمين متجر متطابقين، أو رقم طلب مكرر |
| `RatePercentSnapshot` نوعه `decimal(18,2)` | نسبة العمولة تكفيها `decimal(5,2)` |
| كل الـ Enums بتتخزن أرقام | الأرقام صعبة القراءة في الداتابيز |
| مفيش Check Constraints (سعر > 0، مخزون >= 0، LineTotal = السعر × الكمية) | الداتابيز بتقبل بيانات غلط |
| كل الـ Foreign Keys `Cascade` | مسح مستخدم ممكن يمسح تاريخ مالي |

**الحل (TASK-01):** ضفت الـ Configurations ناقصة في `src/MarKetHub.DAL/Data/Configurations/` (12 ملف) و`ApplicationUserConfiguration.cs`. الـ Entities والـ Enums والـ DbContext **ما لمستهمش**.

## 2) TASK-02: الـ Migration (لازم تعملها إنت)

مقدرتش أعملها لأن `dotnet ef` محتاج NuGet. اعمل كده من مجلد الـ Solution:

1. امسح مجلد `Data/Migrations` القديم من الـ DAL بتاعك (الـ zip بتاعي مفيهوش Migrations).
2. لو الداتابيز موجودة: `dotnet ef database drop --force --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC`
3. `dotnet ef migrations add InitialCreate --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC --output-dir Data/Migrations`
4. `dotnet ef database update --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC`

**للتأكد:** افتح SSMS أو SQL Server Object Explorer: جدول `StripeWebhookEvents` عنده Unique Index على `StripeEventId`، وجدول `Products` عمود `RowVersion` نوعه `rowversion` (بيظهر `timestamp` في SSMS).

## 3) الملفات والتحقق، مهمة بمهمة

| المهمة | اتعمل فين بالظبط | إزاي تجرّبها بإيدك |
|---|---|---|
| **TASK-01** تجميد الـ Entities والـ Configurations | `MarKetHub.DAL/Data/Configurations/*.cs` + `Data/ApplicationUserConfiguration.cs` | الخطوة 2 فوق (الـ Unique والـ rowversion) |
| **TASK-02** أول Migration | أنت بتعملها (الأوامر فوق) | `dotnet ef database update` ينجح |
| **TASK-03** الـ Contracts | `MarKetHub.BLL/Interfaces/IServices.cs` + `Models/*.cs` + `Common/Result.cs` + `Common/PagedResult.cs` + `Interfaces/ICurrentVendorService.cs` | `dotnet build` ينجح. **الفريق لازم يراجعها ويتفق عليها قبل التجميد** (المهمة مشتركة) |
| **TASK-04** Shared Seed Data | `MarKetHub.DAL/Data/DbSeeder.cs`: الأدمن، تصنيفات، بائعين Approved، بائع Pending، عميل، 12 منتج، **طلب Delivered وطلب PendingPayment** (`MH-SEED-DELIVERED` و`MH-SEED-PENDING`) | شغّل المشروع، وافتح جدول `Orders` فيه الطلبين، و`CommissionRecords` فيه سجلين |
| **TASK-05** الهيكل والـ DI | `MarKetHub.MVC/Infrastructure/Features/` (5 ملفات، ملف لكل شخص) + سطر لكل واحد في `Program.cs` | كل واحد يفتح ملفه ويسجّل خدماته هناك بس |
| **TASK-06** الـ Validation والـ Git | `docs/CONTRIBUTING.md` (مشترك: اتفقوا عليه) | اقروه مع الفريق في أول اجتماع |
| **TASK-07** Roles وIdentity | `Infrastructure/Features/IdentityFeature.cs` + `MarKetHub.DAL/Constants/Roles.cs` | سجّل بباسورد `abc` يرفض. باسورد 8 حروف بدون حرف كبير يرفض. جرّب 5 باسوردات غلط = قفل 10 دقايق |
| **TASK-08** Register/Login/Logout/AccessDenied | `Controllers/AccountController.cs` + `Views/Account/*` + `ViewModels/AccountViewModels.cs` | سجّل حساب جديد = دور Customer. افتح `/Account/Login?returnUrl=https://evil.com` وسجّل دخول: لازم ما يروحش لـ evil.com |
| **TASK-09** الـ Seeder | `MarKetHub.DAL/Data/DbSeeder.cs` | شغّل المشروع مرتين: مفيش تكرار في الجداول. الأدمن من الـ user-secrets مش من الكود |
| **TASK-10** Program.cs والأمان | `MarKetHub.MVC/Program.cs` | افتح `/Orders` وانت مش مسجّل = يحوّلك للـ Login (الموقع مقفول افتراضياً). افتح DevTools Network وشوف Headers (`X-Frame-Options`...) |
| **TASK-11** الـ Policies المبدئية | `Authorization/PolicyNames.cs` + الـ Policies داخل `IdentityFeature.cs` | في Week 2 (TASK-30) هتتحوّل لفحص الداتابيز |
| **TASK-12** الـ Layout و`ICurrentVendorService` | `Views/Shared/_Layout.cshtml` + `Infrastructure/CurrentVendorService.cs` | سجّل بـ `vendor1@demo.test`: القايمة فيها Vendor. سجّل بعميل: ملهوش قايمة Vendor |

## 4) إزاي تشغّل لأول مرة

```bash
dotnet user-secrets set "Seed:AdminEmail" "admin@markethub.test" --project src/MarKetHub.MVC
dotnet user-secrets set "Seed:AdminPassword" "Admin@12345" --project src/MarKetHub.MVC
dotnet run --project src/MarKetHub.MVC --launch-profile https
```

حسابات التجربة (Development بس، باسورد `Demo@12345`): `customer@demo.test`، `vendor1@demo.test`، `vendor2@demo.test`، `pending@demo.test`.

## 5) ملاحظات لازم تعرفها

1. **التسمية:** مشروعك اسمه `MarKetHub.DAL` (K كبيرة) والـ Enums في Namespace `MarKetHub.DAL.Enums`، بينما الـ Entities والـ DbContext في `MarketHub.DAL...`. سيبتها زي ما هي عشان ما أكسرش الـ Migration بتاعتك، وكل كودي بيستخدم الاتنين صح. لو عايز تنضّفها في نفس الأسبوع، اعملها كلها مرة واحدة قبل الـ Migration.
2. **الطلب المعلّق في الـ Seed:** `PendingOrderCleanupService` (مهمة D في Week 2) هيلغيه بعد فترة، فلو عايز تجرّب الـ Webhook عليه من تاني امسح الداتابيز وشغّل المشروع.
3. **Home وصفحات الخطأ:** عملت `HomeController` بسيط جداً عشان الموقع يفتح ومسار الأخطاء يشتغل. ده مؤقت وB هيستبدله.
4. **`ICurrentVendorService`:** بيقرا الداتابيز مباشرة من الـ MVC دلوقتي. في Week 2 بعد `VendorService` بنحوّله يعدّي عليها (ملاحظة في الكود).
5. **اللي مش في مهام A ولسه ما اتعملش:** طلب البائع وموافقة الأدمن وسياسة الـ ApprovedVendor الحقيقية (Week 2)، ومراجعة الأمان (Week 3).
6. **تعمّدت في الـ Register:** مفيش حقل Role أبداً، عشان محدش يقدر يبعت دور بنفسه (Over-posting).
