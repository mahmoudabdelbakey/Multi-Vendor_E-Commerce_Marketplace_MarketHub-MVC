# 8. معمارية Stripe والنظام المالي (Stripe and Financial Architecture)

## 8.1 أربع حاجات لازم نفرّق بينهم

| # | الموضوع | مين بيعمله؟ | في الـ MVP؟ |
|---|---|---|---|
| 1 | **استقبال الدفع** من العميل | Stripe (Checkout) بيستقبل ويخصم من الكارت | نعم (Test Mode) |
| 2 | **تسجيل العمولة** | تطبيقنا في جدول `CommissionRecords` | نعم |
| 3 | **حساب صافي الـ Vendor** (`VendorNetAmount`) | تطبيقنا (حساب محاسبي) | نعم |
| 4 | **تحويل الفلوس فعلياً للـ Vendors** | Stripe Connect (Transfers/Payouts) | **لا**: مرحلة مستقبلية منفصلة |

> **تنبيه مهم:** Stripe Checkout العادي **مبيوزعش** الفلوس تلقائياً على أكتر من Vendor. الفلوس كلها بتروح لحساب Stripe بتاع المنصة. الأرقام اللي بنحسبها (العمولة وصافي الـ Vendor) هي **سجلات محاسبية** في الـ Database بتوضح المستحق لكل Vendor، لكن مفيش تحويل حقيقي في الـ MVP.

## 8.2 ليه Stripe Checkout (Hosted) بدل بناء فورم كارت؟

- بيانات الكارت بتتكتب على صفحة Stripe، **مبتعديش على سيرفرنا**، فبنقلل عبء الـ PCI Compliance.
- نقدر نتجاهل UI الكارت ونركز على الـ Business Logic.
- Stripe بيدير الـ 3D Secure وطرق الدفع المتأخرة (ولهذا بنتعامل مع `async_payment_succeeded`).
- خيار `PaymentIntent` مع Stripe Elements أقوى في التخصيص لكنه أعقد في الـ Frontend، وده مش مناسب للـ MVP [Recommendation].

## 8.3 الـ Workflow الكامل (11 خطوة)

مطابق للـ Diagrams 14 و 15 و 17:

1. **Customer** يضغط Checkout ويدخل بيانات الشحن.
2. **السيرفر يتحقق** من الأسعار والمخزون: يعيد تحميل كل منتج من SQL Server ويتأكد إنه Active وإن الـ Vendor Approved وإن المخزون كافي.
3. **السيرفر ينشئ** `Order` بحالة `PendingPayment` + `OrderItems` (Snapshots) + `Payment` بحالة `Pending`، ويحجز المخزون، **كل ده في Transaction واحدة**.
4. **السيرفر ينادي Stripe** لإنشاء Checkout Session فيها `line_items` من الـ Order (مش من الـ Cart) و`metadata.orderId` و`expires_at`.
5. العميل يكمل الدفع في صفحة Stripe بكارت تجريبي.
6. **Stripe يبعت Webhook** (`checkout.session.completed`).
7. **التطبيق يتحقق من التوقيع** بالـ raw body وسر الـ Endpoint.
8. **معالجة Idempotent:** تسجيل `StripeEventId` في نفس الـ Transaction (Unique).
9. **تحديث آمن:** `Payment = Succeeded` و`Order = Paid` (بعد التأكد إن المبلغ والعملة مطابقين).
10. **المخزون والعمولة:** المخزون اتحجز بالفعل في الخطوة 3 فمفيش خصم هنا؛ بننشئ `CommissionRecords` داخل نفس الـ Transaction. وفي حالة الفشل/الانتهاء بنرجّع المخزون.
11. العميل والـ Vendor يشوفوا الطلب: العميل في `/Orders`، والـ Vendor في `/Vendor/Orders` (Items بتاعته فقط).

### استراتيجية المخزون والـ Transaction [Recommendation]

