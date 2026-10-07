# 13. الأمان والموثوقية (Security and Reliability)

كل بند هنا بيتحول لـ Checklist بنراجعها في Sprint 8 (Phase 22)، وبعضها بيتطبق من أول Sprint.

| # | الخطر | الشرح بالعربي ومثال في MarketHub | الحل |
|---|---|---|---|
| 1 | **Authentication / Authorization** | لو Route من غير `[Authorize]` أي حد يوصله. مثال: `/Admin/Commission/Edit` مفتوح. | `[Authorize]` على `VendorBaseController` وكل Admin Controller، وTests (TC-01, TC-14)، و`FallbackPolicy` تطلب Login ما لم يُكتب `[AllowAnonymous]` [Recommendation]. |
| 2 | **Vendor Data Isolation (IDOR)** | تغيير `id` في الـ URL للوصول لبيانات Vendor تاني. | فلترة بـ `VendorProfileId` في كل Query + 404 موحد (انظر 3.4، TC-02/03). |
| 3 | **Server-Side Validation** | الـ Client Validation بتتجاوز بسهولة (Postman). | Data Annotations + Business Validation في الـ Services، ومفيش ثقة في أي قيمة من الـ Browser. |
| 4 | **Overposting (Mass Assignment)** | إرسال `Price` أو `VendorProfileId` أو `Status` مش مقصود إنه يتعدّل. | ViewModels مخصصة لكل Action، ومفيهاش الحقول الحساسة؛ ولا `[Bind]` على الـ Entities. |
| 5 | **CSRF** | موقع خبيث يخلّي المتصفح بتاعك يبعت POST (مثلاً Approve Vendor). | `[ValidateAntiForgeryToken]` (أو `AutoValidateAntiforgeryTokenAttribute` global) على كل POST، و`asp-action` Tag Helpers بتضيف الـ Token تلقائياً. الـ Webhook استثناء ومحمي بالتوقيع. |
| 6 | **Secure File Uploads** | ملف `.exe` أو `.svg` فيه Script بيتحط كصورة. | Whitelist للامتدادات (jpg/png/webp)، فحص الـ Content-Type وحجم الملف، **اسم ملف جديد (GUID) من السيرفر**، تخزين تحت `wwwroot/uploads` (أو Storage خارجي) بدون تنفيذ، ومنع Path Traversal (`Path.GetFileName`). |
| 7 | **SQL Injection** | دمج مدخلات المستخدم في Query كنص. | EF Core/LINQ بتستخدم Parameters. لو احتجنا SQL خام: `FromSqlInterpolated`، **وممنوع `FromSqlRaw` بتجميع Strings**. |
| 8 | **Password Security** | تخزين Passwords أو سياسة ضعيفة. | Identity بيعمل Hashing؛ نضبط الطول 8+، Lockout، ولا نسجل Passwords في Logs. |
| 9 | **Secrets Management** | مفتاح Stripe على GitHub = خسارة مالية. | `user-secrets` محلياً، Environment Variables على الاستضافة، Secret Scanning، Roll عند التسريب (11.6). |
| 10 | **Stripe Webhook Verification** | أي حد يبعت POST ويقول "الدفع تم". | التحقق من `Stripe-Signature` بالـ raw body وسر الـ Endpoint، ورفض أي حاجة غير موقّعة (TC-10b). |
| 11 | **Idempotency** | Stripe بيعيد الإرسال (At-least-once)، فنكرر العمولات. | `StripeWebhookEvent` Unique + فحص حالة الـ Order (8.4، TC-10). |
| 12 | **Transaction Management** | فشل في نص العملية يسيب بيانات متناقضة. | `BeginTransactionAsync` حوالين: إنشاء الـ Order + الحجز، ومعالجة الـ Webhook. |
| 13 | **Inventory Consistency** | اتنين يشتروا آخر قطعة. | Conditional `UPDATE` + `RowVersion` + TC-06b. |
| 14 | **Error Handling** | Stack Trace ظاهر للمستخدم بيكشف معلومات. | `UseExceptionHandler` + صفحة Error عامة في Production، و`UseDeveloperExceptionPage` في Development فقط. |
| 15 | **Logging** | غياب Logs = مفيش تحقيق، وLogs فيها بيانات حساسة = تسريب. | `ILogger` لـ: رفض Webhook، 403 متكرر، فشل Transaction. ممنوع تسجيل Secrets أو بيانات بطاقات أو Passwords. |
| 16 | **Open Redirect** | `returnUrl` يوجّه لموقع خارجي بعد Login. | `Url.IsLocalUrl(returnUrl)` قبل `Redirect`. |
| 17 | **Cookie / HTTPS / Headers** | سرقة الـ Session على HTTP. | HTTPS + HSTS، `Secure` و`HttpOnly` Cookies (الافتراضي في Identity)، ومراجعة Security Headers (CSP/X-Content-Type-Options) [Team Decision]. |

## مثال عملي: لماذا نرفض السعر القادم من الـ Browser؟

لو فورم الـ Checkout فيه `<input type="hidden" name="price" value="500">` والـ Controller بيستخدمها، أي مستخدم يفتح Developer Tools ويخليها `1`، وبكده اشترى بـ 1. الحل ببساطة إن مفيش Input للسعر أصلاً، والـ Service بتجيب `Product.Price` من SQL Server.

## مثال عملي: الـ Idempotency

Stripe ممكن يبعت `checkout.session.completed` نفسه مرتين (مثلاً لأن ردنا اتأخر). من غير الحماية: Commission مرتين ومخزون متحرر مرتين. مع جدول `StripeWebhookEvent` الـ Insert التاني بيفشل بسبب الـ Unique، فبنرجّع 200 وما بنعملش حاجة.

---pagebreak---
