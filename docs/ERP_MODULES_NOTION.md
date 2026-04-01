# NiagaOne ERP — Complete Module Map & Build Phases

## Overview

NiagaOne is being evolved from a logistics management app into a **SaaS multi-tenant ERP platform** targeting Indonesian mid-market retail businesses operating physical stores and e-commerce channels.

- **Target Market:** Indonesian mid-market (20–1,000 employees)
- **First Release:** Store (Physical Retail) + Online Shop (E-commerce)
- **Architecture:** Modular Monolith, Clean Architecture, .NET 8
- **Database:** MySQL 8.0, single-database multi-tenancy with TenantId
- **Frontend:** Blazor Server + WebAssembly (Back-office) / Blazor WASM (Storefront)

---

## ERP Module Map

### Tier 1 — Foundation

> Must exist before anything else. Every other module depends on these.

---

**Module 1 · SharedKernel**

- **Status:** 🆕 New
- **Purpose:** Base types, interfaces, and value objects shared across all modules
- **Key Components:**
    - `ITenantScoped` — interface for tenant data isolation
    - `TenantAuditableEntity` — base class with Id, TenantId, timestamps, soft delete
    - `ITenantContext` — scoped service holding current tenant
    - `Money` — value object for IDR (no decimals, Rp formatting)
    - `Result<T>` — unified result pattern for use cases
    - `IDomainEvent` — base interface for cross-module events (MediatR)
- **Dependencies:** None
- **Estimated Entities:** 0 (interfaces and value objects only)

---

**Module 2 · Platform**

- **Status:** 🆕 New
- **Purpose:** Tenant lifecycle management, subscription plans, provisioning, super-admin operations
- **Key Entities:**
    - `Tenant` — core tenant record (slug, company name, NPWP, NIB, plan, timezone, locale, branding)
    - `TenantFeatures` — feature flags per subscription tier (JSON column)
    - `TenantSubscription` — subscription plan, start/end dates, billing status
    - `PlatformUser` — super-admin accounts (not tenant-scoped)
- **Key Services:**
    - Tenant provisioning (create tenant + seed roles/permissions/warehouse)
    - Tenant store (resolve TenantId from slug/domain, cached in Redis)
    - Subscription enforcement (gate features by plan)
- **Dependencies:** SharedKernel
- **Estimated Entities:** 4

---

**Module 3 · IAM (Identity & Access Management)**

- **Status:** ♻️ Refactor from existing
- **Purpose:** Authentication, RBAC, tenant-scoped users/roles/permissions
- **Existing Entities (Modified):**
    - `User` — add TenantId, tenant-scoped unique email
    - `Role` — add TenantId, roles become per-tenant (seeded on provisioning)
    - `Permission` — module-aware naming (`catalog:product:create`, `sales:pos:access`)
    - `UserRoleAssignment` — add TenantId
    - `RolePermission` — add TenantId
    - `RefreshToken` — add TenantId
- **Key Changes:**
    - JWT claims get `tenant_id` added
    - Login resolves tenant context
    - Permission naming evolves to `{module}:{resource}:{action}` format
    - Default roles per tenant: TenantOwner, Manager, Kasir, Staff Gudang, Staff
- **Dependencies:** SharedKernel, Platform
- **Estimated Entities:** 6 (all existing, modified)

---

### Tier 2 — Core Commerce

> Revenue-generating modules. Products, stock, orders, and online storefront.

---

**Module 4 · Catalog**

- **Status:** 🆕 New
- **Purpose:** Central product master data consumed by POS, e-commerce, inventory, purchasing, and reporting
- **Key Entities:**
    - `Product` — name, description (plain + rich HTML), SKU, type (Simple/Variable/Service/Bundle), status
    - `ProductVariant` — variant per product (Size: S/M/L, Color: Red/Blue), independent SKU/price/stock
    - `Category` — self-referential tree (parent_id) for multi-level hierarchy
    - `Brand` — brand master data
    - `PriceList` — named price lists (Retail, Wholesale, Online, Member)
    - `VariantPrice` — price per variant per price list
    - `ProductImage` — multiple images per product with ordering and primary flag
    - `Supplier` — supplier master data (name, NPWP, bank account, payment terms)
    - `SupplierProduct` — supplier-specific pricing per product
- **Indonesia-Specific:**
    - Bilingual names (Bahasa Indonesia + English)
    - IDR pricing format "Rp 1.500.000"
    - Halal certification (MUI), BPOM registration, SNI certification fields
    - Barcode: EAN-13, UPC-A, internal
    - UOM: pcs, kg, liter, meter, box, carton, dozen