| الخيار | الميزة | العيب |
|---|---|---|
| **A) Reserve عند إنشاء الـ Order (المختار)** | مفيش Overselling، والـ Webhook بسيط | لازم نرجّع المخزون لو الدفع ما تمّش |
| B) خصم بعد الدفع | مفيش حجز لفلوس لسه ما اتدفعتش | ممكن اتنين يدفعوا لآخر قطعة والتاني يتدفع له ويتعذر تسليمه |

المخزون المحجوز بيرجع في 3 حالات: `checkout.session.expired`، `async_payment_failed`، أو إلغاء العميل. ولو Webhook الـ expired اتأخر أو ضاع، **Background Job** بسيط (`BackgroundService`) كل 10 دقايق يلغي الـ Orders اللي `PendingPayment` من أكتر من (مدة الـ Session + هامش) ويرجّع مخزونها، كشبكة أمان [Recommendation].

> **حالة إلغاء العميل (UC-11):** لو العميل ألغى Order لسه `PendingPayment` وفيه Checkout Session مفتوحة، لازم **ننهي الـ Session من ناحية Stripe أولاً** (Expire Session API) وبعدين نلغي الـ Order ونرجّع المخزون، وإلا ممكن العميل يدفع بعد الإلغاء. لو الـ Session اتدفعت فعلاً وقت الإلغاء، الـ Webhook هو اللي يحسم الموقف. [Team Decision: تأكدوا من اسم الـ Method في Stripe.net وسلوكها من الـ API Reference.]

## 8.4 الكود الأساسي

> **[Team Decision]:** أسماء Classes والـ Properties في `Stripe.net` بتتغير بين الإصدارات. الأمثلة دي للتوضيح، **راجعوا الـ API Reference الرسمي للنسخة المثبّتة** قبل الاعتماد عليها.

### حجز المخزون بشكل آمن (Conditional UPDATE)

```csharp
foreach (var line in orderLines)
{
    var rows = await _db.Products
        .Where(p => p.Id == line.ProductId && p.IsActive && p.StockQuantity >= line.Quantity)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockQuantity, p => p.StockQuantity - line.Quantity));

    if (rows == 0)
    {
        await tx.RollbackAsync();
        return Result.Fail($"Insufficient stock for {line.Name}.");
    }
}
```

الـ `UPDATE ... WHERE StockQuantity >= qty` عملية واحدة ذرّية (Atomic) في SQL Server، فلو اتنين اشتروا آخر قطعة في نفس اللحظة، واحد بس هيحصل عنده `rows = 1`.

### إنشاء Checkout Session

```csharp
var options = new SessionCreateOptions
{
    Mode = "payment",
    ClientReferenceId = order.Id.ToString(),
    SuccessUrl = $"{baseUrl}/Checkout/Success?orderId={order.Id}",
    CancelUrl = $"{baseUrl}/Checkout/Cancel?orderId={order.Id}",
    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
    Metadata = new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
    LineItems = order.Items.Select(i => new SessionLineItemOptions
    {
        Quantity = i.Quantity,
        PriceData = new SessionLineItemPriceDataOptions
        {
            Currency = order.Currency.ToLowerInvariant(),
            UnitAmount = (long)Math.Round(i.UnitPrice * 100m), // Stripe uses the smallest currency unit
            ProductData = new SessionLineItemPriceDataProductDataOptions { Name = i.ProductNameSnapshot }
        }
    }).ToList()
};
var session = await new SessionService().CreateAsync(options);
payment.StripeCheckoutSessionId = session.Id;
await _db.SaveChangesAsync();
return session.Url;
```

الـ `line_items` مبنية من `OrderItems` اللي اتحسبت من الـ Database، **مش من الـ Cart ولا من الـ Browser**.

### Webhook Controller

