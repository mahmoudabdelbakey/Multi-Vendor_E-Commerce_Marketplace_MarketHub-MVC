M=['A','B','C','D','E']
ROLES=[('Facilitator','يدير الـ Planning والـ Review والـ Retro ويتابع الـ Board'),
       ('Backend Lead','يقود قرارات الـ Database والـ Services في الـ Sprint'),
       ('Frontend Lead','يقود قرارات الـ Views والـ Layout والـ UX'),
       ('QA Lead','يقود خطة الاختبار واختبارات الـ Authorization والـ Regression'),
       ('Integration and Docs Lead','يقود ترتيب الـ Merges والـ Migrations وتحديث الـ Docs والـ Diagrams')]
SP=[
 dict(n=0,name='Discovery and Design',goal='وصول الفريق لفهم مشترك: Requirements معتمدة، Diagrams، ERD، وRepository جاهز.',
  phases='1-7',design='Workshop للـ Requirements، كتابة User Stories، مراجعة الـ ERD سطر بسطر، رسم الـ Wireframes.',
  tasks=['تحديث الـ ERD والـ Data Dictionary','كتابة Stories وAcceptance Criteria لـ Cart/Orders','رسم Wireframes للـ Storefront وصفحات Vendor','إعداد GitHub (Protection, Templates, CI)','تجهيز مجلد docs وتحديث الـ Diagrams'],
  integ='دمج الـ Docs والـ Diagrams عبر PR واحد مراجع من اتنين.',test='مراجعة قابلية اختبار كل Acceptance Criteria.',docs='Docs v1 + CONTRIBUTING.md.',demo='عرض الـ Diagrams والـ Backlog.'),
 dict(n=1,name='Foundation',goal='المشروع شغال عند الخمسة، Database وIdentity جاهزين.',
  phases='8-10',design='جلسة على هيكل الفولدرات وقواعد الـ Migrations وسياسة الـ Roles.',
  tasks=['Entities والـ Configurations وأول Migration','إعداد Identity والـ Seeder','Layout وNavbar حسب الـ Role','CI + مشروع الـ Tests + TestDbContext','Authorization Tests + خطوات الـ README'],
  integ='Merge الـ Migration الأولى أولاً ثم باقي الفروع تعمل Rebase.',test='Test Cases 1 و14.',docs='README: خطوات التشغيل.',demo='تسجيل دخول بـ 3 Roles.'),
 dict(n=2,name='Vendor Onboarding',goal='Vendor يقدّم طلب والـ Admin يوافق أو يرفض.',
  phases='11',design='مراجعة Diagram 11 وقواعد الانتقال بين الحالات.',
  tasks=['VendorProfile: Entity وMigration','VendorService: Apply/Approve/Reject','فورم التقديم وقائمة Pending','Policy ApprovedVendor وVendorBaseController','Tests: Vendor غير معتمد وState Transitions'],
  integ='ترتيب: Entity ثم Service ثم Views.',test='Test Case 4.',docs='تحديث Diagram 11 لو اتغير.',demo='Apply ثم Approve ثم دخول Vendor Area.'),
 dict(n=3,name='Catalog',goal='Storefront للعامة وإدارة منتجات Vendor مع عزل كامل.',
  phases='12-13',design='مراجعة ERD للـ Product وقواعد الصور (BR-09) وDiagram 12 و22.',
  tasks=['Category/Product/ProductImage: Entities وMigration وSeed','ProductService: Search/Paging وVendor CRUD بالملكية','Views الـ Storefront (Listing/Details)','Vendor Product Views وFileStorageService','Tests: IDOR وSearch + تحديث ERD'],
  integ='Merge الـ Migration أولاً، ثم الـ Services، ثم الـ Views.',test='Test Cases 2 و5 و13.',docs='تحديث Diagram 6B.',demo='Vendor يضيف منتج ويظهر في الـ Storefront.'),
 dict(n=4,name='Shopping Cart',goal='Cart كامل بحسابات من الـ Database.',
  phases='14',design='مراجعة Diagram 13 وقواعد BR-03 و BR-04.',
  tasks=['CartItem: Entity وUnique Index وMigration','CartService: Add/Update/Remove/Get','Cart Views وعداد الـ Navbar','CartController وAntiForgery ورسائل الأخطاء','Tests: مخزون/كمية/تلاعب بالسعر'],
  integ='ربط عداد الـ Navbar بالـ Layout المشترك بالتنسيق مع Frontend Lead.',test='Test Cases 6-8.',docs='README: Seed Demo Accounts.',demo='Cart من 3 Vendors.'),
 dict(n=5,name='Orders and Checkout',goal='Checkout بـ Transaction وحجز المخزون وشاشات الطلبات.',
  phases='15',design='Design Session للـ Transaction ورسم Diagram 14 على السبورة قبل الكود.',
  tasks=['Order/OrderItem/Payment: Entities وConstraints وMigration','CheckoutService: Transaction وConditional UPDATE','Checkout Views وOrder History','Vendor Orders وOrderService.UpdateFulfillment','Tests: سباق آخر قطعة وState Transitions'],
  integ='Migration واحدة بس لكل الجداول المالية، يكتبها Integration Lead بعد مراجعة الـ Entities.',test='Test Cases 9-12 و15.',docs='Diagram 20A/20B.',demo='Order مقسوم بين Vendorين.'),
 dict(n=6,name='Payments and Commission',goal='دفع Stripe Test Mode وWebhook Idempotent وعمولات صحيحة.',
  phases='16-17',design='الفريق كله يقرأ صفحات Stripe المعتمدة (Checkout Fulfillment وWebhooks) ويرسم Diagram 17 بإيده.',
  tasks=['StripePaymentService: Create Session','Webhook Controller/Service: Signature وIdempotency','CommissionSetting/Record وCommissionService','Success/Cancel/Retry وAdmin Commission Form','Tests: Duplicate/Signature/Commission وStripe CLI'],
  integ='الـ Webhook والـ Commission يتدمجوا في Branch مشترك قبل الـ Merge (Pair).',test='Test Cases 9-11.',docs='Stripe CLI Guide.',demo='دفع تجريبي كامل حتى ظهور Commission.'),
 dict(n=7,name='Dashboards',goal='Customer وVendor وAdmin Dashboards.',
  phases='18-20',design='مراجعة الأرقام المطلوبة في كل Dashboard مع الـ Stories.',
  tasks=['Aggregates للـ Vendor','Aggregates وOrders Monitoring للـ Admin','تفاصيل الطلب وCancel للـ Customer','Views وCharts','Tests: الأرقام مقابل الـ Database والـ Authorization'],
  integ='توحيد الـ Layout بين الـ Areas.',test='Test Case 3.',docs='Screenshots أولية.',demo='لوحة Vendor وAdmin بأرقام حقيقية.'),
 dict(n=8,name='Hardening',goal='اختبار شامل وأمان وإصلاح الأخطاء.',
  phases='21-23',design='مراجعة Checklist الأمان وTest Matrix كاملة.',
  tasks=['Security Review (Secrets/CSRF/Uploads)','إصلاح Bugs من Test Report','مراجعة Indexes والـ Queries','Regression Automation في الـ CI','Test Report وتحديث Docs'],
  integ='Freeze للـ Features الجديدة؛ Bug Fixes فقط.',test='كل الحالات الـ 15.',docs='Test Report.',demo='Regression كامل.'),
 dict(n=9,name='Release',goal='نشر Demo وWebhook حي وتجهيز العرض.',
  phases='24-26',design='مراجعة Deployment Plan وسيناريو الـ Demo.',
  tasks=['Deployment Config وMigrations Script','Smoke Tests على Production وWebhook Endpoint','README وScreenshots وDemo Accounts','العرض التقديمي وسكريبت الـ Demo','تحديث نهائي للـ Diagrams وDry Runs'],
  integ='Tag `v1.0.0` بعد Smoke Test.',test='Smoke + Dry Run.',docs='Docs النهائية.',demo='العرض النهائي.')]