- **Dependencies:** SharedKernel
- **Estimated Entities:** 10

---

**Module 5 · Inventory**

- **Status:** 🆕 New (absorbs existing Warehouse entity)
- **Purpose:** Multi-warehouse stock tracking with batch/lot, serial numbers, expiry, FEFO, and automated reordering
- **Key Entities:**
    - `Branch` — physical location (store/office), hierarchy (HQ → regional → branch)
    - `Warehouse` — ♻️ existing entity moved here, extended with BranchId and capacity details
    - `StockItem` — stock level per ProductVariant per Warehouse (qty_on_hand, qty_reserved, qty_available)
    - `StockMovement` — immutable ledger (type: IN/OUT/TRANSFER/ADJUSTMENT/RETURN, source/dest warehouse, reference)
    - `StockCount` — stocktake session header
    - `StockCountLine` — individual count entries with variance
    - `StockTransfer` — inter-warehouse transfer order
    - `PurchaseOrder` — PO to supplier (line items, approval status, delivery date)
    - `PurchaseOrderLine` — PO line item detail
    - `GoodsReceipt` — receiving against PO (partial receiving supported)
- **Key Capabilities:**
    - FIFO / weighted average valuation
    - Low stock alerts with configurable thresholds
    - Batch/lot tracking (assign on receipt, track to sale)
    - Expiry tracking with FEFO picking
    - Serial number tracking for high-value items
    - Auto-reorder point (triggers purchase requisition)
    - Barcode operations (scan to receive/pick/adjust/transfer)
    - Bin/location management (aisle-rack-shelf-bin)
- **Indonesia-Specific:**
    - FEFO mandatory for food products per BPOM regulation
    - Indonesian logistics hub cities as warehouse locations
- **Dependencies:** SharedKernel, Catalog
- **Estimated Entities:** 10

---

**Module 6 · Sales**

- **Status:** 🆕 New
- **Purpose:** Unified order management for all channels — POS terminal, e-commerce checkout, manual/phone orders
- **Key Entities:**
    - `SalesOrder` — unified order (customer, lines, shipping address, source: POS/Ecom/Manual/Marketplace)
    - `SalesOrderLine` — line item (product variant, qty, price, discount, tax)
    - `OrderStatusHistory` — status change log with timestamps
    - `PosTransaction` — in-store POS sale (register, cashier, branch)
    - `PosTransactionLine` — POS line items
    - `PosPayment` — split payment support (cash + QRIS on same transaction)
    - `Cart` — e-commerce shopping cart
    - `CartItem` — cart line items
    - `Return` — return/refund header (RMA)
    - `ReturnLine` — return line items with reason
    - `Quotation` — proforma invoice / quote (converts to SalesOrder)
- **Key Capabilities:**
    - POS: session management, barcode scan, split payment, receipt printing, hold/recall, offline mode
    - E-commerce: cart persistence, guest checkout, multi-step checkout flow
    - Order lifecycle: Pending → Confirmed → Processing → Shipped → Delivered → Completed
    - Auto inventory reservation on confirmation
    - Fulfillment workflow (pick list → pack → ship)
    - Return/refund workflow (RMA → approve → receive → inspect → refund/replace)
- **Indonesia-Specific:**
    - Cash rounding to nearest Rp100
    - QRIS payment at POS
    - PPN line on receipt per DJP requirement
    - COD order type
    - Indonesian address format (Jalan, RT/RW, Kelurahan, Kecamatan, Kota, Provinsi, Kode Pos)
- **Dependencies:** SharedKernel, Catalog, Inventory, CRM
- **Estimated Entities:** 12

---

**Module 7 · Storefront**

- **Status:** 🆕 New
- **Purpose:** Customer-facing online shop — product browsing, cart, checkout, account management, order tracking
- **Technology:** Blazor WebAssembly (standalone) with prerendering for SEO
- **Key Pages:**
    - Home (featured products, banners)
    - Product listing (category filter, price filter, search)
    - Product detail (image gallery, variant selector, stock status, reviews)
    - Cart & Checkout (multi-step: address → shipping → payment → confirmation)
    - Customer account (order history, tracking, addresses, profile)
    - Static pages (About, Contact, FAQ, Privacy Policy, Terms)