```csharp
[AllowAnonymous]
[IgnoreAntiforgeryToken]
[Route("stripe/webhook")]
public class StripeWebhookController : Controller
{
    private readonly IStripeWebhookService _service;
    private readonly ILogger<StripeWebhookController> _logger;
    public StripeWebhookController(IStripeWebhookService service, ILogger<StripeWebhookController> logger)
        => (_service, _logger) = (service, logger);

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        // The raw body is REQUIRED: signature verification fails if the payload is parsed and re-serialized.
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();
        try
        {
            await _service.HandleAsync(json, signature);
            return Ok();
        }
        catch (StripeException ex)   // includes signature verification failures
        {
            _logger.LogWarning(ex, "Stripe webhook rejected.");
            return BadRequest();
        }
    }
}
```

### Webhook Service (Signature + Idempotency + Transaction)

```csharp
public async Task HandleAsync(string json, string signature)
{
    var stripeEvent = EventUtility.ConstructEvent(json, signature, _options.WebhookSecret); // throws if invalid

    await using var tx = await _db.Database.BeginTransactionAsync();

    // Idempotency: the UNIQUE index on StripeEventId rejects a second insert.
    _db.StripeWebhookEvents.Add(new StripeWebhookEvent { StripeEventId = stripeEvent.Id, EventType = stripeEvent.Type });
    try { await _db.SaveChangesAsync(); }
    catch (DbUpdateException)  // duplicate event (already processed)
    {
        _logger.LogInformation("Duplicate Stripe event {EventId} ignored.", stripeEvent.Id);
        return; // respond 200 with no side effects
    }

    var session = stripeEvent.Data.Object as Session;
    switch (stripeEvent.Type)
    {
        case "checkout.session.completed" when session?.PaymentStatus == "paid":
        case "checkout.session.async_payment_succeeded":
            await MarkPaidAsync(session!);        // Payment, Order, CommissionRecords
            break;
        case "checkout.session.expired":
        case "checkout.session.async_payment_failed":
            await CancelAndReleaseStockAsync(session!);
            break;
    }
    await _db.SaveChangesAsync();
    await tx.CommitAsync();
}
```

> الـ `catch (DbUpdateException)` هنا واسع للتوضيح. في الكود الحقيقي افحصوا إن السبب هو **Unique Constraint Violation** (SQL Server Error 2601/2627) علشان منبلعش أخطاء تانية.

داخل `MarkPaidAsync`: نحمّل الـ Payment بـ `StripeCheckoutSessionId`، ولو الـ Order `Paid` بالفعل نخرج، ونتأكد إن `session.AmountTotal` يساوي `Payment.Amount × 100` وإن العملة مطابقة، ثم نحدّث الحالات وننادي `CommissionService.CreateForOrderAsync`.

### مثال حساب العمولة

بفرض النسبة 10%، والـ Order فيه Item من Vendor X (قيمة 200.00) وItem من Vendor Y (قيمة 55.55):

| Item | Gross | Rate | Commission | Vendor Net |
|---|---|---|---|---|
| Vendor X | 200.00 | 10.00% | 20.00 | 180.00 |
| Vendor Y | 55.55 | 10.00% | 5.56 (5.555 تتقرب لأعلى بـ AwayFromZero) | 49.99 |
| **الإجمالي** | **255.55** | | **25.56** | **229.99** |

## 8.5 حالات الفشل ومخاطر الاتساق (Consistency Risks)