def role(m,s): return (m+s)%5
def task(m,s): return (m+2*s)%5
def pairs(s):
    o=[(i+s)%5 for i in range(5)]
    s1=[(o[0],o[1]),(o[2],o[3])]; r1=o[4]
    s2=[(o[1],o[2]),(o[3],o[4])]; r2=o[0]
    return s1,r1,s2,r2
def reviewers(m,s):
    d1=1+(s%4); d2=(d1%4)+1
    return (m+d1)%5,(m+d2)%5
o=[]
w=o.append
w("# 10. خطة التعاون للفريق (Five-Member Collaboration Plan)\n")
w("## 10.1 الفلسفة: خمسة أشخاص، مشروع واحد مفهوم عند الكل\n")
w("هدفنا مش بس نخلّص المشروع، هدفنا إن **كل واحد فينا يقدر يبني ويصلّح ويشرح أي جزء في النظام**. عشان كده مفيش 'مسؤول Backend' ثابت ولا 'مسؤول Frontend' ثابت. لو كل واحد اتخصص في طبقة، هتطلع النتيجة خمس موديولات منفصلة، وكل واحد مش فاهم باقي المشروع.\n")
w("### الممارسات اللي هنلتزم بيها\n")
w("- **Shared Planning:** كل Sprint يبدأ بجلسة واحدة للخمسة، وكل واحد بيقول فهمه للـ Stories.\n- **Small Cross-Functional Tasks:** كل Task صغيرة (يوم إلى يومين) وبتمس طبقة واحدة، ولكن بتتجمع في Vertical Slice واحد.\n- **Pair Programming:** جلستين Pair في كل Sprint بتتبدل الثنائيات (Driver/Navigator بيتبدلوا كل 20-30 دقيقة).\n- **Rotating Leadership:** القيادة (مش الملكية) بتدور كل Sprint بين 5 أدوار.\n- **Cross-Member Code Review:** كل PR بيتراجع من عضو غير الكاتب.\n- **Collective Testing:** كل عضو بيجرب Feature كتبها عضو تاني (Fresh Eyes).\n- **Knowledge Sharing:** 30 دقيقة في آخر كل Sprint: كل عضو يشرح جزء كتبه زميله.\n- **Sprint Review وRetrospective** في آخر كل Sprint.\n")
w("## 10.2 الأدوار الدوّارة (Rotating Leadership Roles)\n")
w("| الدور | المسؤولية خلال الـ Sprint |\n|---|---|")
for r,d in ROLES: w(f"| **{r}** | {d} |")
w("\n> **الـ Lead مش المالك.** الـ Backend Lead مثلاً بيدير النقاش ويقرر لو الفريق اختلف، لكن Code الـ Backend بيكتبه ويراجعه أكتر من واحد، وأي حد ممكن يسأل ويعدّل. القاعدة: **لو عضو لوحده بيفهم جزء، ده Risk لازم نعالجه في نفس الـ Sprint.**\n")
w("## 10.3 جدول المسؤوليات الدوّارة (Responsibility Matrix)\n")
w("الجدول بيبيّن دور كل عضو في كل Sprint (Fac = Facilitator، BE = Backend Lead، FE = Frontend Lead، QA = QA Lead، INT = Integration and Docs Lead). كل عضو بياخد كل دور مرتين على مدار الـ 10 Sprints.\n")
sh={0:'Fac',1:'BE',2:'FE',3:'QA',4:'INT'}
w("| العضو | "+" | ".join(f"S{s}" for s in range(10))+" |\n|---|"+"---|"*10)
for m in range(5): w(f"| Member {M[m]} | "+" | ".join(sh[role(m,s)] for s in range(10))+" |")
w("\n### جدول الـ Pair Programming (Session 1 / Session 2)\n")
w("في كل Sprint بنقسم الفريق مرتين: الـ Rover هو العضو الخامس اللي بينضم لثنائي ويشارك كـ Navigator تالت.\n")
w("| Sprint | Session 1 (النصف الأول) | Session 2 (النصف التاني) |\n|---|---|---|")
for s in range(10):
    s1,r1,s2,r2=pairs(s)
    f=lambda ps,r: " ، ".join(f"{M[a]}+{M[b]}" for a,b in ps)+f" (Rover: {M[r]})"
    w(f"| S{s} | {f(s1,r1)} | {f(s2,r2)} |")