- **Key Capabilities:**
    - Tenant-branded (logo, colors, theme via CSS custom properties)
    - Mobile-first responsive (70%+ Indonesian mobile traffic)
    - SEO: meta titles, clean URLs, sitemap.xml, JSON-LD structured data
    - WhatsApp "Chat dengan Penjual" button
    - Guest checkout support
    - Wishlist / save for later
- **Indonesia-Specific:**
    - Bahasa Indonesia default
    - IDR format "Rp 1.500.000"
    - Courier display (JNE, J&T, SiCepat, AnterAja)
    - Ongkos kirim calculator
    - Holiday theming (Ramadan, Lebaran, Harbolnas 12.12)
- **Dependencies:** SharedKernel, Catalog, Inventory, Sales, Payment Gateway
- **Estimated Entities:** Lightweight (mostly consumes other module APIs)

---

### Tier 3 — Financial

> Compliance and money tracking. Required for legal operation in Indonesia.

---

**Module 8 · Finance**

- **Status:** 🆕 New
- **Purpose:** Invoicing, payment recording, expense tracking, basic double-entry journal
- **Key Entities:**
    - `Invoice` — auto-generated or manual (number, date, due date, customer, status, payment terms)
    - `InvoiceLine` — line items with product, qty, price, discount, tax
    - `Payment` — payment record linked to invoice(s), method, gateway reference, status
    - `PaymentGatewayLog` — raw gateway webhook/response log for debugging
    - `JournalEntry` — double-entry bookkeeping header (auto-generated from transactions)
    - `JournalLine` — debit/credit entries per account
    - `BankAccount` — bank accounts for reconciliation
    - `Expense` — operational expenses (category, amount, receipt, approval status)
- **Key Capabilities:**
    - Auto-generate invoice from completed order/POS transaction
    - Configurable numbering (INV/2026/03/00001)
    - Invoice PDF generation with tenant branding
    - Payment terms (Due on receipt, Net 7/14/30/60)
    - Partial payment, over/underpayment handling
    - Credit note / debit note
    - AR aging report
    - Expense approval workflow
    - Simplified P&L (revenue − COGS − expenses)
- **Indonesia-Specific:**
    - Faktur Pajak format compliance
    - Terbilang: total in Indonesian words ("Satu Juta Lima Ratus Ribu Rupiah")
    - NPWP of seller and buyer on invoice
    - Materai digital for invoices above Rp 5.000.000
- **Dependencies:** SharedKernel, Sales, CRM
- **Estimated Entities:** 8

---

**Module 9 · Tax**

- **Status:** 🆕 New
- **Purpose:** PPN calculation, e-Faktur generation, NSFP management, PPh withholding tracking
- **Key Entities:**
    - `TaxRecord` — tax calculation per transaction
    - `TaxLine` — individual tax line items (DPP, PPN amount, rate)
    - `EfakturRecord` — e-Faktur submission tracking per invoice
    - `NsfpAllocation` — Nomor Seri Faktur Pajak range management
    - `TaxPeriod` — monthly tax period summary (Masa Pajak)
- **Key Capabilities:**
    - PPN engine: 11% (configurable for 12% increase)
    - Tax-inclusive and tax-exclusive pricing modes
    - Product tax categories (Standard, Exempt, Not Collected, Non-PPN)
    - Output PPN (sales) vs Input PPN (purchases) tracking
    - Monthly PPN summary report
    - NSFP allocation and tracking
    - e-Faktur CSV export (DJP compatible format)
    - PPh 23 withholding on service purchases (2%)
    - PPh Final 0.5% for UMKM (< Rp 4.8B revenue)
- **Indonesia-Specific:**
    - PPN rate per UU HPP
    - PKP compliance (mandatory > Rp 4.8B annual revenue)
    - DPP calculation rules per Indonesian regulation
    - Tax invoice numbering per DJP format: 010.000-26.00000001
    - Coretax DJP integration readiness
- **Dependencies:** SharedKernel, Finance, Sales
- **Estimated Entities:** 5

---

**Module 10 · Payment Gateway**

- **Status:** 🆕 New
- **Purpose:** Integration with Midtrans and Xendit for online payments, QRIS, VA, e-wallets
- **Key Entities:**
    - `PaymentTransaction` — gateway transaction record (reference ID, method, amount, status)
    - `PaymentWebhookLog` — raw webhook payload log (immutable, for debugging/audit)
    - `TenantPaymentConfig` — per-tenant gateway configuration (API keys encrypted, sandbox mode, enabled methods)
