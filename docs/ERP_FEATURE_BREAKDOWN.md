# StockLedger — Feature Breakdown

> Version 2.0 | Updated: 2026-04-13
> Focused on Inventory + Finance for Indonesian mid-upper retailers

---

## Priority Legend

| Priority | Meaning | Timeline |
|----------|---------|----------|
| **P0** | Must-have for launch | Build now |
| **P1** | Important for paid conversion | Build within 3 months post-launch |
| **P2** | Nice-to-have, builds moat | Build within 6-12 months |
| **P3** | Future expansion | Only if validated demand |

## Status Legend

| Status | Meaning |
|--------|---------|
| **Built** | Implemented and tested |
| **Partial** | Data model exists, needs API/UI work |
| **To build** | Not started |

---

## Module 1: Foundation & Platform

### 1.1 Multi-Tenancy [P0] — Built
- [x] Single-database TenantId isolation
- [x] Global EF Core query filters
- [x] SaveChanges interceptor (auto-stamp + cross-tenant protection)
- [x] Tenant registration (creates tenant + admin user)
- [x] Tenant settings (company profile, regional config)
- [x] Default tenant seeding

### 1.2 Authentication [P0] — Built
- [x] JWT access tokens (HS256, 15-min expiry)
- [x] Refresh token rotation (SHA256 hashed, 7-day expiry)
- [x] Login / Register / Logout / Logout-all
- [x] BCrypt password hashing
- [x] Tenant resolution from JWT claims or X-Tenant-Id header

### 1.3 Authorization (RBAC) [P0] — Built
- [x] Role-based access control
- [x] 4 built-in roles (Admin, Manager, Driver, Viewer)
- [x] 70+ granular permissions across all modules
- [x] Custom role creation via API
- [x] Permission-based authorization policies

### 1.4 Audit & Compliance [P0] — Built
- [x] Immutable audit log (who, what, when, field-level changes)
- [x] System log for errors/events
- [x] Idempotency middleware (prevent duplicate requests)

---

## Module 2: Product Catalog

### 2.1 Categories [P0] — Built
- [x] Hierarchical categories (parent-child)
- [x] Category slug auto-generation
- [x] Sort ordering
- [x] Soft delete

### 2.2 Brands [P0] — Built
- [x] Brand CRUD with slug
- [x] Logo URL support
- [x] Active/inactive status

### 2.3 Units of Measure [P0] — Built
- [x] Unit CRUD (Piece, Box, Kg, etc.)
- [x] Abbreviations
- [x] Unit conversions (e.g., 1 Box = 12 Pieces)

### 2.4 Products [P0] — Built
- [x] Product CRUD with rich metadata
- [x] Category + Brand association
- [x] Product status (Active, Inactive, Draft, Discontinued)
- [x] Soft delete

### 2.5 Product Variants [P0] — Built
- [x] Multiple variants per product (size, color, storage)
- [x] Independent SKU per variant
- [x] Cost price + selling price per variant
- [x] Barcode support
- [x] Weight tracking

### 2.6 Product Images [P0] — Built
- [x] Multiple images per product
- [x] Image ordering / sort

---

## Module 3: Inventory & Warehousing

### 3.1 Warehouse Management [P0] — Built
- [x] Multi-warehouse support
- [x] Warehouse code, name, address
- [x] Active/inactive status

### 3.2 Stock Tracking [P0] — Built
- [x] Per-warehouse, per-variant stock tracking
- [x] Quantity on-hand, quantity reserved, quantity available
- [x] Reorder point and max stock level fields
- [x] Optimistic concurrency (row versioning)

### 3.3 Stock Movements [P0] — Built
- [x] Immutable movement ledger (append-only)
- [x] Movement types: In, Out, Transfer, Adjustment
- [x] Movement reasons: Sale, Purchase, Transfer, Adjustment, Return
- [x] Quantity before/after tracking
- [x] Reference document linking (PO, GR, etc.)

