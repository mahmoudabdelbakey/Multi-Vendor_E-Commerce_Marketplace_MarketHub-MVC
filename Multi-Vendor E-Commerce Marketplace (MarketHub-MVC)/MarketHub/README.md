# MarketHub: Member A deliverables (TASK-01 .. TASK-12)

3-tier solution: `MarKetHub.MVC` -> `MarKetHub.BLL` -> `MarKetHub.DAL` (SQL Server, EF Core, Identity).

## Run (first time)

```bash
# 1. Secrets (never in source control)
dotnet user-secrets set "Seed:AdminEmail"    "admin@markethub.test" --project src/MarKetHub.MVC
dotnet user-secrets set "Seed:AdminPassword" "Admin@12345"          --project src/MarKetHub.MVC

# 2. Database (only Member A creates migrations)
dotnet ef database drop --force --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC    # skip if no database yet
dotnet ef migrations add InitialCreate --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC --output-dir Data/Migrations
dotnet ef database update --project src/MarKetHub.DAL --startup-project src/MarKetHub.MVC

# 3. Run (the seeder also runs on start-up)
dotnet run --project src/MarKetHub.MVC --launch-profile https
```

Demo accounts (Development only, password `Demo@12345`): `customer@demo.test`, `vendor1@demo.test`, `vendor2@demo.test`, `pending@demo.test`.
Admin: the e-mail / password you put in user-secrets.

See `docs/MemberA_Guide.md` (Arabic) for what every task changed and how to check it by hand, and `docs/CONTRIBUTING.md` for the team rules.