- **Key Capabilities:**
    - Strategy pattern: `IPaymentGateway` with pluggable Midtrans/Xendit adapters
    - QRIS generation and confirmation
    - Virtual Account per bank (BCA, Mandiri, BRI, BNI, Permata, CIMB)
    - E-wallet: GoPay, OVO, ShopeePay, Dana, LinkAja
    - Credit/debit card via hosted tokenization (PCI handled by gateway)
    - Webhook handling with signature verification
    - Refund processing
    - Convenience store payment (Alfamart, Indomaret)
- **POS Payment Flow:**
    - Kasir scans items → total displayed → customer taps QRIS
    - System calls Midtrans CreateCharge → QR displayed
    - Customer pays via GoPay/OVO/DANA → webhook confirms
    - PosTransaction marked paid → receipt generated
- **Dependencies:** SharedKernel, Finance
- **Estimated Entities:** 3

---

### Tier 4 — Customer & Growth

> Drive repeat purchases, targeted promotions, and customer retention.

---

**Module 11 · CRM**

- **Status:** 🆕 New
- **Purpose:** Unified customer database across POS (walk-in) and online (registered), segmentation, interaction tracking
- **Key Entities:**
    - `Customer` — master record (name, email, phone/WhatsApp, type: retail/wholesale/member)
    - `CustomerAddress` — multiple shipping/billing addresses per customer
    - `CustomerSegment` — segment definitions (VIP, Regular, Wholesale, New, Inactive)
    - `SegmentMember` — customer-to-segment mapping
    - `Interaction` — interaction log (call, visit, complaint, note)
    - `Lead` — potential customer tracking
- **Key Capabilities:**
    - Unified profile across POS and e-commerce (phone/email deduplication)
    - Purchase history aggregation across all channels
    - Customer groups/segments with manual and rule-based membership
    - Credit limit for B2B/wholesale customers
    - Customer balance (store credit from returns)
    - CSV/Excel import for migration
- **Indonesia-Specific:**
    - Indonesian phone (+62/08xx) as primary identifier
    - WhatsApp number field (critical for communication)
    - KTP number for B2B/wholesale
    - Indonesian address structure (Provinsi, Kota, Kecamatan, Kelurahan, Kode Pos, RT/RW)
- **Dependencies:** SharedKernel
- **Estimated Entities:** 6

---

**Module 12 · Promotions**

- **Status:** 🆕 New
- **Purpose:** Discounts, coupons, flash sales, bundle pricing, and promotion rules engine
- **Key Entities:**
    - `Promotion` — promotion header (name, type, schedule, status, stacking rules)
    - `PromotionRule` — conditions and actions (min purchase, target products/categories, discount type/amount)
    - `Coupon` — coupon/voucher codes (unique code, usage limits, expiry)
    - `CouponUsage` — redemption tracking per customer
- **Key Capabilities:**
    - Discount types: percentage, fixed amount, buy-X-get-Y, bundle pricing
    - Scope: entire order, specific products, categories, customer groups
    - Scheduling: start/end datetime, active days (weekends only, etc.)
    - Stacking rules (combinable vs exclusive)
    - Auto-application at POS and e-commerce checkout
    - Flash sale with stock allocation and countdown
    - Free shipping promotions
- **Indonesia-Specific:**
    - Paket hemat (bundle) — standard Indonesian retail promotion
    - Harbolnas campaigns (10.10, 11.11, 12.12)
    - Ramadan/Lebaran seasonal promotions
    - Hadiah langsung (buy X get free gift)
- **Dependencies:** SharedKernel, Catalog, CRM
- **Estimated Entities:** 4

---

**Module 13 · Loyalty**

- **Status:** 🆕 New
- **Purpose:** Points-based loyalty program with membership tiers and referrals
- **Key Entities:**
    - `LoyaltyProgram` — program configuration (points per Rupiah, redemption rules)
    - `PointTransaction` — earn/redeem/expire/adjust log (immutable ledger)
    - `LoyaltyTier` — tier definitions (Bronze, Silver, Gold, Platinum) with thresholds and benefits
    - `MemberTier` — customer's current tier assignment with qualifying period
- **Key Capabilities:**
    - Earn points per Rupiah spent (configurable rate)
    - Redeem points for discounts at POS and e-commerce
    - Tier progression based on spending within qualifying period
    - Tier benefits (exclusive discounts, early access, free shipping)
    - Points expiry management
    - Referral program (refer friend, both earn bonus)