### 3.4 CSV/Import Engine [P0] — To Build
- [ ] CSV file upload endpoint
- [ ] Column mapping UI (match CSV columns to StockLedger fields)
- [ ] Tokopedia order export parser (auto-detect format)
- [ ] Shopee order export parser (auto-detect format)
- [ ] Manual offline sales entry form
- [ ] SKU matching (CSV SKU → ProductVariant lookup)
- [ ] Auto stock deduction on import confirmation
- [ ] Import history log
- [ ] Duplicate detection (order ID / reference number)
- [ ] Import error report (unmatched SKUs, format issues)

### 3.5 Stock Reconciliation [P1] — To Build
- [ ] Physical count entry (stocktake)
- [ ] Variance report (system vs physical)
- [ ] Adjustment entries from reconciliation
- [ ] Reconciliation history

### 3.6 Inter-Warehouse Transfer [P1] — To Build
- [ ] Transfer request (source → destination)
- [ ] Transfer confirmation
- [ ] Stock movement created for both warehouses
- [ ] Transfer history

### 3.7 Advanced Inventory [P2] — Not Started
- [ ] Batch/lot number tracking
- [ ] Expiry date tracking (FEFO)
- [ ] Serial number tracking
- [ ] Bin/location management within warehouse

---

## Module 4: Purchasing & Procurement

### 4.1 Supplier Management [P0] — Built
- [x] Supplier CRUD (name, code, contact, address)
- [x] Active/inactive status

### 4.2 Purchase Orders [P0] — Built
- [x] PO creation with line items
- [x] Status lifecycle: Draft → Submitted → Approved → Received
- [x] Multi-item support with quantities and unit prices

### 4.3 Goods Receipt [P0] — Built
- [x] Goods receipt linked to PO
- [x] Line-by-line received quantity entry
- [x] Confirmation triggers:
  - [x] WarehouseStock auto-update
  - [x] StockMovement records created
  - [x] PO status transition

### 4.4 Purchase Cost Tracking [P1] — To Build
- [ ] Landed cost calculation (purchase price + shipping + duties)
- [ ] Cost history per variant (price trend over time)
- [ ] Automatic COGS calculation from average cost

### 4.5 Supplier Analytics [P2] — Not Started
- [ ] Lead time tracking (order to receipt)
- [ ] Fulfillment rate (ordered vs received)
- [ ] Price comparison across suppliers

---

## Module 5: Finance & Accounting

### 5.1 Chart of Accounts [P0] — Built
- [x] Account CRUD (code, name, type, sub-type)
- [x] Account types: Asset, Liability, Equity, Revenue, Expense
- [x] Parent-child hierarchy
- [x] Normal balance (debit/credit)
- [x] System account flag

### 5.2 Journal Entries [P0] — Built
- [x] Double-entry bookkeeping
- [x] Multi-line entries with debit/credit
- [x] Balance validation (total debit = total credit)
- [x] Status lifecycle: Draft → Posted → Voided
- [x] Reference document linking
- [x] Entry numbering

### 5.3 Invoicing [P0] — Built
- [x] Invoice creation with line items
- [x] Status lifecycle: Draft → Issued → Paid → Cancelled
- [x] e-Faktur tax invoice numbering
- [x] Payment term association
- [x] Mark-as-paid with payment date and method
- [x] Tax amount calculation

### 5.4 Tax Management [P0] — Built
- [x] PPN 11% tax rate management
- [x] Tax rate effective dates
- [x] Product-to-tax-rate mapping
- [x] Tax type classification

### 5.5 Payment Terms [P0] — Built
- [x] Payment term CRUD (COD, Net 30, Net 60)
- [x] Due days configuration

