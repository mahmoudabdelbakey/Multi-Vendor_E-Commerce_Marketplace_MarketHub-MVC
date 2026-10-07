# 14. النشر والتسليم النهائي (Deployment and Final Delivery)

## 14.1 ثلاث بيئات مختلفة لازم نفرّق بينها

| الجانب | Local Development | Stripe Test Mode (على Demo Online) | Production حقيقي (مستقبلي) |
|---|---|---|---|
| الغرض | تطوير واختبار | عرض للجنة التقييم | عملاء حقيقيين |
| Stripe Keys | `sk_test_...` في user-secrets | `sk_test_...` في Environment Variables | `sk_live_...` (بعد التحقق من الحساب والامتثال) |
| Webhook | `stripe listen` (سر مؤقت `whsec_` من الـ CLI) | Endpoint مسجّل في Dashboard بـ HTTPS عام | Endpoint مخصص بسر مختلف |
| فلوس حقيقية | لا | **لا** | نعم، ومحتاج مراجعة قانونية ومالية |
| Database | LocalDB/Express | SQL Server مستضاف (بيانات Demo) | SQL Server مُدار مع Backups |
| Seed Data | Demo كامل | Demo Accounts محدودة | Roles + Admin فقط |
| Stripe Connect Payouts | غير موجود | غير موجود | مرحلة مستقبلية منفصلة (8.8) |

> الـ Demo Online **مش Production**: بياخد فلوس وهمية فقط. لو قررنا نستقبل دفع حقيقي لازم نتحقق من: دولة حساب Stripe، شروط الخدمة، الضرائب، وسياسة الخصوصية.

## 14.2 خيارات الاستضافة [Team Decision]

مفيش خيار واحد صح، ولا نفترض إن مزوّد معين متاح أو مجاني. قارنوا الخيارات الحالية من مواقعها الرسمية، وتأكدوا من الأسعار والـ Free Tiers وقت التنفيذ.

| الخيار | ملاحظات للمقارنة |
|---|---|
| Azure App Service + Azure SQL | تكامل قوي مع .NET وSQL Server، راجعوا الأسعار وبرامج الطلاب (لو متاحة لكم). |
| استضافة Windows/IIS (Shared/VPS) | مناسبة لو متاح عندكم، محتاجة إعداد يدوي. |
| VPS Linux + Docker | مرونة، ومحتاج SQL Server Container أو مزوّد قاعدة بيانات تاني، وخبرة أعلى. |
| مزوّدين PaaS تانيين | تأكدوا من دعم .NET 10 وSQL Server والـ Environment Variables والـ HTTPS. |

**معايير الاختيار:** دعم .NET 10، دعم SQL Server (مُدار أو Container)، HTTPS مجاني، Environment Variables، Logs، السعر.

## 14.3 خطوات النشر

1. **Production Configuration:** `ASPNETCORE_ENVIRONMENT=Production`، وApp Settings من Environment Variables (`ConnectionStrings__DefaultConnection`, `Stripe__SecretKey`, `Stripe__WebhookSecret`, `Seed__AdminEmail`, `Seed__AdminPassword`).
2. **SQL Server Hosting:** إنشاء Database وUser بأقل صلاحيات لازمة، وتفعيل الـ Firewall للـ Host فقط.
3. **Database Migrations:** توليد Script `dotnet ef migrations script --idempotent -o deploy.sql` وتطبيقه، أو أمر منفصل في الـ Deployment. [Team Decision: التطبيق التلقائي عند البدء مقبول للـ Demo فقط.]
4. **Application Deployment:** `dotnet publish -c Release`، ورفع المخرجات (أو نشر عبر GitHub Actions بعد الـ Merge).
5. **HTTPS:** تفعيل شهادة (غالباً مجانية من الاستضافة)، و`UseHsts` و`UseHttpsRedirection`.
6. **Stripe Webhook:** تسجيل `https://<your-domain>/stripe/webhook` في Stripe Dashboard (Test Mode)، واختيار الأحداث المطلوبة (`checkout.session.completed`, `checkout.session.async_payment_succeeded`, `checkout.session.async_payment_failed`, `checkout.session.expired`)، ونسخ الـ Signing Secret لـ Environment Variables.
7. **Uploads:** تأكد إن مكان الصور دائم (Persistent). على بعض الاستضافات الـ File System مؤقت، فممكن تحتاجوا Blob/Object Storage [Team Decision].
8. **Logging:** تفعيل Logs الاستضافة، ومراجعة الأخطاء بعد أول تشغيل.
9. **Error Handling:** تأكد إن صفحة الخطأ العامة بتظهر ومفيش Stack Trace.
10. **Smoke Test:** Register → Vendor Apply/Approve → Product → Cart → Checkout بكارت تجريبي → Webhook → Commission → Vendor Orders.
11. **Rollback Plan:** احتفظوا بآخر Build سليم وبـ Backup للـ Database قبل أي Migration.

## 14.4 Demo Accounts

| الدور | Email | ملاحظة |
|---|---|---|
| Super Admin | من `Seed:AdminEmail` | الباسورد في Environment Variables |
| Vendor 1 / 2 (Approved) | `vendor1@demo.test` / `vendor2@demo.test` | باسورد Demo ظاهر في README (حسابات تجريبية فقط) |
| Vendor (Pending) | `pending@demo.test` | لعرض Flow الموافقة |
| Customer | `customer@demo.test` | |

> **مهم:** حسابات الـ Demo باسوردها المعلن مبتتحطش في Production الحقيقي. الـ Seed بتاعها مشروط بـ Flag/Environment.

## 14.5 README النهائي و Screenshots و Demonstration

- **README:** (11.6) + Screenshots (Home, Product Details, Cart, Stripe Checkout Test, Vendor Dashboard, Admin Dashboard, Order History) + لينك Demo + Test Cards (من Stripe Testing Docs) + Diagrams.
- **Final Demonstration:** سكريبت مكتوب ومجرّب (الفصل 15)، مع Backup Video لو النت أو Stripe اتعطلوا.
- **Checklist قبل العرض:** Database Seeded، Webhook Endpoint شغال (Stripe Dashboard → Webhooks → Recent deliveries)، حسابات الـ Demo تدخل، Stripe Test Mode فعلاً مفعّل.

---pagebreak---