w("\n### جدول الـ Code Review\n")
w("كل صف بيوضح مين بيراجع PRs مين. الـ Reviewer الأول مطلوب لكل PR، والتاني مطلوب لـ PRs اللي بتمس الـ Database أو الدفع أو الـ Authorization.\n")
w("| Sprint | "+" | ".join(f"PR بتاع {M[m]}" for m in range(5))+" |\n|---|"+"---|"*5)
for s in range(10):
    w(f"| S{s} | "+" | ".join(f"{M[reviewers(m,s)[0]]} + {M[reviewers(m,s)[1]]}" for m in range(5))+" |")
w("")
w("## 10.4 خطة كل Sprint بالتفصيل\n")
w("المدة المقترحة 2 أسبوع لكل Sprint [Team Decision]. الـ Tasks الخمسة صغيرة ومتكاملة في نفس الـ Vertical Slice. توزيع الـ Tasks بيدور كل Sprint.\n")
tnames=lambda s: SP[s]['tasks']
for sp in SP:
    s=sp['n']
    w(f"### Sprint {s}: {sp['name']} (Phases {sp['phases']})\n")
    w("| البند | التفاصيل |\n|---|---|")
    w(f"| **1. Sprint Goal** | {sp['goal']} |")
    w(f"| **2. Shared Planning Session** | اليوم الأول، ساعتين، الخمسة: مراجعة الـ Backlog، تقسيم الـ Stories لـ Tasks صغيرة، وتحديد Definition of Done، وتوزيع الأدوار ({', '.join(f'{M[m]}={sh[role(m,s)]}' for m in range(5))}). |")
    w(f"| **3. Shared Requirements and Design** | {sp['design']} |")
    tl="<br>".join(f"Member {M[m]}: {sp['tasks'][task(m,s)]}" for m in range(5))
    w(f"| **4. Small Tasks لكل عضو** | {tl} |")
    s1,r1,s2,r2=pairs(s)
    pl=f"Session 1: "+" ، ".join(f"{M[a]}+{M[b]}" for a,b in s1)+f" (Rover {M[r1]}). Session 2: "+" ، ".join(f"{M[a]}+{M[b]}" for a,b in s2)+f" (Rover {M[r2]})."
    w(f"| **5. Pair Programming** | {pl} |")
    rl=" ، ".join(f"PR {M[m]} ← {M[reviewers(m,s)[0]]}" +(f" و{M[reviewers(m,s)[1]]}" if True else '') for m in range(5))
    w(f"| **6. Code Review** | {rl} |")
    w(f"| **7. Integration Tasks** | {sp['integ']} (المسؤول: {M[[m for m in range(5) if role(m,s)==4][0]]} كـ INT Lead). |")
    qa=[m for m in range(5) if role(m,s)==3][0]
    w(f"| **8. Testing Tasks** | {sp['test']} الكل يختبر Feature زميله يدوياً، وQA Lead ({M[qa]}) يجمّع النتائج. |")
    w(f"| **9. Documentation Tasks** | {sp['docs']} ({M[[m for m in range(5) if role(m,s)==4][0]]} يتأكد من التحديث). |")
    fac=[m for m in range(5) if role(m,s)==0][0]
    w(f"| **10. Sprint Review** | Demo: {sp['demo']} بتقديم عضو مختلف لكل جزء ({M[fac]} يدير الجلسة). |")
    w(f"| **11. Retrospective** | 45 دقيقة: ما الذي نجح؟ ما الذي لم ينجح؟ Action Item واحد على الأقل بمسؤول وموعد. |")
    w("")