- **Dependencies:** SharedKernel, CRM, Sales
- **Estimated Entities:** 4

---

### Tier 5 — Operations

> Delivery, multi-location management, and human resources.

---

**Module 14 · Logistics**

- **Status:** ♻️ Refactor from existing
- **Purpose:** Shipment fulfillment, Indonesian courier integration, fleet management for self-delivery
- **Existing Entities (Moved & Extended):**
    - `Shipment` — add SalesOrderId for order-to-shipment linking, add courier fields (carrier, AWB/resi, service type)
    - `ShipmentAssignment` — existing driver + vehicle pairing, unchanged logic
    - `ShipmentTracking` — extend with courier tracking data alongside internal tracking
    - `Driver` — existing, unchanged
    - `Vehicle` — existing, unchanged
- **New Capabilities:**
    - Shipping method selection: self-delivery (existing fleet) OR third-party courier
    - Indonesian courier API integration (JNE, J&T, SiCepat, AnterAja)
    - RajaOngkir API for multi-courier rate comparison
    - Shipping rate calculation (origin kota → destination kota, weight, volumetric)
    - AWB/resi number generation and retrieval from courier API
    - Shipping label printing (A6 thermal format)
    - Courier pickup request scheduling
    - COD shipment tracking and reconciliation
    - Delivery cost rules (free shipping threshold, flat rate, weight-based)
- **Indonesia-Specific:**
    - RajaOngkir API (standard Indonesian logistics rate API)
    - Shipping zones: Jawa, Sumatera, Kalimantan, Sulawesi, Bali/NTT/NTB, Papua/Maluku
    - Volumetric weight: P × L × T / 6000
    - Shipping insurance (asuransi pengiriman)
- **Dependencies:** SharedKernel, Sales, Inventory
- **Estimated Entities:** 5 (all existing, moved + extended)

---

**Module 15 · Multi-Branch**

- **Status:** 🆕 New
- **Purpose:** Branch hierarchy, branch-level data scoping, inter-branch operations, POS register management
- **Key Entities:**
    - `Branch` — shared with Inventory module (physical location, hierarchy, manager, hours)
    - `BranchSetting` — per-branch configuration (receipt header/footer, payment methods, operating hours)
    - `BranchRegister` — POS terminal registration per branch (register ID, status, assigned cashier)
- **Key Capabilities:**
    - Branch hierarchy: Head Office → Regional → Branch
    - Branch-warehouse association (each branch has 1+ warehouses/stock rooms)
    - User assignment to branch(es) with multi-branch access option
    - Branch-level data filtering (users see own branch by default, managers see multiple)
    - Branch-level inventory view
    - Branch-level sales reporting
    - Inter-branch stock transfer workflow
    - Consolidated multi-branch dashboard
    - Branch-specific receipt customization
- **Indonesia-Specific:**
    - Indonesian address format per branch
    - Time zone support (WIB, WITA, WIT) for multi-region businesses
    - Province/city mapping for regional reporting
- **Dependencies:** SharedKernel, Inventory
- **Estimated Entities:** 3

---

**Module 16 · HRM (Human Resource Management)**

- **Status:** 🆕 New
- **Purpose:** Basic employee management, attendance tracking, leave management for retail staff
- **Key Entities:**
    - `Employee` — master data (name, NIK/KTP, NPWP, position, department, branch, join date, status)
    - `Department` — department master
    - `Position` — job title/position master
    - `Attendance` — clock-in/clock-out records
    - `LeaveRequest` — leave request with approval workflow
    - `LeaveBalance` — leave type balances per employee per year
    - `EmployeeDocument` — uploaded documents (KTP, NPWP, BPJS card, contracts)
- **Key Capabilities:**
    - Employee lifecycle: Active, Probation, Resigned, Terminated
    - Branch/department assignment
    - Basic attendance (manual entry or simple time clock)
    - Leave types: Cuti Tahunan, Sakit, Izin, Cuti Besar
    - Leave request → approval workflow
    - Employee document storage
    - Link employee to system user account
- **Phase 2 Capabilities:**
    - Payroll calculation (salary, tunjangan, BPJS, PPh 21)
    - Shift scheduling for retail staff
    - THR (Tunjangan Hari Raya) calculation
    - Overtime per UU Cipta Kerja
- **Indonesia-Specific:**
    - NIK (Nomor Induk Kependudukan) / KTP number as identifier
    - NPWP for PPh 21 withholding
    - BPJS Kesehatan and Ketenagakerjaan membership tracking
    - Cuti Tahunan: minimum 12 days/year per UU Ketenagakerjaan
    - UMR/UMK reference for salary compliance
