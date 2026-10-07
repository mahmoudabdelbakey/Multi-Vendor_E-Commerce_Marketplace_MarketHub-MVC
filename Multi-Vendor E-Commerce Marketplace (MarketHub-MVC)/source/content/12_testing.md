# 12. استراتيجية الاختبار وضمان الجودة (Testing and QA)

## 12.1 أنواع الاختبارات ومكانها في المشروع

| النوع | بيختبر إيه؟ | الأداة | مثال |
|---|---|---|---|
| **Unit Testing** | Logic معزول من غير Database حقيقية | xUnit + (Moq أو NSubstitute اختياري) | `CommissionService` يحسب 10% صح |
| **Integration Testing** | Services مع Database فعلية | xUnit + SQL Server LocalDB أو Testcontainers [Team Decision]، أو SQLite In-Memory بحذر | `CheckoutService` ينشئ Order ويحجز المخزون |
| **MVC Functional Testing** | Request كامل عبر الـ Pipeline | `WebApplicationFactory<Program>` + `HttpClient` | `GET /Vendor/Products` بدون Login يرجع Redirect |
| **Manual Testing** | تجربة المستخدم والـ Flow | Test Scripts في Issue Templates | Checkout بكارت تجريبي |
| **Authorization Testing** | الصلاحيات (أهم نوع عندنا) | Functional Tests بـ Users مختلفين | Vendor A ضد منتج Vendor B |
| **Database Testing** | القيود والـ Indexes والـ Transactions | Integration على SQL Server حقيقي | `CHECK (Price > 0)` يرفض -5 |
| **Payment Testing** | Stripe في Test Mode والـ Webhooks | Stripe CLI (`stripe trigger`, `stripe listen`) + Unit Tests للـ Handler | Webhook مكرر |
| **Regression Testing** | إن القديم لسه شغال | تشغيل كل الـ Tests في كل PR (CI) + Checklist يدوي قبل الـ Release | |

> **تنبيه:** SQLite أو InMemory Provider **مبيتصرفوش** زي SQL Server في بعض الحاجات (Transactions, `rowversion`, `CHECK`, `ExecuteUpdate`). اختبارات الـ Checkout والـ Concurrency لازم تشتغل على SQL Server حقيقي (LocalDB أو Container).

## 12.2 هرم الاختبارات في مشروعنا

أغلب الاختبارات Unit/Integration سريعة على الـ Services، وعدد قليل من الـ Functional Tests للـ Authorization، وقليل جداً من Manual Tests للـ End-to-End مع Stripe. **كل Feature Pull Request لازم يضيف Tests للـ Service Logic والـ Authorization.**

## 12.3 Test Cases الأساسية (الـ 15 مجال المطلوبين)

الأرقام TC-01 إلى TC-15 مطابقة لترتيب المجالات. بنفذ كل واحد كـ Automated Test لما يكون ممكن، وإلا كـ Manual Script.

