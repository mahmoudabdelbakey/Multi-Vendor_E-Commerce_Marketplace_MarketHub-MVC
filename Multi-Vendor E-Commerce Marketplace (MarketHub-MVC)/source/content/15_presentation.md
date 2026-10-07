# 15. خطة العرض النهائي (Final Presentation)

## 15.1 هيكل العرض (حوالي 20 إلى 25 دقيقة [Team Decision])

| # | الموضوع | المحتوى | Diagram / Screenshot | المقدّم |
|---|---|---|---|---|
| 1 | The Problem | بائعون صغار بلا متجر، وعملاء يريدون مكاناً واحداً | Screenshot للـ Home | يتبدل |
| 2 | The Proposed Solution | منصة Multi-Vendor بثلاث بوابات | Diagram 1 (System Context) | |
| 3 | Project Goals | O1-O5 من الفصل 2 | جدول الأهداف | |
| 4 | User Roles | Guest/Customer/Vendor/Super Admin | Diagram 2 (Use Case) | |
| 5 | Main Features | Storefront، Cart، Checkout، Vendor/Admin | Screenshots | |
| 6 | System Architecture | الطبقات والفولدرات | Diagrams 7 و 8 | |
| 7 | Database Design | الجداول الأساسية و Snapshots | Diagrams 6A و 6C | |
| 8 | Security and Authorization | Vendor Isolation | Diagram 22 + Demo قصير للـ 404 | |
| 9 | Payment Flow | Checkout ← Webhook | Diagrams 15 و 17 | |
| 10 | Commission System | مثال حساب | جدول 8.4 | |
| 11 | Team Collaboration | Rotation، Pair، Review | Diagrams 18 و 19 + Matrix | |
| 12 | Testing Results | عدد الـ Tests ونتائج الـ 15 مجال | لقطة CI أخضر | |
| 13 | Live Demo | (انظر 15.2) | | |
| 14 | Challenges and Solutions | Overselling، Webhooks مكررة، Migrations Conflicts | | |
| 15 | Future Improvements | Stripe Connect، Shipping، Email، Refunds | جدول 2.8 | |

كل عضو يقدّم جزء ويجاوب أسئلة الأجزاء الأخرى (علشان يثبت إن الكل فاهم المشروع).

## 15.2 سيناريوهات الـ Live Demo المقترحة

1. **Vendor Onboarding:** Guest يقدّم كـ Vendor ← Admin يوافق ← الـ Vendor يدخل Dashboard.
2. **Product Creation:** Vendor يضيف منتج بصورة ويظهر في الـ Storefront.
3. **Multi-Vendor Cart:** Customer يضيف منتجين من Vendorين مختلفين.
4. **Checkout و Stripe Test:** دفع بكارت تجريبي، وشاشة "قيد التأكيد"، ثم الـ Order يتحول لـ Paid (من الـ Webhook).
5. **Vendor Views:** كل Vendor يشوف Items بتاعته بس ويعمل Shipped/Delivered.
6. **Commission:** Admin/Vendor يشوف العمولة وصافي المبلغ.
7. **Security Moment:** Vendor A يحاول فتح URL منتج Vendor B ← 404.
8. **Idempotency (اختياري):** إعادة إرسال Event بـ Stripe CLI/Dashboard ← مفيش تغيير.

## 15.3 نصائح التحضير

- جرّب الـ Demo 3 مرات على نفس البيئة، وجهّز **Backup Video** و Screenshots.
- جهّز حسابات Demo ومتصفحين (Customer وVendor) جنب بعض.
- اعرض الـ Test Card من Stripe Testing Docs قدامك.
- جهّز إجابات لأسئلة متوقعة: ليه Approach A؟ ليه Reserve Stock؟ ازاي منعتوا Duplicate Webhook؟ ليه مفيش Repository؟ إيه الفرق بين Stripe Checkout وConnect؟

---pagebreak---
