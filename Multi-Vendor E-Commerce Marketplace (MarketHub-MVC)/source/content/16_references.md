# 16. المراجع والروابط الرسمية (References)

> الروابط دي نقطة البداية للتحقق. **الوثائق بتتحدث**، فالفريق مسؤول عن مراجعة النسخة الحالية قبل التنفيذ. بعض الروابط ثابتة المنشأ (Microsoft Learn و Stripe Docs)، لكن ممكن تتغير مسارات الصفحات.

## Microsoft

| الموضوع | المرجع |
|---|---|
| .NET Support Policy | https://dotnet.microsoft.com/platform/support/policy/dotnet-core |
| .NET Download | https://dotnet.microsoft.com/download |
| ASP.NET Core MVC | https://learn.microsoft.com/aspnet/core/mvc/overview |
| Identity on ASP.NET Core | https://learn.microsoft.com/aspnet/core/security/authentication/identity |
| Authorization (Roles/Policies) | https://learn.microsoft.com/aspnet/core/security/authorization/introduction |
| Entity Framework Core | https://learn.microsoft.com/ef/core/ |
| EF Core Migrations | https://learn.microsoft.com/ef/core/managing-schemas/migrations/ |
| `ExecuteUpdate` / `ExecuteDelete` | https://learn.microsoft.com/ef/core/saving/execute-insert-update-delete |
| Concurrency Tokens | https://learn.microsoft.com/ef/core/saving/concurrency |
| Areas in ASP.NET Core | https://learn.microsoft.com/aspnet/core/mvc/controllers/areas |
| Anti-Request Forgery | https://learn.microsoft.com/aspnet/core/security/anti-request-forgery |
| Safe storage of app secrets | https://learn.microsoft.com/aspnet/core/security/app-secrets |
| Integration tests | https://learn.microsoft.com/aspnet/core/test/integration-tests |
| Host and deploy | https://learn.microsoft.com/aspnet/core/host-and-deploy/ |

## Stripe

| الموضوع | المرجع |
|---|---|
| Stripe Docs الرئيسية | https://docs.stripe.com |
| Checkout: Fulfill orders | https://docs.stripe.com/checkout/fulfillment |
| Webhooks | https://docs.stripe.com/webhooks |
| Testing | https://docs.stripe.com/testing |
| Stripe CLI | https://docs.stripe.com/stripe-cli |
| Stripe Connect | https://docs.stripe.com/connect |
| Stripe.net (GitHub) | https://github.com/stripe/stripe-dotnet |
| API Reference | https://docs.stripe.com/api |

## أدوات ومصادر أخرى

| الموضوع | المرجع |
|---|---|
| GitHub Docs (Branch protection, PRs, Actions) | https://docs.github.com |
| Conventional Commits | https://www.conventionalcommits.org |
| Mermaid | https://mermaid.js.org |
| Graphviz | https://graphviz.org |
| OWASP Top 10 | https://owasp.org/www-project-top-ten/ |
| Bootstrap | https://getbootstrap.com/docs |

## ملخص ما تم التحقق منه وما لم يتم

- **تم التحقق (وقت إعداد الوثيقة):** سياسة دعم .NET (.NET 10 LTS)، وسلوك أحداث Stripe Checkout الأساسية (`checkout.session.completed`, `async_payment_succeeded`, `async_payment_failed`)، ومتطلب Signature Verification بالـ raw body، وأمر `stripe listen`.
- **لم يتم التحقق ويجب على الفريق مراجعته:** أسماء Classes ودوال `Stripe.net` بالضبط، حدود `expires_at`، تفاصيل الأحداث الإضافية مثل `checkout.session.expired` وقواعد Stripe Connect والدول المدعومة، وأسماء Methods في Identity، وأوامر `dotnet ef` الأحدث، وأسعار وشروط الاستضافة.