| ID | السيناريو | النتيجة المتوقعة | طريقة التنفيذ |
|---|---|---|---|
| **TC-01a** | **Unauthorized Access:** Guest يفتح `/Cart` | Redirect لـ Login (302)، مفيش بيانات | Functional: `GET /Cart` بدون Cookie |
| **TC-01b** | Guest يفتح `/Vendor/Products` و`/Admin` | Redirect/403 | نفس الطريقة لكل Route محمي (Theory بـ `[InlineData]`) |
| **TC-02** | **Vendor A يعدّل منتج Vendor B:** `POST /Vendor/Products/Edit/{idB}` | 404، والمنتج لم يتغير في الـ DB | Functional بـ Vendor A Login، وبعدها Assert على الـ DB |
| **TC-02b** | Overposting: إرسال `VendorProfileId=B` في الـ Form | يتجاهل الحقل، والملكية تفضل A | Functional بإرسال حقل زيادة |
| **TC-03** | **Vendor A يشوف OrderItems بتاعة B:** `GET /Vendor/Orders/Details/{itemB}` | 404، وقائمة الطلبات فيها items A فقط | Seed Order فيه Items لـ Vendorين، ثم Assert على المحتوى |
| **TC-04a** | **Vendor غير معتمد (Pending/Rejected/Suspended)** يفتح `/Vendor/*` | 403 أو Redirect، مع رسالة الحالة | Seed Vendor بكل حالة، `Theory` |
| **TC-04b** | Vendor اتعمله Suspend وهو Logged-In | الطلب التالي مرفوض فوراً | Suspend ثم طلب بنفس الـ Cookie |
| **TC-05a** | **Invalid Product ID:** `GET /Products/Details/99999` | 404 | Functional |
| **TC-05b** | `POST /Cart/Add` بـ productId غير موجود/Inactive | رسالة خطأ ولا Insert | Unit على `CartService` |
| **TC-06** | **Insufficient Stock:** المخزون 2 والعميل يطلب 3 | رفض بـ "Not enough stock" | Unit + Integration |
| **TC-06b** | آخر قطعة، اتنين Checkout في نفس اللحظة | واحد بس ينجح، والتاني يُرفض، والمخزون = 0 مش سالب | Integration بـ `Task.WhenAll` على SQL Server |
| **TC-07** | **Invalid Quantities:** 0، -1، 1000000، نص | رفض Validation | Unit بـ `[Theory]` |
| **TC-08** | **Client-side Price Manipulation:** إرسال `price=0.01` في الـ Request | السعر المستخدم هو سعر الـ DB، والـ Order بالسعر الصحيح | Functional: إضافة Field `price` وAssert على `OrderItem.UnitPrice` |
| **TC-09** | **Failed Payment:** كارت مرفوض | الـ Order يفضل `PendingPayment` ولا Commission، ولو الـ Session اتنهت: Cancelled والمخزون يرجع | Manual بكارت الرفض من Stripe Testing + `stripe trigger` |
| **TC-10** | **Duplicate Webhooks:** نفس `evt_` مرتين | المرة الأولى تعمل Paid، التانية تُتجاهل: Commissions ما اتكررتش | Unit/Integration: نادي `HandleAsync` مرتين بنفس الـ Event |
| **TC-10b** | توقيع غلط أو Body متعدّل | 400 ولا تغيير | Functional بإرسال Signature خاطئ |
| **TC-10c** | مبلغ الـ Session ≠ مبلغ الـ Order | رفض + Log، ولا Paid | Unit بـ Event مصطنع |
| **TC-11** | **Incorrect Commission Calculation** | النتائج مطابقة لجدول الأمثلة (255.55 ← 25.56 / 229.99) | Unit بقيم محسوبة يدوياً، مع حالات التقريب (0.005) |
| **TC-11b** | تغيير النسبة بعد الدفع | السجلات القديمة بنفس النسبة القديمة | Integration |
| **TC-12** | **Multi-Vendor Order:** Cart فيه منتجات من 3 Vendors | Order واحد، 3+ Items، كل Item بـ VendorProfileId الصح، Commission لكل Item | Integration |
| **TC-13a** | **DB Constraints:** `Price = -1` مباشرة في الـ DB | `SqlException` (CHECK) | Integration بـ SaveChanges مباشرة |
| **TC-13b** | Unique: StoreName/Email/StripeEventId مكرر | `DbUpdateException` | Integration |
| **TC-13c** | FK: OrderItem لمنتج غير موجود | رفض | Integration |
| **TC-14a** | **Invalid Role Permissions:** Customer يفتح `/Vendor/*` و`/Admin/*` | 403 | Functional بـ Customer |
| **TC-14b** | Vendor يفتح `/Admin/*` | 403 | Functional |
| **TC-14c** | Customer يطلب `/Orders/Details/{orderOfOtherCustomer}` | 404 | Functional |
| **TC-15** | **Order Status Transitions:** المسموح `PendingPayment→Paid/Cancelled`، `Paid→Completed`، الممنوع `Paid→Cancelled`، `Completed→Paid`، Shipped قبل Paid | الممنوع يرجع Fail ولا يتغير شيء | Unit على State Machine (Diagram 20) بـ `Theory` |
| **TC-15b** | Fulfillment: `Delivered` قبل `Shipped` | رفض | Unit |

## 12.4 مثال Authorization Test (xUnit + WebApplicationFactory)

```csharp
public class VendorIsolationTests : IClassFixture<MarketHubFactory>
{
    private readonly MarketHubFactory _factory;
    public VendorIsolationTests(MarketHubFactory factory) => _factory = factory;

    [Fact]
    public async Task VendorA_CannotEdit_VendorB_Product()
    {
        var (vendorA, productOfB) = await _factory.SeedTwoVendorsAsync();
        var client = _factory.CreateClientLoggedInAs(vendorA);

        var response = await client.PostFormAsync(
            $"/Vendor/Products/Edit/{productOfB.Id}",
            new { Name = "Hacked", Price = 1 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var fromDb = await _factory.GetProductAsync(productOfB.Id);
        Assert.NotEqual("Hacked", fromDb.Name);
    }
}
```

`MarketHubFactory` و`PostFormAsync` و`CreateClientLoggedInAs` Helpers نكتبهم مرة في `TestHelpers/` (بيتعاملوا مع AntiForgery Token وتسجيل الدخول). الفريق يتفق على Test Authentication Handler بسيط للـ Tests [Team Decision].

## 12.5 مثال Unit Test للعمولة

```csharp
[Theory]
[InlineData(200.00, 10.00, 20.00, 180.00)]
[InlineData(55.55, 10.00, 5.56, 49.99)]   // 5.555 rounds away from zero to 5.56
[InlineData(100.00, 0.00, 0.00, 100.00)]
public void Calculate_ReturnsExpectedAmounts(decimal gross, decimal rate, decimal commission, decimal net)
{
    var result = CommissionCalculator.Calculate(gross, rate);
    Assert.Equal(commission, result.CommissionAmount);
    Assert.Equal(net, result.VendorNetAmount);
}
```

## 12.6 الـ Collective Testing و الـ Regression

- **Fresh Eyes:** في آخر كل Sprint، كل عضو بيجرب Feature عضو تاني بـ Test Script مكتوب (من الـ Acceptance Criteria).
- **Bug Report Template:** خطوات، المتوقع، الفعلي، Screenshot، البيئة.
- **Regression Checklist قبل أي Release:** Register/Login، Vendor Apply/Approve، Product Create، Add to Cart، Checkout بكارت تجريبي، Webhook، Commission، Vendor Orders، Admin Dashboard.
- **تعريف الجودة للـ Sprint:** مفيش Bug من نوع Critical (أمان/فلوس/فقد بيانات) مفتوح.

---pagebreak---
