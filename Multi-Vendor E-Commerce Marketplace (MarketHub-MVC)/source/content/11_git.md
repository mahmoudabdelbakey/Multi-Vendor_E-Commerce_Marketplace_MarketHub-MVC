# 11. سير العمل على Git و GitHub (Git and GitHub Workflow)

## 11.1 إعداد الـ Repository

1. واحد من الفريق (يتبدل) ينشئ Repository خاص باسم `MarketHub` ويضيف الأعضاء الخمسة كـ Collaborators.
2. نضيف `.gitignore` لـ .NET (`dotnet new gitignore`) ونتأكد إنه بيتجاهل: `bin/`, `obj/`, `.vs/`, `appsettings.*.local.json`, `**/uploads/*` و`*.user`.
3. نضيف `README.md` و`CONTRIBUTING.md` و`global.json` (لتثبيت الـ SDK) و`.editorconfig`.
4. نضيف `.github/pull_request_template.md` و Issue Templates (Bug / Story).

### Branch Protection على `main` (من Settings → Branches)

- Require a pull request before merging (Approvals: 1، و2 للـ PRs اللي فيها Migration أو Payment).
- Require status checks to pass (الـ Build والـ Tests).
- Require branches to be up to date before merging.
- Do not allow bypassing the above settings (حتى للـ Admins).
- ممنوع Force Push على `main`.

## 11.2 تسمية الـ Branches والـ Commits

| النوع | الصيغة | مثال |
|---|---|---|
| Feature | `feature/<issue#>-<short-name>` | `feature/42-vendor-product-create` |
| Bug Fix | `fix/<issue#>-<short-name>` | `fix/57-cart-negative-qty` |
| Docs | `docs/<short-name>` | `docs/update-erd` |
| Chore | `chore/<short-name>` | `chore/ci-workflow` |

**Commit Message** (Conventional Commits): `type(scope): short description` مثل `feat(cart): validate stock before adding item`، و`fix(webhook): ignore duplicate stripe events`، و`test(vendor): add idor tests`. الأنواع: `feat`, `fix`, `docs`, `test`, `refactor`, `chore`. الرسالة بصيغة الأمر وأقل من 72 حرف في السطر الأول.

## 11.3 مثال كامل: Feature صغيرة من الأول للآخر

**السيناريو:** Member C هيضيف زرار "Deactivate" للمنتج (Issue رقم 42)، وMember D هو الـ Reviewer.

```bash
# 1) Start from an up-to-date main
git switch main
git pull origin main

# 2) Create the feature branch
git switch -c feature/42-product-deactivate

# 3) Work and commit in small steps
git add src/MarketHub.Web/Services/Implementations/ProductService.cs
git commit -m "feat(product): add DeactivateAsync with vendor ownership check"
git add tests/MarketHub.Tests/Unit/ProductServiceTests.cs
git commit -m "test(product): deactivate other vendor product returns not found"

# 4) Sync with main before pushing (keeps history linear)
git fetch origin
git rebase origin/main

# 5) Push and open a Pull Request
git push -u origin feature/42-product-deactivate
gh pr create --base main --title "feat(product): deactivate product (#42)" \
  --body "Closes #42. Adds DeactivateAsync + tests. Reviewer: @member-d"
```

**3) Review:** Member D يفتح الـ PR، يقرأ الـ Diff، ويكتب ملاحظة: *"الـ Controller بيرجّع Redirect حتى لو الـ Service رجّعت Fail. ممكن نعرض رسالة الخطأ؟"* ويضغط **Request changes**.

```bash
# 6) Fix the review comments on the same branch
git add src/MarketHub.Web/Areas/Vendor/Controllers/ProductsController.cs
git commit -m "fix(product): show error message when deactivate fails"
git push

# 7) After approval + green CI: Squash and merge on GitHub, then clean up
git switch main
git pull origin main
git branch -d feature/42-product-deactivate
git push origin --delete feature/42-product-deactivate
```

الـ Reviewer بعد ما يوافق (Approve) يكتب سطرين: *"فهمت إن الـ Service بتبحث بـ `VendorProfileId` فبترجع NotFound لمنتجات الغير"*، وده دليل الفهم (القاعدة 5 في 10.7).

### قالب الـ Pull Request

```markdown
## What
Short description. Closes #<issue>

## Why
Link to the user story / business rule (BR-xx).

## How to test
1. Steps ...

## Checklist
- [ ] Build and tests pass locally
- [ ] Server-side validation and authorization covered
- [ ] No secrets or personal data committed
- [ ] Migration included? (needs 2 reviewers)
- [ ] Docs / diagrams updated if the design changed
```