- **Dependencies:** SharedKernel, Multi-Branch
- **Estimated Entities:** 7

---

### Tier 6 — Cross-Cutting

> Built incrementally alongside other modules. Consumed by every module.

---

**Module 17 · Notification**

- **Status:** 🆕 New
- **Purpose:** Multi-channel notification dispatch — WhatsApp, email, SMS, in-app
- **Key Entities:**
    - `NotificationTemplate` — customizable templates per tenant, per event type, per channel, with merge fields
    - `NotificationLog` — delivery log (recipient, channel, status, sent timestamp)
    - `NotificationPreference` — per-user channel preferences
- **Key Capabilities:**
    - In-app notification center (bell icon, unread count, mark as read)
    - Email: order confirmation, shipping, invoice delivery, password reset
    - WhatsApp: order status, shipping updates, payment reminders (via Fonnte API)
    - Event-driven: modules emit events → notification service dispatches
    - Template engine with merge fields (customer name, order number, tracking link)
    - Configurable preferences per user per channel
- **Indonesia-Specific:**
    - WhatsApp as primary channel (90%+ Indonesian internet users use WhatsApp)
    - Bahasa Indonesia templates
    - WhatsApp Business API integration
- **Dependencies:** SharedKernel
- **Estimated Entities:** 3

---

**Module 18 · Audit**

- **Status:** 🆕 New
- **Purpose:** Immutable action logging for security, accountability, and regulatory compliance
- **Key Entities:**
    - `AuditEntry` — append-only log (who, what, when, where/IP, entity type, entity ID, action, old/new values JSON)
- **Key Capabilities:**
    - Automatic capture via EF Core SaveChanges interceptor
    - All CRUD on critical entities logged
    - Authentication events (login, logout, failed login, password change)
    - Field-level before/after change tracking
    - POS-specific audit (voids, refunds, discount overrides, register open/close)
    - Search/filter by user, entity type, date range, action
    - Immutable — no update or delete on audit entries
    - Export for external/DJP auditors
- **Indonesia-Specific:**
    - 10-year minimum retention per UU KUP (tax law)
    - DJP audit support (transaction record export)
    - UU PDP (data protection law) compliance logging
- **Dependencies:** SharedKernel
- **Estimated Entities:** 1

---

**Module 19 · Reporting**

- **Status:** 🆕 New
- **Purpose:** Dashboards, KPIs, business analytics, and report generation
- **Key Entities:**
    - `ReportDefinition` — saved report configurations
    - `ScheduledReport` — auto-email report schedule (daily/weekly/monthly, recipient list)
- **Key Capabilities:**
    - Executive dashboard: today's sales, MTD, YTD, orders, customers, stock value
    - Sales reports: by period, product, category, channel, branch, salesperson
    - Inventory reports: on hand, movements, slow/fast-moving, near-expiry, valuation
    - Customer reports: new vs returning, top customers, acquisition
    - Financial reports: simplified P&L, cash flow, AR/AP aging, tax summary
    - Product performance: top sellers, margin analysis, sales velocity
    - Trend charts: daily/weekly/monthly with YoY comparison
    - Report scheduling (auto-email PDF/Excel)
    - Export to Excel (XLSX) and PDF
- **Indonesia-Specific:**
    - IDR formatting throughout
    - Ramadan/Lebaran period analysis
    - Regional performance by province/island
    - PPh Final 0.5% UMKM revenue tracking
- **Dependencies:** SharedKernel (reads data from all modules)
- **Estimated Entities:** 2

---

**Module 20 · File Storage**

- **Status:** 🆕 New
- **Purpose:** Centralized file management for product images, documents, invoices, receipts
- **Key Entities:**
    - `FileMetadata` — file record (path, content type, size, uploader, timestamp)
- **Key Capabilities:**
    - Upload/download/delete via `IFileStorageService` interface
    - Path convention: `tenants/{tenantId}/{module}/{entity}/{id}/{filename}`
    - Product images with resize/thumbnail generation
    - Invoice/receipt PDF storage
    - Employee document storage
    - Expense receipt upload
    - Implementation: MinIO (self-hosted S3-compatible), migrate to cloud S3 later
- **Dependencies:** SharedKernel
- **Estimated Entities:** 1

---

**Module 21 · Settings**