w("## 10.5 مثال عملي: الفريق كله على Feature واحدة (Vendor Product Management، Sprint 3)\n")
w("| اليوم | النشاط | مين؟ |\n|---|---|---|")
for r in [
 ("1","Planning: مراجعة US-14 وDiagrams 12 و22 وERD الخاص بـ Product. كل عضو يشرح جزء بصوته.","الخمسة"),
 ("2","Design Session: نكتب على السبورة شكل `ProductService` وحالات الخطأ وقواعد الملكية.","الخمسة"),
 ("2-3","Entities وConfigurations وMigration (Pair).","عضوين + Rover"),
 ("3-5","ProductService (Create/Update/Deactivate) بالملكية (Pair بالتبادل Driver/Navigator).","عضوين"),
 ("3-5","Views الـ Vendor (Index/Create/Edit) و`FileStorageService`.","عضوين"),
 ("5","Integration: Merge الـ Migration، ثم الـ Service، ثم الـ Views، مع Rebase للفروع.","INT Lead + الباقي"),
 ("6","Collective Testing: كل عضو يحاول يكسر الصلاحيات (Vendor A ← منتج Vendor B) ويكتب Test.","الخمسة"),
 ("7","Code Review متبادل (حسب الجدول)، وتصحيح الملاحظات.","كل عضو بيراجع PR"),
 ("8","Docs: تحديث Diagram 6B وـ README.","INT Lead + Reviewer"),
 ("9","Sprint Review: Demo من عضو، والباقي يجاوب أسئلة. Knowledge Share لمدة 30 دقيقة.","الخمسة")]:
    w(f"| {r[0]} | {r[1]} | {r[2]} |")