## 11.4 حل الـ Merge Conflicts

```bash
git fetch origin
git rebase origin/main
# Git stops at a conflict: open the files, look for <<<<<<<, =======, >>>>>>>
# Keep the correct combination of both changes, then:
git add <resolved-file>
git rebase --continue
dotnet build && dotnet test      # ALWAYS re-run after resolving
git push --force-with-lease      # only on YOUR feature branch, never on main
```

قواعد: (1) نحل الـ Conflict وإحنا فاهمين الاتنين تغييرات (ما نختارش "Accept mine" بشكل أعمى). (2) لو الـ Conflict في ملف الـ Migrations: انظر 11.7. (3) لو الموضوع معقد: Pair مع كاتب التغيير التاني.

## 11.5 GitHub Issues و Project Board و Milestones

- **Issue لكل Story-slice:** عنوان واضح، Acceptance Criteria كـ Checklist، Labels (`backend`, `frontend`, `db`, `test`, `docs`, `bug`, `payment`)، وMilestone = الـ Sprint (مثلاً `Sprint 3`).
- **Project Board** بأعمدة: `Backlog` ← `Sprint Backlog` ← `In Progress` ← `In Review` ← `Done`. حد أقصى 2 Issues In Progress للعضو (WIP Limit).
- **Milestones:** واحد لكل Sprint ومعاه تاريخ Demo.
- الـ PR بيحتوي `Closes #42` فالـ Issue بيتقفل تلقائياً بعد الـ Merge.

## 11.6 README و Environment Configuration و إدارة الـ Secrets

**الـ README لازم يحتوي:** وصف المشروع، المتطلبات (.NET 10 SDK، SQL Server، Stripe CLI)، خطوات التشغيل، إعداد الـ Secrets، تشغيل الـ Tests، Demo Accounts، هيكل الفولدرات، ولينك للـ Docs.

```bash
# One-time setup per developer (secrets live outside the repo)
cd src/MarketHub.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=MarketHub;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
dotnet user-secrets set "Seed:AdminEmail" "admin@markethub.test"
dotnet user-secrets set "Seed:AdminPassword" "<choose a strong password>"
```

- **الملف `appsettings.json` فيه مفاتيح فاضية فقط.**
- نفعّل **Secret Scanning و Push Protection** في GitHub لو متاحة للـ Repository.
- لو حد رفع Secret بالغلط: **نغيّر المفتاح فوراً (Roll)**، مش بس نمسحه من الـ Commit لأن التاريخ بيفضل.

## 11.7 تنسيق الـ EF Core Migrations بين الأعضاء

المشكلة: كل Migration بتعدّل ملف `ApplicationDbContextModelSnapshot.cs`. لو عضوين عملوا Migration من نفس نقطة البداية، الـ Snapshot هيتعارض، والـ Migrations ممكن تتناقض.

**القواعد:**

1. **Migration واحدة في الـ PR، وبيعملها عضو واحد** (غالباً INT Lead للـ Sprint) بعد ما الـ Entities تتراجع.
2. قبل ما تنشئ Migration: `git fetch && git rebase origin/main` وتأكد إنك على آخر نسخة، وتطبّق الـ Migrations الموجودة (`dotnet ef database update`).
3. بعد إنشاء الـ Migration، افتح ملف الـ Migration وراجع الـ `Up` و`Down`، وتأكد إنها بتعمل بس اللي إنت عايزه.
4. **اعلن في قناة الفريق** "أنا شغال على Migration" لحد ما تتعمل Merge.
5. **لو حصل Conflict في الـ Snapshot:** احذف الـ Migration بتاعتك (`dotnet ef migrations remove`) واعمل Rebase، ثم أعد إنشاءها. **متحلش الـ Snapshot يدوياً.**
6. **ممنوع تعديل Migration اتعمل لها Merge.**
7. الـ CI بيتأكد إن `dotnet ef migrations has-pending-model-changes` (أو ما يعادله في النسخة المستخدمة) مبيرجّعش تغييرات ناسية [Team Decision: تأكدوا من اسم الأمر في الإصدار الحالي من `dotnet ef`].

## 11.8 مثال CI بسيط (GitHub Actions)

```yaml
name: ci
on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main ]
jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          global-json-file: global.json
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release
```

> **[Team Decision]:** تأكدوا من إصدارات الـ Actions (`checkout`, `setup-dotnet`) من الـ Marketplace الرسمي لأنها بتتحدث.

---pagebreak---