### 5.6 Financial Reports [P0/P1] — To Build
- [ ] **[P0]** Profit & Loss statement (monthly/quarterly)
- [ ] **[P0]** Revenue by channel (Tokopedia, Shopee, Offline)
- [ ] **[P0]** Margin per product report
- [ ] **[P0]** Margin per channel report (including platform fees)
- [ ] **[P1]** Balance sheet
- [ ] **[P1]** Cash flow statement
- [ ] **[P1]** PPN input/output summary (for DJP filing)
- [ ] **[P1]** Accounts receivable aging
- [ ] **[P1]** Accounts payable aging

### 5.7 Auto Journal Entries [P1] — To Build
- [ ] Auto-create journal entry from goods receipt (inventory + AP)
- [ ] Auto-create journal entry from invoice payment (AR + revenue)
- [ ] Reversing entries for voided transactions

---

## Module 6: Dashboards & Analytics

### 6.1 Stock Dashboard [P1] — To Build
- [ ] Total stock value (at cost)
- [ ] Stock level by warehouse
- [ ] Low stock alerts (below reorder point)
- [ ] Overstock warnings (> X months supply)
- [ ] Top movers (fastest selling SKUs)
- [ ] Dead stock (no movement in 30/60/90 days)

### 6.2 Sales Dashboard [P1] — To Build
- [ ] Daily/weekly/monthly sales volume
- [ ] Sales by channel breakdown
- [ ] Platform fee tracking per channel
- [ ] Top selling products
- [ ] Sales trend charts

### 6.3 Profitability Dashboard [P1] — To Build
- [ ] Gross margin (overall, per channel, per product)
- [ ] Net margin after platform fees
- [ ] Most profitable products (top 10)
- [ ] Loss-making products (margin < threshold)
- [ ] Channel profitability comparison

### 6.4 Smart Recommendations [P2] — Not Started
- [ ] Reorder suggestions based on sales velocity
- [ ] Price optimization hints (margin too low / too high)
- [ ] Channel performance comparison
- [ ] Seasonal demand patterns

---

## Feature Count Summary

| Module | P0 (Launch) | P1 (3 months) | P2 (6-12 months) | Total |
|--------|-------------|---------------|-------------------|-------|
| Foundation & Platform | 18 (built) | 0 | 0 | 18 |
| Product Catalog | 16 (built) | 0 | 0 | 16 |
| Inventory & Warehousing | 14 built + 10 to build | 7 | 4 | 35 |
| Purchasing & Procurement | 9 (built) | 4 | 3 | 16 |
| Finance & Accounting | 18 built + 4 to build | 9 | 0 | 31 |
| Dashboards & Analytics | 0 | 15 | 4 | 19 |
| **Total** | **89** | **35** | **11** | **135** |

### Build Status

| Status | Count |
|--------|-------|
| Built and tested | 75 features |
| To build for launch (P0) | 14 features |
| To build post-launch (P1) | 35 features |
| Future (P2) | 11 features |

**StockLedger is ~55% built for a launchable v1.** The remaining P0 work is focused on the CSV import engine and core financial reports — the two features that directly deliver the primary value proposition.

---

## Explicitly OUT OF SCOPE

These features were part of the old NiagaOne full-ERP plan. They are **not** part of StockLedger:

| Module | Reason Excluded |
|--------|----------------|
| POS System | Retailers keep their existing POS. We import results. |
| E-Commerce Storefront | We're not a marketplace competitor. |
| Shopping Cart / Checkout | Not our product. |
| Customer Management (CRM) | Back-office only — no customer-facing features. |
| Loyalty Programs | Marketplaces handle this. |
| Promotions / Coupons | Not our product. |
| HRM (Employees, Attendance, Leave, Payroll) | Completely different product domain. |
| Notifications (Email, WhatsApp, Push) | Deferred to P3 at earliest. |
| Webhooks | Deferred to P2 (API access for Enterprise tier). |
| Driver / Vehicle Management | Legacy from original LogisticHub — removed. |
| Shipment Tracking | Handled by marketplace logistics. |
