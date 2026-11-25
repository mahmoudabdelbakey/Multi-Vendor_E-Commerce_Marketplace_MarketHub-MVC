# MarketHub Architecture

## Layers Diagram

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                │
│                   (MarKetHub.MVC)                   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐   │
│  │ Controllers │ │    Views    │ │   Areas     │   │
│  └─────────────┘ └─────────────┘ └─────────────┘   │
└─────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│                Business Logic Layer                 │
│                   (MarKetHub.BLL)                   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐   │
│  │  Services   │ │ Interfaces  │ │   Models    │   │
│  └─────────────┘ └─────────────┘ └─────────────┘   │
└─────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│                 Data Access Layer                   │
│                   (MarKetHub.DAL)                   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐   │
│  │  Entities   │ │DbContext    │ │Configurations│  │
│  └─────────────┘ └─────────────┘ └─────────────┘   │
└─────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│                     Database                        │
│                 (SQL Server/SQLite)                 │
└─────────────────────────────────────────────────────┘
```

## Project Structure

- **MarKetHub.MVC**: ASP.NET Core MVC web application
  - Controllers: Handle HTTP requests
  - Views: Razor views for UI
  - Areas: Admin, Vendor sections
  - Infrastructure: DI configuration, features

- **MarKetHub.BLL**: Business Logic Layer
  - Services: Business logic implementation
  - Interfaces: Service contracts
  - Models: DTOs and view models
  - Common: Shared utilities (Result, PagedResult)

- **MarKetHub.DAL**: Data Access Layer
  - Entities: Database models
  - Data: DbContext and configurations
  - Enums: Status enumerations
  - Migrations: EF Core migrations