| الحالة | الخطر | المعالجة |
|---|---|---|
| العميل دفع وقفل المتصفح قبل الـ Redirect | الـ Order يفضل `PendingPayment` | الـ Webhook هو المصدر الحقيقي، فالـ Order بيتحدث برضه |
| الـ Redirect رجع `Success` لكن الدفع لسه متأكدش | لو اعتمدنا عليه نبيع من غير تأكيد | `Success` تعرض "قيد التأكيد" فقط |
| Webhook اتبعت مرتين | عمولات مكررة، مخزون مكرر | `StripeWebhookEvent` Unique + فحص `Order.Status` |
| Webhook اتأخر أو الـ Events وصلت بترتيب مختلف | تحديث على حالة قديمة | نفحص الحالة الحالية قبل أي انتقال |
| السيرفر وقع بين تحديث `Payment` و`Order` | حالة نص-نص | كل حاجة داخل Transaction واحدة؛ لو فشل Stripe هيعيد الإرسال |
| فشل الاتصال بـ Stripe بعد Commit الـ Order | Order معلّق ومخزون محجوز | نحاول إلغاءه فوراً، وإلا الـ Background Job ينظّفه |
| مبلغ الـ Session ≠ مبلغ الـ Order | تلاعب أو Bug | نرفض التأكيد ونسجّل Alert |
| فشل الكارت | العميل يعيد المحاولة | Checkout بيسمح بإعادة المحاولة داخل نفس الـ Session |

## 8.6 قواعد الأمان في الدفع

- **ممنوع** نخزّن رقم كارت أو CVC في أي مكان (Database, Logs).
- **ممنوع** نرفع `SecretKey` أو `WebhookSecret` على GitHub. نستخدم `dotnet user-secrets` محلياً وEnvironment Variables على الاستضافة. لو مفتاح اتسرّب، نغيّره (Roll) من Stripe Dashboard فوراً.
- الـ Webhook Endpoint لازم يكون HTTPS في Production.
- نستخدم **Test Mode** فقط في الـ MVP. كروت الاختبار موجودة في صفحة Stripe Testing الرسمية.

## 8.7 تجربة الـ Webhooks محلياً

```bash
stripe login
stripe listen --forward-to https://localhost:7001/stripe/webhook
# The CLI prints a whsec_... secret: put it in user-secrets as Stripe:WebhookSecret
stripe trigger checkout.session.completed
```

**ملحوظة:** الـ `whsec_` اللي بيطلع من `stripe listen` مختلف عن سر الـ Endpoint المسجّل في الـ Dashboard. استخدموا كل واحد في بيئته.

## 8.8 Stripe Connect كمرحلة مستقبلية

**إيه هو Stripe Connect؟** منتج من Stripe بيسمح للمنصات (زينا) إنها تستقبل مدفوعات وتوزّعها على أطراف تانية (Vendors) بشكل مرخّص ومتوافق مع القوانين.

| الموضوع | الشرح |
|---|---|
| **Connected Accounts** | كل Vendor ليه حساب Stripe مرتبط بحساب المنصة. |
| **Vendor Onboarding** | الـ Vendor بيعمل Onboarding (هوية، حساب بنكي) عن طريق صفحات Stripe المستضافة، والمنصة بتتابع حالة الحساب (هل يقدر يستقبل Payouts). |
| **Transfers و Platform Fees** | المنصة ممكن تحوّل جزء من الفلوس لكل Connected Account وتخصم Application Fee (عمولتنا). التفاصيل بتختلف حسب نوع الـ Charge (Destination / Separate Charges and Transfers). |
| **Country eligibility** | Connect متاح في دول معينة، وحساب المنصة وحسابات الـ Vendors لازم يكونوا في دول مدعومة. |
| **Compliance** | KYC، شروط الخدمة، مسؤولية Refunds والـ Disputes، وربما التزامات ضريبية وقانونية. |
| **Testing إضافي** | سيناريوهات Onboarding ناقص، Payouts معلّقة، Refunds، وWebhooks خاصة بـ Connect. |
| **أثر على التصميم** | هنحتاج `StripeAccountId` في `VendorProfile`، وربما `VendorOrder` و`Payout`، وربط `CommissionRecord` بـ Transfer. |

> **تحذير:** قبل ما الفريق يخطط لدفعات حقيقية **لازم يتحقق من وثائق Stripe الحالية** (Connect) ومن الدول المدعومة لحساب المنصة وللـ Vendors، لأن الإتاحة والشروط بتتغير. وفر وقت لكتابة Use Case منفصل ومراجعة قانونية قبل الإنتاج.

---pagebreak---
