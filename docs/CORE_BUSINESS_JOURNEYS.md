# StockLedger — Core Business Journeys

> Version 2.0 | Updated: 2026-04-13
> End-to-end flows for Inventory + Finance SaaS

---

## Overview

StockLedger has **5 core business journeys** that represent the daily, weekly, and monthly workflows of an Indonesian multi-channel retailer. Each journey maps data flow from trigger to outcome.

```
Journey 1: "Set Up My Store"       ──  One-time setup (Admin)
Journey 2: "Stock the Shelves"     ──  Purchasing cycle (Manager + Warehouse)
Journey 3: "Record My Sales"       ──  Multi-channel sales import (Daily)
Journey 4: "Close the Books"       ──  Finance & tax reporting (Monthly)
Journey 5: "Check My Business"     ──  Dashboards & decisions (On-demand)
```

---

## Journey 1: "Set Up My Store" — Initial Configuration

**Actor:** Admin / Owner
**Frequency:** One-time (first 15 minutes after signup)
**Goal:** Configure StockLedger to match the retailer's business structure

```
Register Account (email + company name)
  │
  ├──► Create Warehouses
  │     └── Gudang Utama Jakarta (primary)
  │     └── Gudang Surabaya (secondary, if multi-branch)
  │
  ├──► Create Product Catalog
  │     ├── Categories (Electronics > Smartphones, Fashion > T-Shirts)
  │     ├── Brands (Samsung, Nike)
  │     ├── Units of Measure (Piece, Box, Kg)
  │     ├── Products (Galaxy S24, Nike T-Shirt)
  │     └── Variants (128GB Black @ Rp12.999.000, 256GB @ Rp14.999.000)
  │
  ├──► Configure Finance
  │     ├── Chart of Accounts (AR, AP, Revenue, COGS, Tax Payable)
  │     ├── Tax Rates (PPN 11%)
  │     └── Payment Terms (COD, Net 30, Net 60)
  │
  ├──► Set Up Team
  │     ├── Invite Warehouse Manager (inventory permissions)
  │     ├── Invite Accountant (finance permissions)
  │     └── Invite Staff (view-only permissions)
  │
  └──► Import Existing Data (optional)
        ├── Upload product CSV from marketplace seller center
        └── Set initial stock quantities per warehouse
```

**Outcome:** Store fully configured. Ready for purchasing, importing sales, and tracking finances.

**API Endpoints Used:**
- `POST /api/auth/register` — Create tenant
- `POST /api/warehouses` — Create warehouses
- `POST /api/categories` — Category hierarchy
- `POST /api/brands` — Brand management
- `POST /api/units` — Units of measure
- `POST /api/products` — Products
- `POST /api/products/{id}/variants` — Variants with pricing
- `POST /api/chart-of-accounts` — GL accounts
- `POST /api/tax-rates` — PPN rates
- `POST /api/payment-terms` — Payment terms

---

## Journey 2: "Stock the Shelves" — Purchasing & Receiving

**Actors:** Manager (creates PO), Warehouse Staff (receives goods)
**Frequency:** Weekly to bi-weekly
**Goal:** Order from suppliers and receive goods into warehouse with full traceability

```
Manager identifies low stock (reorder point alert or manual check)
  │
  ├──► Create Purchase Order
  │     ├── Select Supplier (PT Samsung Indonesia)
  │     ├── Select Warehouse (Gudang Utama Jakarta)
  │     ├── Add Line Items:
  │     │     ├── Galaxy S24 128GB × 50 @ Rp8.000.000 = Rp400.000.000
  │     │     └── Galaxy S24 256GB × 30 @ Rp9.000.000 = Rp270.000.000
  │     └── PO Status: Draft
  │
  ├──► Submit PO (Draft → Submitted)
  │     └── Notification sent to approver
  │
  ├──► Approve PO (Submitted → Approved)
  │     └── PO ready for receiving
  │
  ▼
Warehouse Staff receives physical shipment
  │
  ├──► Create Goods Receipt (linked to PO)
  │     ├── Scan/verify received quantities
  │     └── Note any discrepancies
  │
  └──► Confirm Goods Receipt
        ├── WarehouseStock.OnHand += received quantity
        ├── StockMovement created (Type: In, Reason: Purchase)
        ├── PO status → Received
        └── [Future] Auto-create AP journal entry
```

**Data Flow:**
```
Supplier ──► PurchaseOrder ──► GoodsReceipt ──► WarehouseStock
                                    │
                                    └──► StockMovement (audit trail)
                                    └──► [Future] JournalEntry (AP debit, Inventory credit)
```