- **Status:** 🆕 New
- **Purpose:** Tenant-level configuration for all modules
- **Key Entities:**
    - `TenantSetting` — key-value configuration per module (JSON values)
- **Setting Categories:**
    - **Company:** name, legal name, address, NPWP, NIB, logo
    - **Regional:** timezone (WIB/WITA/WIT), date format, IDR formatting
    - **Tax:** PKP status, PPN rate, NSFP range, e-Faktur config
    - **Invoice:** numbering format, payment terms, template, footer text
    - **POS:** receipt header/footer, default payment method, cash rounding, barcode format
    - **Inventory:** valuation method (FIFO/average), low stock thresholds, auto-reorder toggle
    - **Email:** SMTP config or email service API key, sender name/email
    - **Notification:** enable/disable channels per event type
    - **Payment:** Midtrans/Xendit API keys, enabled payment methods, sandbox mode
    - **Shipping:** default origin address, courier API keys, free shipping threshold
- **Dependencies:** SharedKernel
- **Estimated Entities:** 1

---

**Module 22 · API Platform**

- **Status:** ♻️ Refactor from existing
- **Purpose:** Versioned REST API, webhooks, rate limiting, API key management for external integrations
- **Key Entities:**
    - `ApiKey` — tenant-scoped API keys for external system integration
    - `WebhookSubscription` — outbound webhook registrations per tenant (URL, events, secret)
    - `WebhookLog` — webhook delivery log (payload, response, retry count)
- **Key Capabilities:**
    - RESTful API for all modules (extend existing Swagger/OpenAPI)
    - JWT Bearer + API key authentication (dual auth for integrations)
    - API versioning (URL-based: `/api/v1/`, `/api/v2/`)
    - Rate limiting per tenant/API key
    - Configurable outbound webhooks (order.created, payment.received, stock.low)
    - Idempotency keys for critical operations (prevent double-charge/double-order)
    - Payment gateway abstraction (pluggable adapters)
    - Courier abstraction (pluggable JNE, J&T, SiCepat adapters)
- **Phase 2:**
    - Marketplace adapters (Tokopedia, Shopee, Lazada)
    - Accounting integration (Jurnal.id, Accurate Online)
    - GraphQL for complex reporting queries
- **Dependencies:** SharedKernel, Platform
- **Estimated Entities:** 3

---

## Numbers at a Glance

| Metric | Count |
| --- | --- |
| Total modules | 22 |
| New modules to build | 19 |
| Existing modules to refactor | 3 (IAM, Logistics, API Platform) |
| Total entities (estimated) | ~69 |
| Total database tables | ~81 |
| Features at P0 (MVP Critical) | ~95 |
| Features at P1 (Important for Launch) | ~85 |
| Features at P2 (Phase 2) | ~75 |
| Features at P3 (Future) | ~35 |
| Features total | ~290 |

---

## Build Phase Mapping

### Phase 1 · Foundation — Sprint 1–2

> Multi-tenant infrastructure. Everything depends on this.

| Module | Work |
| --- | --- |
| SharedKernel | Create project: ITenantScoped, TenantAuditableEntity, ITenantContext, Money, Result\<T\>, IDomainEvent |
| Platform | Create Tenant entity, provisioning service, tenant resolution middleware |
| IAM (refactor) | Add TenantId to all 6 existing entities, update JWT claims, update login/register flow |
| Infrastructure | EF Core global query filters, SaveChanges interceptor, Redis setup |
| Database | Migration: add TenantId to all 12 existing tables, create platform\_tenants table |

**Deliverable:** Existing NiagaOne features work per-tenant. Multi-tenant auth. Super-admin can create tenants.

---

### Phase 2 · Catalog + Inventory — Sprint 3–4

> Product data model and stock tracking backbone.

| Module | Work |
| --- | --- |
| Catalog | Product, ProductVariant, Category, Brand, PriceList, ProductImage, Supplier CRUD. Barcode/SKU. CSV import. |
| Inventory | Refactor Warehouse into Inventory module. StockItem, StockMovement, Branch. Low stock alerts. Batch/lot + expiry. |
| File Storage | MinIO integration for product images |

**Deliverable:** Can manage products with variants, track stock across warehouses, receive alerts on low stock.

---

### Phase 3 · Sales Core — Sprint 5–6

> Physical store can start selling. Customers tracked.

