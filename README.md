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
CREATE USER 'stockledger_user'@'%' IDENTIFIED BY '<your-password>';
GRANT ALL PRIVILEGES ON stockledger.* TO 'stockledger_user'@'%';
FLUSH PRIVILEGES;
```

Then update `API/appsettings.json` with your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=127.0.0.1;port=3306;database=stockledger;user=stockledger_user;password=<your-password>;AllowUserVariables=true"
}
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

## Core Business Journeys

StockLedger supports **5 core workflows** that map to the daily, weekly, and monthly operations of a multi-channel retailer:

### Journey 1: "Set Up My Store" — Initial Configuration

**Actor:** Admin / Owner | **Frequency:** One-time (first 15 minutes)

```
Register Account
  ├── Create Warehouses (Gudang Utama Jakarta, Gudang Surabaya)
  ├── Create Product Catalog
  │     ├── Categories (hierarchical)
  │     ├── Brands, Units of Measure
  │     ├── Products + Variants (with SKU, cost price, selling price)
  │     └── Product Images
  ├── Configure Finance
  │     ├── Chart of Accounts (AR, AP, Revenue, COGS, Tax Payable)
  │     ├── Tax Rates (PPN 11%)
  │     └── Payment Terms (COD, Net 30, Net 60)
  └── Set Up Team (invite warehouse staff, accountant, viewer with RBAC)
```

### Journey 2: "Stock the Shelves" — Purchasing & Receiving

**Actors:** Manager + Warehouse Staff | **Frequency:** Weekly

```
Low stock detected
  ├── Create Purchase Order (supplier, warehouse, line items)
  ├── Submit → Approve (approval workflow)
  └── Goods Receipt (confirm received quantities)
        ├── WarehouseStock auto-updated
        ├── StockMovement created (Type: In, Reason: Purchase)
        └── Full audit trail
```

### Journey 3: "Record My Sales" — Multi-Channel Sales Import

**Actor:** Owner / Manager | **Frequency:** Daily

```
End of day:
  ├── Tokopedia: Export CSV → Upload → SKU matching → Stock deduction
  ├── Shopee: Export CSV → Upload → SKU matching → Stock deduction
  └── Offline toko: Manual entry → Stock deduction
        ├── StockMovement per channel (Type: Out, Reason: Sale)
        ├── Revenue + platform fees recorded
        └── Dashboard updates in real-time
```

### Journey 4: "Close the Books" — Monthly Finance & Tax

**Actor:** Accountant | **Frequency:** Monthly

```
Month-end:
  ├── Review & create invoices (with e-Faktur numbering)
  ├── Record journal entries (double-entry, balanced)
  ├── Generate P&L by channel (revenue - COGS - platform fees)
  └── Tax compliance (PPN output - PPN input = net payable)
```

### Journey 5: "Check My Business" — Dashboards & Decisions

**Actor:** Owner | **Frequency:** On-demand

```
Dashboard:
  ├── Stock Health: levels, low stock alerts, overstock warnings
  ├── Sales Performance: by channel, top products, trends
  ├── Profitability: margin per product, margin per channel
  └── Action Items: reorder suggestions, pricing alerts
```

---

## Features — Build Status

### Built and Tested

| Module | Key Features |
|--------|-------------|
| **Multi-Tenancy** | Single-DB TenantId isolation, global query filters, tenant settings |
| **Auth & RBAC** | JWT + refresh tokens, BCrypt, 70+ permissions, custom roles |
| **Audit** | Immutable audit log, system logs, idempotency middleware |
| **Product Catalog** | Categories (hierarchical), brands, units of measure + conversions, products, variants (SKU, pricing, barcode), images |
| **Warehouses** | Multi-warehouse management, per-variant stock tracking, reorder points, optimistic concurrency |
| **Stock Movements** | Immutable ledger (In/Out/Transfer/Adjust), quantity before/after, reference linking |
| **Purchasing** | Suppliers, POs (Draft → Submit → Approve → Receive), goods receipt with auto-stock update |
| **Chart of Accounts** | Full CoA (Asset, Liability, Equity, Revenue, Expense), parent-child hierarchy |
| **Journal Entries** | Double-entry with multi-line, balance validation, Draft → Posted → Voided |
| **Invoicing** | Line items, e-Faktur numbering, payment tracking, Draft → Issued → Paid |
| **Tax Management** | PPN 11%, effective dates, product-tax mapping |
| **Payment Terms** | Configurable terms (COD, Net 30, Net 60) |

> **75 features built** across 12 modules

### To Build — P0 (Launch)

| Feature | Description |
|---------|-------------|
| **CSV Import Engine** | Upload marketplace CSVs, column mapping, SKU matching, auto stock deduction, duplicate detection |
| **Financial Reports** | P&L by channel, margin per product, margin per channel, revenue breakdown |

### To Build — P1 (Post-Launch)

| Feature | Description |
|---------|-------------|
| Stock Reconciliation | Physical count, variance report, adjustment entries |
| Inter-Warehouse Transfer | Transfer requests with confirmation and dual stock movements |
| Purchase Cost Tracking | Landed cost, cost history per variant, COGS calculation |
| Auto Journal Entries | Auto-create entries from goods receipt and invoice payment |
| Balance Sheet & Cash Flow | Standard financial statements |
| PPN Summary | Input/output summary for DJP e-Faktur filing |
| Dashboards | Stock health, sales performance, profitability, smart recommendations |

### Explicitly Out of Scope

POS, e-commerce storefront, CRM, loyalty programs, HRM, driver/vehicle management, shipment tracking — these are not part of StockLedger's focused inventory + finance product.

---

## License

Private / Proprietary