**API Endpoints Used:**
- `POST /api/suppliers` — Supplier management
- `POST /api/purchase-orders` — Create PO
- `POST /api/purchase-orders/{id}/submit` — Submit for approval
- `POST /api/purchase-orders/{id}/approve` — Approve
- `POST /api/goods-receipts` — Create goods receipt
- `POST /api/goods-receipts/{id}/confirm` — Confirm receipt (triggers stock update)
- `GET /api/warehouse-stock` — Verify stock levels

---

## Journey 3: "Record My Sales" — Multi-Channel Sales Import

**Actor:** Owner / Warehouse Manager
**Frequency:** Daily or every 2-3 days
**Goal:** Import sales data from all channels into StockLedger to keep stock and revenue accurate

```
Retailer sells products across 3 channels today:
  ├── Tokopedia: 15 orders
  ├── Shopee: 12 orders
  └── Offline toko: 8 transactions
  │
  ▼
End of day / next morning:
  │
  ├──► Channel 1: Tokopedia
  │     ├── Export orders CSV from Tokopedia Seller Center
  │     ├── Upload CSV to StockLedger
  │     ├── Map columns (SKU, qty, price, platform fee, shipping)
  │     ├── StockLedger matches SKUs to ProductVariants
  │     ├── WarehouseStock.OnHand -= sold quantity (per variant)
  │     ├── StockMovement created (Type: Out, Reason: Sale, Channel: Tokopedia)
  │     └── Revenue + platform fee recorded for P&L
  │
  ├──► Channel 2: Shopee
  │     ├── Export orders CSV from Shopee Seller Center
  │     ├── Upload CSV to StockLedger
  │     ├── Same mapping & deduction process
  │     └── Shopee platform fees tracked separately
  │
  └──► Channel 3: Offline Toko
        ├── Enter sales manually (product, qty, price, payment method)
        ├── WarehouseStock.OnHand -= sold quantity
        ├── StockMovement created (Type: Out, Reason: Sale, Channel: Offline)
        └── No platform fee — full margin retained
  │
  ▼
Dashboard updates in real-time:
  ├── Total stock across all warehouses
  ├── Today's sales by channel
  ├── Revenue by channel (with and without platform fees)
  └── Low stock alerts (items below reorder point)
```

**Data Flow:**
```
CSV Upload ──► Parser (Tokopedia/Shopee/Manual)
                  │
                  ├──► SKU Matching ──► ProductVariant lookup
                  ├──► Stock Deduction ──► WarehouseStock update
                  ├──► Stock Movement ──► Immutable audit ledger
                  ├──► Revenue Record ──► Channel attribution
                  └──► Import Log ──► History & duplicate detection
```

**API Endpoints (To Build):**
- `POST /api/imports/upload` — Upload CSV file
- `POST /api/imports/{id}/map` — Map columns to fields
- `POST /api/imports/{id}/confirm` — Process import
- `GET /api/imports` — Import history
- `POST /api/sales/manual` — Manual offline entry
- `GET /api/dashboard/stock-summary` — Real-time stock overview

---

## Journey 4: "Close the Books" — Monthly Finance & Tax

**Actor:** Accountant / Finance Manager
**Frequency:** Monthly (end of month)
**Goal:** Generate financial reports and prepare tax compliance documents

```
Month-end trigger:
  │
  ├──► Review & Create Invoices
  │     ├── Generate invoices from sales records
  │     ├── Issue invoices (Draft → Issued)
  │     ├── Assign e-Faktur tax numbers (format: 010.000-24.XXXXXXXX)
  │     └── Track payment status (Issued → Paid)
  │
  ├──► Record Journal Entries
  │     ├── Sales revenue entries (Debit AR, Credit Revenue, Credit PPN)
  │     ├── Purchasing entries (Debit Inventory, Credit AP)
  │     ├── Verify all entries balanced (total debit = total credit)
  │     └── Post entries (Draft → Posted)
  │
  ├──► Generate Financial Reports
  │     ├── Profit & Loss Statement
  │     │     ├── Revenue by channel (Tokopedia, Shopee, Offline)
  │     │     ├── COGS (from purchase costs)
  │     │     ├── Platform fees (per channel)
  │     │     ├── Gross margin per channel
  │     │     └── Net profit
  │     ├── Margin per Product Report
  │     │     ├── Selling price vs cost price
  │     │     ├── Platform fee allocation
  │     │     └── True margin per SKU
  │     └── [P1] Balance Sheet & Cash Flow
  │
  └──► Tax Compliance
        ├── PPN Output summary (from sales invoices)
        ├── PPN Input summary (from purchase invoices)
        ├── Net PPN payable = Output - Input
        └── Export for DJP e-Faktur submission
```