| Module | Work |
| --- | --- |
| CRM | Customer, CustomerAddress, segments. Unified profile (phone/email dedup). |
| Sales (POS) | PosTransaction, POS UI (Blazor), barcode scan, cart, split payment, cash handling, receipt generation |
| Multi-Branch | Branch entity, branch-register association, branch-level data filtering |

**Deliverable:** Functional POS for physical store. Cash + manual payment recording. Customer linkage. Multi-branch ready.

---

### Phase 4 · Finance + Payments — Sprint 7–8

> Money tracking and Indonesian tax compliance.

| Module | Work |
| --- | --- |
| Finance | Invoice, Payment, Expense CRUD. Auto-invoice from order/POS. Invoice PDF. AR aging. |
| Tax | PPN engine (11%). Tax-inclusive/exclusive modes. Output/input PPN tracking. e-Faktur CSV export. |
| Payment Gateway | Midtrans integration (QRIS, VA, GoPay). Xendit integration (OVO, DANA). Webhook handling. |

**Deliverable:** Invoices generated automatically. PPN calculated. Online payments accepted via QRIS/VA/e-wallet.

---

### Phase 5 · E-commerce — Sprint 9–10

> Online shop goes live.

| Module | Work |
| --- | --- |
| Sales (E-commerce) | Cart, CartItem, online SalesOrder, checkout flow, guest checkout |
| Storefront | Blazor WASM app: product listing, detail, cart, checkout, customer accounts, order tracking |
| Storefront API | /store/v1/\* endpoints: public product browsing, authenticated cart/checkout |

**Deliverable:** Full online shop with browsing, cart, checkout, payment, and order tracking. Tenant-branded.

---

### Phase 6 · Logistics + Promotions — Sprint 11–12

> Delivery integration and growth tools.

| Module | Work |
| --- | --- |
| Logistics (refactor) | Move existing entities to Logistics module. Add SalesOrderId linkage. Indonesian courier API (JNE, J&T, SiCepat). RajaOngkir. Shipping label printing. |
| Promotions | Promotion rules engine, coupons, flash sales, auto-application at POS/checkout |
| Loyalty | Points program, earn/redeem, membership tiers |

**Deliverable:** Orders ship via Indonesian couriers with tracking. Discounts and loyalty points drive growth.

---

### Phase 7 · Supply Chain + Notifications — Sprint 13–14

> Complete procurement cycle and customer communication.

| Module | Work |
| --- | --- |
| Inventory (Purchase) | PurchaseOrder, GoodsReceipt, partial receiving, 3-way invoice matching, auto-reorder |
| Notification | WhatsApp via Fonnte, email via SMTP, in-app notification center, templates |
| HRM | Employee master data, attendance, leave management, document storage |

**Deliverable:** Full purchase-to-stock cycle. Customers notified via WhatsApp. Basic HR for retail staff.

---

### Phase 8 · Reporting + Polish — Sprint 15–16

> Business intelligence, compliance, and system configuration.

| Module | Work |
| --- | --- |
| Reporting | Executive dashboard, sales/inventory/financial/customer reports, Excel/PDF export, scheduled reports |
| Audit | EF Core interceptor, immutable audit trail, search/filter UI |
| Settings | Tenant-level configuration UI for all modules |
| API Platform | API key management, outbound webhooks, rate limiting dashboard |

**Deliverable:** Full business visibility. Compliance-ready audit trail. Self-service configuration. API for integrations.

---

## Sprint Timeline Summary

| Sprint | Months | Modules | Milestone |
| --- | --- | --- | --- |
| 1–2 | Month 1–2 | SharedKernel, Platform, IAM | Multi-tenant foundation |
| 3–4 | Month 2–3 | Catalog, Inventory, File Storage | Product + stock management |
| 5–6 | Month 3–4 | CRM, Sales (POS), Multi-Branch | Physical store selling |
| 7–8 | Month 4–5 | Finance, Tax, Payment Gateway | Financial compliance |
| 9–10 | Month 5–6 | Sales (E-commerce), Storefront | Online shop live |
| 11–12 | Month 6–7 | Logistics, Promotions, Loyalty | Delivery + growth |
| 13–14 | Month 7–8 | Purchase, Notification, HRM | Supply chain + comms |
| 15–16 | Month 8–9 | Reporting, Audit, Settings, API | BI + compliance + polish |

---

*Last updated: 2026-03-25*
*Reference documents: ERP\_SYSTEM\_DESIGN.md · ERP\_FEATURE\_BREAKDOWN.md · PRODUCT\_STRATEGY\_CANVAS.md*
