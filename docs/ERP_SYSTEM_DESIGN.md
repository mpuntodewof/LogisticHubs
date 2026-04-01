# NiagaOne ERP - System Architecture & Design Document

**Version:** 1.0
**Date:** 2026-03-25
**Status:** Design Phase
**Author:** Brainstorming Session (Deep Research + System Design + Product Strategy)

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Current State Assessment](#2-current-state-assessment)
3. [Target Architecture Overview](#3-target-architecture-overview)
4. [Multi-Tenancy Architecture](#4-multi-tenancy-architecture)
5. [Domain Model & Bounded Contexts](#5-domain-model--bounded-contexts)
6. [Module Dependency Architecture](#6-module-dependency-architecture)
7. [API Architecture](#7-api-architecture)
8. [Frontend Architecture](#8-frontend-architecture)
9. [Data Architecture](#9-data-architecture)
10. [Integration Architecture](#10-integration-architecture)
11. [Security Architecture](#11-security-architecture)
12. [Infrastructure & Deployment](#12-infrastructure--deployment)
13. [Migration Strategy](#13-migration-strategy)
14. [Implementation Sequence](#14-implementation-sequence)

---

## 1. Executive Summary

NiagaOne is being evolved from a single-tenant logistics management application into a **multi-tenant SaaS ERP platform** targeting Indonesian mid-market retail businesses (20-1,000 employees) operating physical stores and e-commerce channels.

### Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Multi-tenancy | Single DB, TenantId column | Cost-effective for mid-market, simplest operations |
| Module structure | Modular monolith | Solo dev team, can extract to microservices later |
| Cross-module communication | MediatR domain events | Decoupled, testable, no message broker overhead |
| Storefront tech | Blazor WASM + prerendering | Shared codebase with back-office, single language |
| Payment integration | Midtrans + Xendit | Covers all Indonesian payment methods |
| Background jobs | Hangfire with MySQL | Simple, reliable, dashboard included, .NET native |
| Caching | Redis (L2) + IMemoryCache (L1) | Essential for multi-tenant performance |
| File storage | MinIO (S3-compatible) | Self-hosted, cheap, migrate to cloud S3 later |
| Database | MySQL 8.0 (keep existing) | No reason to migrate, Pomelo is mature |
| PDF generation | QuestPDF | Free, .NET native, excellent for invoices |
| Notifications | Fonnte (WhatsApp) + SMTP | WhatsApp is dominant in Indonesian commerce |

---

## 2. Current State Assessment

### What Already Exists (NiagaOne)

**6 projects** following Clean Architecture:
- **Domain** — 12 entities (User, Driver, Vehicle, Warehouse, Shipment, ShipmentAssignment, ShipmentTracking, Role, Permission, UserRoleAssignment, RolePermission, RefreshToken)
- **Application** — 7 use cases, DTOs, service interfaces
- **Infrastructure** — EF Core with MySQL 8.0, 7 repositories, JWT + BCrypt services
- **API** — 7 REST controllers, RequirePermission filter, Swagger
- **BlazorApp** — Blazor Server host
- **BlazorApp.Client** — Blazor WebAssembly SPA

**Technology Stack:** .NET 8, C# 12, EF Core 8.0, MySQL 8.0, Blazor Hybrid, JWT Auth, RBAC (4 roles, 18 permissions)

### Key Gaps for ERP Transformation

- No multi-tenancy
- No product/catalog management
- No POS system
- No inventory beyond basic warehouse capacity
- No sales/order management
- No e-commerce storefront
- No financials (invoicing, payments, expenses)
- No tax management (PPN, e-Faktur)
- No CRM
- No HRM
- No promotions/loyalty
- No notification system
- No background job processing
- No caching layer

---

## 3. Target Architecture Overview

### High-Level System Component Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          INTERNET / CLIENTS                             │
│                                                                         │
│  ┌──────────────┐  ┌──────────────────┐  ┌───────────────────────────┐ │
│  │ Back-Office   │  │ Storefront       │  │ Mobile Apps / 3rd Party   │ │
│  │ Blazor Hybrid │  │ Blazor WASM+SSR  │  │ REST API Consumers        │ │
│  │ (*.app.erp.id)│  │ (tenant.toko.id) │  │                           │ │
│  └──────┬───────┘  └────────┬─────────┘  └────────────┬──────────────┘ │
└─────────┼───────────────────┼──────────────────────────┼────────────────┘
          │                   │                          │
          ▼                   ▼                          ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                   REVERSE PROXY / CDN (YARP + Cloudflare)               │
│              ┌─────────────────────────────────┐                        │
│              │  Tenant Resolution Middleware    │                        │
│              │  Rate Limiting (per tenant)      │                        │
│              │  TLS Termination                 │                        │
│              └─────────────────────────────────┘                        │
└────────────────────────────┬────────────────────────────────────────────┘
                             │
          ┌──────────────────┼──────────────────┐
          ▼                  ▼                  ▼
┌─────────────────┐ ┌────────────────┐ ┌────────────────────┐
│ Back-Office API  │ │ Storefront API │ │ Webhook Receiver   │
│ /api/v1/...      │ │ /store/v1/...  │ │ /hooks/payments    │
│ (Authenticated)  │ │ (Public+Auth)  │ │ /hooks/logistics   │
└────────┬────────┘ └───────┬────────┘ └─────────┬──────────┘
         │                  │                    │
         ▼                  ▼                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                      APPLICATION CORE (Modular Monolith)                │
│                                                                         │
│  ┌─────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────────┐  │
│  │ Tenant  │ │ Catalog  │ │  Sales   │ │ Finance  │ │  Logistics   │  │
│  │ Module  │ │ Module   │ │  Module  │ │  Module  │ │  Module      │  │
│  │         │ │          │ │          │ │          │ │  (existing)  │  │
│  └────┬────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ └──────┬───────┘  │
│       │           │            │            │              │           │
│  ┌────┴───────────┴────────────┴────────────┴──────────────┴────────┐  │
│  │                    SHARED KERNEL                                  │  │
│  │  (TenantId, AuditableEntity, Money, DomainEvents, RBAC)         │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │
         ┌───────────────────────┼───────────────────────┐
         ▼                       ▼                       ▼
┌─────────────────┐  ┌─────────────────────┐  ┌──────────────────────┐
│   MySQL 8.0     │  │   Redis             │  │  Object Storage      │
│   (Primary DB)  │  │   (Cache + Sessions │  │  (MinIO / S3)        │
│                 │  │    + Queues)        │  │  Product images,     │
│                 │  │                     │  │  documents, invoices │
└─────────────────┘  └─────────────────────┘  └──────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                    BACKGROUND SERVICES (Hangfire)                        │
│  ┌──────────────┐ ┌───────────────┐ ┌──────────────┐ ┌──────────────┐  │
│  │ Order        │ │ Notification  │ │ Report       │ │ Tax/e-Faktur │  │
│  │ Processing   │ │ Worker        │ │ Generator    │ │ Sync         │  │
│  └──────────────┘ └───────────────┘ └──────────────┘ └──────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
```

### What Stays, Changes, and Gets Added

**Stays (preserved):**
- Clean Architecture layer separation
- EF Core with MySQL 8.0
- Blazor Server + WASM hybrid for back-office
- JWT authentication mechanism
- All 12 existing entities and 7 controllers (refactored into Logistics module)

**Changes (modified):**
- Every existing entity gains a `TenantId` column
- EF Core DbContext gets global tenant filters
- User entity becomes tenant-scoped; new PlatformUser for super-admin
- JWT claims carry `tenant_id`
- RBAC becomes tenant-scoped with module-aware permissions
- API controllers move under `/api/v1/logistics/...` namespace

**Added (new):**
- Multi-tenancy infrastructure
- 15+ new bounded contexts / modules
- Storefront API + Blazor WASM storefront
- Payment gateway integrations (Midtrans, Xendit)
- Tax engine (PPN, e-Faktur)
- Background job infrastructure (Hangfire)
- Redis cache layer
- Object storage (MinIO)
- Notification subsystem (WhatsApp, SMS, Email)
- Reporting/BI engine

---

## 4. Multi-Tenancy Architecture

### 4.1 Isolation Strategy

**Single Database, Shared Schema, TenantId Discriminator**

Every tenant-scoped table carries a non-nullable `TenantId CHAR(36)` column as the first element of every composite index.

```csharp
// Shared kernel: all tenant-scoped entities implement this
public interface ITenantScoped
{
    Guid TenantId { get; }
}

public abstract class TenantAuditableEntity : ITenantScoped
{
    public Guid Id { get; protected set; }
    public Guid TenantId { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
}
```

### 4.2 Tenant Resolution

**Subdomain-first, header fallback for API consumers.**

```
Back-office:  {tenant-slug}.app.niagaone.id
Storefront:   {tenant-slug}.toko.niagaone.id  OR custom domain (CNAME)
API clients:  X-Tenant-Id header (validated against JWT claim)
```

```csharp
public class TenantResolutionMiddleware
{
    public async Task InvokeAsync(HttpContext context, ITenantStore tenantStore,
        ITenantContext tenantContext)
    {
        Guid? tenantId = null;

        // 1. Try subdomain
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);
        if (subdomain is not null)
            tenantId = await tenantStore.GetTenantIdBySlugAsync(subdomain);

        // 2. Try custom domain
        if (tenantId is null)
            tenantId = await tenantStore.GetTenantIdByDomainAsync(host);

        // 3. Try header (API clients) — validated against JWT claim
        if (tenantId is null && TryResolveFromHeader(context, out var headerTenantId))
            tenantId = headerTenantId;

        if (tenantId is null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant tidak dapat diidentifikasi." });
            return;
        }

        tenantContext.SetTenant(tenantId.Value);
        await _next(context);
    }
}
```

### 4.3 EF Core Data Isolation (Defense in Depth)

```csharp
// Automatic global query filter for every ITenantScoped entity
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            ApplyTenantFilter(modelBuilder, entityType.ClrType);
    }
}

// SaveChanges interceptor: stamps TenantId on new entities, prevents cross-tenant access
public override Task<int> SaveChangesAsync(...)
{
    foreach (var entry in ChangeTracker.Entries<ITenantScoped>()
        .Where(e => e.State == EntityState.Added))
        entry.Property(nameof(ITenantScoped.TenantId)).CurrentValue = _tenantContext.TenantId;

    foreach (var entry in ChangeTracker.Entries<ITenantScoped>()
        .Where(e => e.State is EntityState.Modified or EntityState.Deleted))
        if (entry.Entity.TenantId != _tenantContext.TenantId)
            throw new UnauthorizedAccessException("Cross-tenant data access denied.");

    return base.SaveChangesAsync(...);
}
```

### 4.4 Tenant Entity & Feature Flags

```csharp
public class Tenant
{
    public Guid Id { get; set; }
    public string Slug { get; set; }           // "tokomakmur"
    public string CompanyName { get; set; }
    public string? Npwp { get; set; }
    public string? CustomDomain { get; set; }
    public string Plan { get; set; }            // "starter" | "growth" | "enterprise"
    public string TimeZone { get; set; }        // "Asia/Jakarta"
    public string Locale { get; set; }          // "id-ID"
    public bool IsActive { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public TenantFeatures Features { get; set; } // JSON column
}

public class TenantFeatures
{
    public bool PosEnabled { get; set; }
    public bool EcommerceEnabled { get; set; }
    public bool ProcurementEnabled { get; set; }
    public bool CrmEnabled { get; set; }
    public bool LogisticsEnabled { get; set; } = true;
    public bool LoyaltyEnabled { get; set; }
    public int MaxBranches { get; set; }
    public int MaxUsers { get; set; }
    public int MaxProducts { get; set; }
}
```

---

## 5. Domain Model & Bounded Contexts

### 5.1 Bounded Context Map (8 Contexts)

```
┌─────────────────────────────────────────────────────────────┐
│                     BOUNDED CONTEXTS                         │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │ 1. PLATFORM  │  │ 2. CATALOG   │  │ 3. SALES &        │  │
│  │              │  │              │  │    COMMERCE        │  │
│  │ - Tenant     │  │ - Product    │  │ - SalesOrder      │  │
│  │ - Plan       │  │ - Category   │  │ - PosTransaction  │  │
│  │ - Billing    │  │ - Variant    │  │ - Cart            │  │
│  │ - PlatformUser│ │ - PriceList  │  │ - EcomOrder       │  │
│  │              │  │ - Brand      │  │ - Return          │  │
│  └──────────────┘  │ - Supplier   │  │ - Promotion       │  │
│                     └──────────────┘  └───────────────────┘  │
│  ┌──────────────┐                                            │
│  │ 4. INVENTORY │  ┌──────────────┐  ┌───────────────────┐  │
│  │              │  │ 5. FINANCE   │  │ 6. CRM            │  │
│  │ - StockItem  │  │              │  │                    │  │
│  │ - StockMove  │  │ - Invoice    │  │ - Customer        │  │
│  │ - StockCount │  │ - Payment    │  │ - Lead            │  │
│  │ - Warehouse  │  │ - TaxRecord  │  │ - Interaction     │  │
│  │ - Branch     │  │ - Expense    │  │ - Segment         │  │
│  │ - PurchaseOrd│  │ - Journal    │  │                    │  │
│  └──────────────┘  └──────────────┘  └───────────────────┘  │
│                                                              │
│  ┌──────────────┐  ┌──────────────────────────────────────┐  │
│  │ 7. LOGISTICS │  │ 8. SUPPORT SERVICES                  │  │
│  │ (existing)   │  │                                       │  │
│  │ - Shipment   │  │ - Notification (WA/SMS/Email)        │  │
│  │ - Driver     │  │ - FileStorage (images/docs)          │  │
│  │ - Vehicle    │  │ - AuditTrail                         │  │
│  │ - Tracking   │  │ - Reporting                          │  │
│  └──────────────┘  └──────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 5.2 Project Structure

```
src/
├── SharedKernel/
│   └── NiagaOne.SharedKernel/           # ITenantScoped, AuditableEntity, Money,
│                                            # DomainEvent, Result<T>, IUnitOfWork
├── Modules/
│   ├── Platform/
│   │   ├── NiagaOne.Platform.Domain/
│   │   ├── NiagaOne.Platform.Application/
│   │   ├── NiagaOne.Platform.Infrastructure/
│   │   └── NiagaOne.Platform.Api/
│   ├── Catalog/
│   │   ├── NiagaOne.Catalog.Domain/
│   │   ├── NiagaOne.Catalog.Application/
│   │   ├── NiagaOne.Catalog.Infrastructure/
│   │   └── NiagaOne.Catalog.Api/
│   ├── Sales/                              # POS + E-commerce + Orders
│   ├── Inventory/                          # Warehouse entity moves here
│   ├── Finance/                            # Invoicing + Payments + Tax
│   ├── Crm/
│   └── Logistics/                          # Existing code refactored here
│
├── Hosts/
│   ├── NiagaOne.Api.Host/               # Composite API host (all modules)
│   ├── NiagaOne.BackOffice/             # Blazor Server+WASM hybrid
│   ├── NiagaOne.Storefront/             # Blazor WASM (standalone)
│   └── NiagaOne.Worker/                 # Hangfire + background services
│
└── Tests/
    ├── NiagaOne.SharedKernel.Tests/
    ├── NiagaOne.Catalog.Tests/
    └── ...
```

### 5.3 Cross-Module Communication

Modules communicate via **Domain Events** (MediatR INotification), never direct DbContext access.

```csharp
// When a SalesOrder is confirmed:
public record OrderConfirmedEvent(
    Guid TenantId, Guid OrderId,
    IReadOnlyList<OrderLineItem> Items,
    Guid CustomerId, decimal TotalAmount
) : IDomainEvent;

// Inventory module handles it:
public class ReserveStockOnOrderConfirmed : INotificationHandler<OrderConfirmedEvent> { ... }

// Finance module handles it:
public class CreateInvoiceOnOrderConfirmed : INotificationHandler<OrderConfirmedEvent> { ... }
```

---

## 6. Module Dependency Architecture

### 6.1 Dependency Graph

```
                    ┌───────────┐
                    │ Platform  │  (depends on nothing)
                    └─────┬─────┘
                          │
                    ┌─────▼─────┐
                    │  Shared   │
                    │  Kernel   │
                    └─────┬─────┘
                          │
         ┌────────────────┼────────────────┐
         │                │                │
   ┌─────▼─────┐   ┌─────▼─────┐   ┌─────▼─────┐
   │  Catalog   │   │   IAM     │   │  Support  │
   └──┬────┬───┘   └───────────┘   └───────────┘
      │    │
      │    └──────────────────────┐
      │                           │
┌─────▼─────┐              ┌─────▼─────┐
│ Inventory  │              │   CRM     │
└──┬────┬───┘              └─────┬─────┘
   │    │                        │
   │    └────────────┐           │
┌──▼──────┐   ┌─────▼───────────▼──┐
│Logistics│   │      Sales         │
└─────────┘   └────────┬───────────┘
                       │
                 ┌─────▼─────┐
                 │  Finance   │
                 └─────┬─────┘
                       │
              ┌────────▼────────┐
              │  Promotions &   │
              │  Loyalty        │
              └─────────────────┘
```

---

## 7. API Architecture

### 7.1 URL Patterns

```
Back-office:  /api/v1/catalog/products
              /api/v1/sales/orders
              /api/v1/inventory/stock
              /api/v1/logistics/shipments     ← existing, moved under prefix

Storefront:   /store/v1/products              ← public browsing
              /store/v1/cart                   ← authenticated customer
              /store/v1/checkout
              /store/v1/orders/{id}/track

Webhooks:     /hooks/midtrans                 ← payment callbacks
              /hooks/xendit
```

### 7.2 Module Registration (Composite Host)

```csharp
// NiagaOne.Api.Host/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatformModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddFinanceModule(builder.Configuration);
builder.Services.AddCrmModule(builder.Configuration);
builder.Services.AddLogisticsModule(builder.Configuration);
builder.Services.AddSupportServices(builder.Configuration);

builder.Services.AddTenantInfrastructure();
builder.Services.AddRateLimiting();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapPlatformEndpoints();
app.MapCatalogEndpoints();
app.MapSalesEndpoints();
// ...
```

### 7.3 Rate Limiting Per Tenant

- Back-office: 200 requests/min per tenant (token bucket)
- Storefront: 60 requests/min per tenant+IP (sliding window)

---

## 8. Frontend Architecture

### 8.1 Back-Office (Blazor Hybrid — Extended)

Each module is a separate **Razor Class Library (RCL)** for lazy loading. Sidebar navigation driven by tenant's enabled modules and user permissions.

```csharp
public class ModuleRegistry
{
    public IReadOnlyList<ModuleNavItem> GetVisibleModules(TenantFeatures features, UserPermissions perms)
    {
        var modules = new List<ModuleNavItem>();
        modules.Add(new("Dashboard", "bi-speedometer", "/dashboard"));

        if (features.PosEnabled && perms.Has("sales:pos:access"))
            modules.Add(new("Kasir (POS)", "bi-cash-register", "/pos"));
        if (perms.Has("catalog:product:read"))
            modules.Add(new("Produk", "bi-box", "/catalog/products"));
        if (perms.Has("inventory:stock:read"))
            modules.Add(new("Inventori", "bi-boxes", "/inventory"));
        // ... etc
        return modules;
    }
}
```

### 8.2 Storefront (Blazor WASM Standalone)

Separate Blazor WASM application with prerendering for SEO. Tenant-aware theming via CSS custom properties loaded at startup.

```
NiagaOne.Storefront/
├── Pages/
│   ├── Home.razor                 # Featured products
│   ├── ProductList.razor          # Category browsing
│   ├── ProductDetail.razor        # Prerendered for SEO
│   ├── Cart.razor
│   ├── Checkout.razor             # Multi-step flow
│   ├── OrderTracking.razor        # Real-time tracking
│   └── Account/                   # Login, Register, History, Profile
├── Services/
│   ├── StorefrontApiClient.cs     # Typed HttpClient for /store/v1/*
│   ├── CartStateService.cs        # Local storage + server sync
│   └── TenantThemeService.cs      # Loads branding at startup
└── wwwroot/
```

---

## 9. Data Architecture

### 9.1 Table Naming Convention (Module Prefix)

```sql
-- Platform (no tenant_id)
platform_tenants, platform_users, platform_plans

-- Tenant-scoped (all have tenant_id)
iam_users, iam_roles, iam_permissions, iam_refresh_tokens
catalog_products, catalog_product_variants, catalog_categories, catalog_brands
sales_orders, sales_order_lines, sales_pos_transactions, sales_carts
inventory_branches, inventory_warehouses, inventory_stock_items, inventory_stock_movements
finance_invoices, finance_payments, finance_tax_lines, finance_expenses
crm_customers, crm_customer_addresses, crm_segments
logistics_shipments, logistics_drivers, logistics_vehicles  ← renamed from existing
promo_promotions, promo_coupons, loyalty_programs
notification_templates, notification_logs
audit_trail
```

### 9.2 Indexing Strategy

Every tenant-scoped table leads with `tenant_id` in composite indexes:

```sql
CREATE INDEX idx_products_tenant ON catalog_products (tenant_id);
CREATE INDEX idx_products_tenant_sku ON catalog_products (tenant_id, sku);
CREATE UNIQUE INDEX idx_products_tenant_slug ON catalog_products (tenant_id, slug);
CREATE INDEX idx_orders_tenant_status ON sales_orders (tenant_id, status, created_at DESC);
CREATE UNIQUE INDEX idx_stock_items_unique
    ON inventory_stock_items (tenant_id, warehouse_id, product_variant_id);
```

### 9.3 Caching Strategy

```
L1: In-Process (IMemoryCache)
    - Tenant config/features (5 min TTL)
    - Permission sets per user (5 min TTL)
    - Category trees (10 min TTL)

L2: Redis (IDistributedCache)
    - Product catalog pages (tenant:{id}:products)
    - Storefront branding (tenant:{id}:branding)
    - POS session state
    - Shopping cart (before login)
    - Rate limit counters

Cache Key Convention: tenant:{tenantId}:{module}:{entity}:{id}
Invalidation: Write-through on mutations + domain event handlers
```

---

## 10. Integration Architecture

### 10.1 Payment Gateway (Midtrans / Xendit)

Strategy pattern with unified abstraction:

```csharp
public interface IPaymentGateway
{
    string ProviderName { get; }
    Task<PaymentResult> CreateChargeAsync(PaymentRequest request);
    Task<PaymentResult> CheckStatusAsync(string gatewayTransactionId);
    Task<RefundResult> RefundAsync(string gatewayTransactionId, decimal amount);
    bool CanHandle(PaymentMethod method);
    PaymentNotification ParseWebhook(string body, IDictionary<string, string> headers);
}

public enum PaymentMethod
{
    Cash, BankTransfer, Qris, GoPay, Ovo, ShopeePay, CreditCard, Dana
}
```

**POS Payment Flow:**
```
Kasir scans items → Total displayed → Customer taps QRIS
→ System calls Midtrans CreateCharge (QRIS)
→ QR code displayed → Customer pays via GoPay/OVO/DANA
→ Midtrans webhook → System marks PosTransaction as paid
→ Receipt printed / sent via WhatsApp
```

### 10.2 Tax Engine (PPN / e-Faktur)

```csharp
public class IndonesianTaxEngine : ITaxEngine
{
    private const decimal PpnRate = 0.11m; // 11% PPN

    public TaxCalculation Calculate(TaxableTransaction transaction)
    {
        var dpp = transaction.SubTotal;
        var ppn = Math.Round(dpp * PpnRate, 0, MidpointRounding.AwayFromZero);
        return new TaxCalculation
        {
            Dpp = dpp, PpnRate = PpnRate, PpnAmount = ppn,
            Total = dpp + ppn,
            RequiresEfaktur = transaction.Tenant.IsRegisteredPkp
        };
    }
}
```

e-Faktur CSV export scheduled nightly for PKP tenants.

### 10.3 Notification Channels

- **WhatsApp** (primary) — via Fonnte API (popular, affordable in Indonesia)
- **Email** — SMTP / SendGrid
- **SMS** — Indonesian SMS gateway (fallback)
- **In-App** — notification center with unread count

### 10.4 Logistics (Indonesian Couriers)

- **RajaOngkir API** — multi-courier rate comparison
- **JNE, J&T, SiCepat, AnterAja** — direct API for AWB/resi generation
- **GoSend / GrabExpress** — same-day delivery (Phase 2)
- Volumetric weight: P x L x T / 6000

### 10.5 File Storage

MinIO (S3-compatible), path convention: `tenants/{tenantId}/{module}/{entity}/{id}/{filename}`

---

## 11. Security Architecture

### 11.1 Defense in Depth (6 Layers)

```
Layer 1: Tenant Resolution Middleware — rejects unidentifiable tenants
Layer 2: JWT Claims Validation — token contains tenant_id claim
Layer 3: EF Core Global Query Filters — WHERE tenant_id = @tenantId
Layer 4: SaveChanges Interceptor — stamps/validates TenantId
Layer 5: API Response Filtering — DTOs never expose TenantId
Layer 6: Periodic audit job — scan for cross-tenant data anomalies
```

### 11.2 Enhanced RBAC

Permission naming: `{module}:{resource}:{action}`
```
catalog:product:create, catalog:product:read
sales:order:create, sales:order:approve, sales:pos:access
finance:invoice:read, finance:payment:process
inventory:stock:adjust
settings:user:manage, settings:role:manage
```

Default tenant roles: TenantOwner, Manager, Kasir, Staff Gudang, Staff

### 11.3 PCI Compliance

Never handle raw card data. All card payments via Midtrans Snap / Xendit hosted payment page. System only stores transaction reference IDs and masked card numbers.

### 11.4 Audit Trail

Immutable, append-only via EF Core SaveChanges interceptor. Captures: who, what, when, where (IP), old/new values. Minimum retention: 10 years per Indonesian tax law (UU KUP).

---

## 12. Infrastructure & Deployment

### 12.1 Deployment Architecture

```
┌──────────────────────────────────────────────────────┐
│              PRODUCTION ENVIRONMENT                   │
│                                                       │
│  Load Balancer (nginx / cloud LB)                    │
│       ├─► API Host (2+ instances, Docker)            │
│       ├─► BackOffice Blazor Host (2+ instances)      │
│       └─► Storefront Static (CDN-served WASM)        │
│                                                       │
│  Worker Host (1-2 instances, Docker)                 │
│       └── Hangfire server + background services       │
│                                                       │
│  MySQL 8.0 (managed, primary + read replica)         │
│  Redis 7.x (managed, single node)                    │
│  MinIO (object storage)                              │
│                                                       │
│  Docker Compose for dev/staging                       │
│  Kubernetes when > 200 tenants                       │
└──────────────────────────────────────────────────────┘
```

### 12.2 Scaling Phases

| Phase | Tenants | Infra | Est. Cost/month |
|-------|---------|-------|-----------------|
| 1 | 1-50 | Single API + worker, MySQL single, Redis single | IDR 1.5-3M (~$100-200) |
| 2 | 50-200 | 2 API instances, MySQL primary+replica, separate worker | IDR 4.5-9M (~$300-600) |
| 3 | 200-1000 | Kubernetes, MySQL cluster, Redis Sentinel, CDN | IDR 15-45M (~$1K-3K) |
| 4 | 1000+ | DB-per-tier for premium, regional read replicas, Elasticsearch | IDR 45M+ |

### 12.3 Background Jobs (Hangfire)

Queues: `critical` (order processing), `default`, `reports`, `notifications`

Scheduled jobs:
- e-Faktur sync — daily 2 AM WIB
- Stock reorder alerts — hourly
- Abandoned cart reminders — every 4 hours
- Daily sales report generation

---

## 13. Migration Strategy

### 13.1 Database Migration (Non-Destructive)

**Phase 1:** Add TenantId to all 12 existing tables with default value
**Phase 2:** Rename tables to module prefix convention (Users → iam_users, etc.)
**Phase 3:** Add new module tables
**Phase 4:** Seed per-tenant defaults (roles, permissions, tax config)

```csharp
// Migration: backward-compatible TenantId addition
var defaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
foreach (var table in existingTables)
{
    migrationBuilder.AddColumn<Guid>("TenantId", table,
        nullable: false, defaultValue: defaultTenantId);
    migrationBuilder.CreateIndex($"IX_{table}_TenantId", table, "TenantId");
}
```

### 13.2 Feature Flag Strategy

Tenant-level flags in `TenantFeatures` JSON column. Global flags for platform-wide rollouts.

```csharp
public class FeatureFlagService
{
    public async Task<bool> IsEnabledAsync(string featureName)
    {
        // Check tenant-specific override first, then global flag
        var tenantFlags = await GetTenantFlagsAsync(_tenant.TenantId);
        if (tenantFlags.TryGetValue(featureName, out var val)) return val;
        return await GetGlobalFlagAsync(featureName);
    }
}
```

---

## 14. Implementation Sequence

### Recommended Build Order (16 Sprints, 2 weeks each)

```
Sprint 1-2:   FOUNDATION
              ├── SharedKernel (TenantId, AuditableEntity, Money, Result<T>)
              ├── Platform Module (Tenant CRUD, provisioning)
              ├── IAM Module (refactor existing Users/Roles/Permissions)
              ├── Multi-tenancy infrastructure (middleware, EF filters)
              └── Database migration (add TenantId to all existing tables)

Sprint 3-4:   CATALOG + INVENTORY BASE
              ├── Catalog Module (Product, Category, Variant, PriceList)
              ├── Inventory Module (Warehouse refactored, StockItem, Branch)
              └── File Storage integration (product images)

Sprint 5-6:   SALES CORE
              ├── CRM Module (Customer, Address)
              ├── Sales Module — POS (PosTransaction, in-store checkout)
              ├── Cash payment handling
              └── Receipt generation

Sprint 7-8:   FINANCE + PAYMENTS
              ├── Finance Module (Invoice, Payment, Journal)
              ├── Midtrans integration (QRIS, VA, GoPay)
              ├── Xendit integration (OVO, DANA, bank transfer)
              └── Tax engine (PPN calculation)

Sprint 9-10:  E-COMMERCE
              ├── Sales Module — E-commerce (Cart, Online Order, Checkout)
              ├── Storefront Blazor WASM app
              ├── Storefront API
              └── Customer authentication

Sprint 11-12: LOGISTICS INTEGRATION + PROMOTIONS
              ├── Logistics Module (refactor existing, connect to Sales)
              ├── Promotions Module (discounts, coupons)
              ├── Loyalty Module (points, tiers)
              └── Indonesian courier API integration

Sprint 13-14: ADVANCED FEATURES
              ├── Purchase/Procurement (PurchaseOrder, GoodsReceipt)
              ├── Stock transfers, stock counts
              ├── e-Faktur integration
              └── Notification system (WhatsApp, email)

Sprint 15-16: REPORTING + POLISH
              ├── Reporting/BI (sales, inventory, financial reports)
              ├── Dashboard widgets
              ├── Onboarding flow
              └── Performance optimization, load testing
```

---

## Appendix A: Key Entity Count Estimate

| Bounded Context | Entities | Tables |
|----------------|----------|--------|
| Platform | 4 | 4 |
| IAM | 6 | 6 |
| Catalog | 10 | 12 |
| Sales | 12 | 15 |
| Inventory | 10 | 12 |
| Finance | 8 | 10 |
| CRM | 5 | 6 |
| Logistics (existing) | 5 | 5 |
| Promotions & Loyalty | 6 | 7 |
| Support Services | 3 | 4 |
| **Total** | **~69** | **~81** |

## Appendix B: Shared Services

| Service | Interface | Purpose |
|---------|-----------|---------|
| Current User | `ICurrentUserService` | Extract user/tenant from HTTP context |
| Notifications | `INotificationService` | WhatsApp/SMS/Email dispatch |
| File Storage | `IFileStorageService` | Upload/download product images, invoices |
| Audit | `IAuditService` | Custom event logging |
| Domain Events | `IDomainEventDispatcher` | MediatR-based cross-module events |
| PDF Generation | `IPdfGenerator` | QuestPDF for invoices, receipts, reports |

---

*This document is the output of a brainstorming session and should be validated incrementally during implementation.*
