# StockLedger

**Know your real stock. Know your real profit. Across every channel.**

StockLedger is a multi-tenant SaaS platform for **Inventory Management + Financial Visibility**, built for Indonesian mid-to-upper retailers who sell across multiple channels (Tokopedia, Shopee, offline) but lack a unified view of their stock and profitability.

Built with **.NET 8**, **ASP.NET Core Web API**, **Blazor**, and **MySQL** — following **Clean Architecture** principles.

---

## The Problem

Indonesian retailers selling on 2+ channels face three daily pain points:

1. **No unified stock view** — Stock numbers across Tokopedia, Shopee, and offline toko diverge within hours. Overselling causes order cancellations and marketplace penalties.
2. **No real profitability data** — Revenue is visible but true margin per product, per channel is unknown after platform fees (5-15%).
3. **Tax compliance burden** — PKP businesses must issue e-Faktur and track PPN manually. Error-prone and consumes 2+ days/month.

## The Solution

StockLedger is the **single source of truth** for stock and money:

- **Import sales from any channel** (CSV from Tokopedia/Shopee, manual for offline)
- **Real-time stock levels** across all warehouses
- **Profit & Loss by channel** — see true margin after platform fees
- **PPN tracking + e-Faktur** — tax compliance built into every transaction
- **Full purchasing cycle** — Supplier > PO > Goods Receipt > Stock update

---

## Architecture

```
StockLedger/
  |-- Domain/              # Entities, enums, interfaces (zero dependencies)
  |-- Application/         # Use cases, DTOs, service interfaces, AutoMapper
  |-- Infrastructure/      # EF Core, repositories, JWT, middleware
  |-- API/                 # ASP.NET Core controllers, Swagger, auth policies
  |-- BlazorApp/           # Blazor Server + WebAssembly frontend
  |-- Tests/               # E2E integration tests (xUnit)
  |-- docs/                # Product strategy, ERDs, system design
```

### Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 8.0 |
| API | ASP.NET Core 8.0 Web API |
| ORM | Entity Framework Core 8.0 (Pomelo MySQL) |
| Database | MySQL 8.0 |
| Auth | JWT Bearer (HS256) + BCrypt |
| Frontend | Blazor Server + WebAssembly |
| Tests | xUnit + FluentAssertions |
| API Docs | Swagger / Swashbuckle |

---

## Modules

### Inventory App

| Module | Features |
|--------|----------|
| **Product Catalog** | Categories (hierarchical), brands, products, variants, images, units of measure with conversions |
| **Warehouses** | Multi-warehouse management, stock level tracking per warehouse |
| **Stock Movements** | Immutable ledger of all stock changes (in/out/adjust/transfer) |
| **Purchasing** | Suppliers, purchase orders (Draft > Submit > Approve), goods receipt with auto-stock |

### Finance App

| Module | Features |
|--------|----------|
| **Chart of Accounts** | Full CoA setup for double-entry bookkeeping |
| **Journal Entries** | Double-entry journal entries with multi-line support |
| **Invoicing** | Invoice creation, line items, e-Faktur numbering |
| **Tax Management** | PPN rates, product-tax mapping, tax compliance tracking |
| **Payment Terms** | Configurable payment terms for invoices |

### Platform

| Module | Features |
|--------|----------|
| **Multi-Tenancy** | Single-DB TenantId isolation, global query filters, tenant settings |
| **Auth & RBAC** | JWT + refresh tokens, roles, 40+ permission-based policies, custom roles |
| **Audit** | Immutable audit log, system logs, idempotency middleware |
| **Settings** | Tenant settings, system settings |

### Blazor Frontend (22 pages)

| Area | Pages |
|------|-------|
| **Auth** | Login, Register |
| **Dashboard** | Landing page, Dashboard |
| **Catalog** | Products, Brands, Categories, Units of Measure |
| **Inventory** | Stock Levels, Stock Movements, Warehouses |
| **Finance** | Chart of Accounts, Invoices, Journal Entries, Payment Terms, Tax Rates |
| **Admin** | Users, Roles, Tenant Settings, Audit Logs, Profile |

---

## API Endpoints (19 Controllers)

| Domain | Controllers |
|--------|------------|
| **Auth** | Auth |
| **Users & RBAC** | Users, Roles |
| **Catalog** | Categories, Brands, Products, ProductVariants, ProductImages, UnitsOfMeasure |
| **Inventory** | Warehouses, WarehouseStock, StockMovements |
| **Finance** | ChartOfAccounts, JournalEntries, Invoices, PaymentTerms, TaxRates |
| **Audit** | AuditLogs, SystemLogs |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/)

### Database Setup

```sql
CREATE DATABASE stockledger;
CREATE USER 'log_user'@'%' IDENTIFIED BY 'Omelete123*#';
GRANT ALL PRIVILEGES ON stockledger.* TO 'log_user'@'%';
FLUSH PRIVILEGES;
```

### Run the API

```bash
# Restore packages
dotnet restore StockLedger.sln

# Run API (auto-migrates database on startup)
cd API
dotnet run
```

- **API:** http://localhost:5164
- **Swagger:** http://localhost:5164/swagger

### Run the Blazor App

```bash
cd BlazorApp/BlazorApp
dotnet run
```

### Run Tests

```bash
# Start API first, then run E2E tests
dotnet test Tests/StockLedger.E2E.Tests/StockLedger.E2E.Tests.csproj
```

---

## Default Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@stockledger.io | P@ssw0rd123 |
| Manager | manager@stockledger.io | P@ssw0rd123 |
| Warehouse | warehouse@stockledger.io | P@ssw0rd123 |
| Accountant | accountant@stockledger.io | P@ssw0rd123 |
| Viewer | viewer@stockledger.io | P@ssw0rd123 |

---

## API Authentication

```bash
# Login
POST /api/auth/login
{
  "email": "admin@stockledger.io",
  "password": "P@ssw0rd123"
}

# Use the accessToken in subsequent requests
Authorization: Bearer {accessToken}
X-Tenant-Id: 00000000-0000-0000-0000-000000000001
```

---

## Configuration

**appsettings.json:**

| Setting | Value |
|---------|-------|
| Database | `stockledger` on MySQL 127.0.0.1:3306 |
| JWT Issuer | `stockledger-api` |
| JWT Audience | `stockledger-client` |
| Access Token Expiry | 15 minutes |
| Refresh Token Expiry | 7 days |

---

## Documentation

| Document | Description |
|----------|-------------|
| [Product Strategy](docs/PRODUCT_STRATEGY_CANVAS.md) | Vision, customer segments, competitive positioning, go-to-market |
| [Pricing & Monetization](docs/MONETIZATION_STRATEGY.md) | Subscription tiers, revenue projections, trial strategy |
| [Core Business Journeys](docs/CORE_BUSINESS_JOURNEYS.md) | End-to-end user workflows with data flows |
| [Feature Breakdown](docs/ERP_FEATURE_BREAKDOWN.md) | Complete feature list with priorities and build status |
| [System Architecture](docs/ERP_SYSTEM_DESIGN.md) | Technical architecture, multi-tenancy, domain model, API design |
| [Database Schema](docs/dbdiagram.dbml) | Full database diagram |

---

## License

Private / Proprietary