**Data Flow:**
```
Sales + Purchases ──► Invoices ──► Journal Entries ──► Financial Reports
                        │                                    │
                        └──► Tax Numbers (e-Faktur)          └──► P&L, Margin, PPN Summary
```

**API Endpoints Used:**
- `POST /api/invoices` — Create invoice
- `POST /api/invoices/{id}/issue` — Issue invoice
- `PUT /api/invoices/{id}/tax-number` — Assign e-Faktur number
- `POST /api/invoices/{id}/mark-paid` — Record payment
- `POST /api/journal-entries` — Create journal entry
- `POST /api/journal-entries/{id}/post` — Post entry
- `GET /api/reports/profit-loss` — P&L report (to build)
- `GET /api/reports/margin-per-product` — Margin report (to build)
- `GET /api/reports/ppn-summary` — PPN summary (to build)

---

## Journey 5: "Check My Business" — Dashboards & Decisions

**Actor:** Owner / any authorized user
**Frequency:** On-demand (daily quick check, weekly deep review)
**Goal:** Get actionable business intelligence at a glance

```
Owner opens StockLedger dashboard:
  │
  ├──► Stock Health Panel
  │     ├── Total SKUs: 847 active
  │     ├── Low stock alerts: 12 items below reorder point
  │     ├── Overstock warnings: 5 items > 3 months supply
  │     ├── Stock value: Rp2.3B (at cost)
  │     └── Stock by warehouse breakdown
  │
  ├──► Sales Performance Panel
  │     ├── This week's sales: Rp145M (across all channels)
  │     ├── Channel breakdown:
  │     │     ├── Tokopedia: Rp68M (47%) — platform fee: Rp8.2M
  │     │     ├── Shopee: Rp52M (36%) — platform fee: Rp7.8M
  │     │     └── Offline: Rp25M (17%) — no platform fee
  │     └── Top 5 selling products this week
  │
  ├──► Profitability Panel
  │     ├── Gross margin this month: 34%
  │     ├── Margin by channel:
  │     │     ├── Tokopedia: 28% (after 12% platform fee)
  │     │     ├── Shopee: 22% (after 15% platform fee)
  │     │     └── Offline: 45% (no platform fee)
  │     ├── Most profitable products (top 10)
  │     └── Loss-making products (margin < 5%)
  │
  └──► Action Items
        ├── "Reorder Galaxy S24 128GB — only 8 left, sells 20/week"
        ├── "Nike T-Shirt Red M hasn't sold in 45 days — consider promotion"
        └── "Shopee fees increased 2% — review pricing on 15 SKUs"
```

**API Endpoints (To Build):**
- `GET /api/dashboard/stock-health` — Stock summary + alerts
- `GET /api/dashboard/sales-performance` — Sales by channel
- `GET /api/dashboard/profitability` — Margin analysis
- `GET /api/dashboard/action-items` — Smart recommendations

---

## Journey Summary

| Journey | Who | When | Core Modules | Status |
|---------|-----|------|-------------|--------|
| 1. Set Up My Store | Admin | One-time | Auth, Catalog, Warehouses, Finance setup | **Mostly built** |
| 2. Stock the Shelves | Manager + Warehouse | Weekly | Purchasing, Inventory, Stock Movements | **Built** |
| 3. Record My Sales | Owner / Manager | Daily | CSV Import, Stock Deduction, Revenue | **To build (P0)** |
| 4. Close the Books | Accountant | Monthly | Invoicing, Journal Entries, Reports, Tax | **Partially built** |
| 5. Check My Business | Owner | On-demand | Dashboards, Analytics, Alerts | **To build (P1)** |

---

## What's NOT a Journey (Explicit Non-Goals for v1)

| Excluded Flow | Why |
|---------------|-----|
| POS checkout flow | Retailers keep their existing POS. We import the results. |
| E-commerce storefront / cart | We don't sell products. We track what was sold elsewhere. |
| Customer management (CRM) | No customer-facing features. We're back-office only. |
| Employee / HR management | Out of scope. Not our product. |
| Loyalty / promotions | Not our product. Marketplaces handle this. |
| Shipment / delivery tracking | Retailers use marketplace logistics or their own couriers. |