w("")
w("## 10.6 تنسيق التعديلات على الملفات المشتركة\n")
w("| الملف المشترك | المشكلة | القاعدة |\n|---|---|---|")
for r in [
 ("`ApplicationDbContext`","كل Feature بتضيف `DbSet`، فيحصل Conflict على نفس السطور.","نضيف الـ `DbSet` في سطر جديد بترتيب أبجدي، ونحط الـ Fluent Config في ملفات `Configurations/` منفصلة (`ApplyConfigurationsFromAssembly`)."),
 ("`Program.cs`","كل Feature بتسجل Services.","نقسّم التسجيل لـ Extension Methods (`AddCartServices()` ...) في `Infrastructure/ServiceRegistration.cs`، وكل Feature تعدّل ملف خاص بيها قدر الإمكان."),
 ("Shared ViewModels","تعديلات متضاربة على نفس الـ Class.","لا نعدّل ViewModel مشترك إلا بعد إعلام الفريق، والتغيير يتعمل في PR صغير مستقل."),
 ("Shared Layouts (`_Layout.cshtml`)","تغييرات في الـ Navbar من أكتر من عضو.","Frontend Lead في الـ Sprint هو بوابة التغيير، ونستخدم Partial Views (`_NavbarCart.cshtml`) لتقليل التضارب."),
 ("Entity Configuration","تعديل نفس الـ Configuration.","ملف لكل Entity، وأي تغيير في Entity موجودة بيمر على PR من الـ Backend Lead."),
 ("DI Configuration","ترتيب التسجيل.","الترتيب مش مهم للـ Scoped Services العادية، لكن لازم نتجنب تسجيل نفس الـ Interface مرتين."),
 ("Migrations","Snapshot واحد، لو اتنين عملوا Migration هيتعارضوا.","القواعد في الفصل 11.")]:
    w(f"| {r[0]} | {r[1]} | {r[2]} |")
w("")
w("## 10.7 ازاي نقلل الـ Merge Conflicts ونضمن إن الكل فاهم؟\n")
w("1. **Branches قصيرة العمر** (1-3 أيام) و`git fetch` + Rebase على `main` كل يوم.\n2. **Small PRs** (أقل من 400 سطر قدر الإمكان).\n3. **ملفات صغيرة** (Configuration لكل Entity، Controller لكل مجال).\n4. **Merge Order** يحدده INT Lead في الـ Integration: Migrations ثم Services ثم Controllers ثم Views.\n5. **Code Review كوسيلة تعلم:** الـ Reviewer لازم يكتب في الـ PR سطرين بيشرح فيهم ماذا فهم من الـ Change، وإن ما فهمش يسأل.\n6. **'Explain it back':** في الـ Sprint Review أي عضو ممكن يتسأل عن أي Feature، وليس فقط اللي كتبها.\n7. **Rotating Tasks:** مفيش حد ياخد نفس نوع الـ Task في Sprintين متتاليين.\n8. **Bus Factor ≥ 3:** أي جزء في النظام لازم يكون على الأقل 3 أشخاص يعرفوه.\n")
w("---pagebreak---\n")
open('/home/claude/pkg/content/10_collaboration.md','w').write("\n".join(o))
