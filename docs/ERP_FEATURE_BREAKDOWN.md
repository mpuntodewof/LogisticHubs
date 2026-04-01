# NiagaOne ERP - Comprehensive Feature Breakdown

**Version:** 1.0
**Date:** 2026-03-25
**Target Market:** Indonesian Mid-Market Retail (Store + E-commerce)
**Priority Legend:** [P0] MVP Critical | [P1] Important for Launch | [P2] Phase 2 | [P3] Future

---

## Feature Summary

| Priority | Count | Description |
|----------|-------|-------------|
| **[P0]** | ~95 | MVP-critical — must be in initial release |
| **[P1]** | ~85 | Important for a competitive launch |
| **[P2]** | ~75 | Phase 2 features for growth |
| **[P3]** | ~35 | Future vision features |
| **Total** | **~290** | Across 23 modules |

---

## Module 1: Multi-Tenancy & Tenant Management

**Purpose:** Isolates data per tenant (business) in shared SaaS infrastructure.

### Core [P0]
- Tenant registration and onboarding wizard (company name, NPWP, business type)
- TenantId data isolation via EF Core global query filters
- Tenant context resolution from JWT claims
- Tenant-scoped user management
- Tenant-scoped RBAC (roles/permissions per tenant)

### Important [P1]
- Subscription plan management (Free trial, Starter, Professional, Enterprise)
- Tenant status lifecycle: Trial → Active → Suspended → Cancelled
- Tenant-level feature flags (enable/disable modules per tier)
- Tenant billing information (legal name, NPWP, address)

### Phase 2+ [P2-P3]
- Custom subdomain per tenant
- Tenant data export for compliance/portability
- Usage metering and quota enforcement
- White-label branding

### Indonesia-Specific
- [P0] NPWP (tax ID) as required tenant field
- [P0] NIB (business license number)
- [P1] Business entity types: PT, CV, UD, Perorangan
- [P1] Default WIB timezone, id-ID locale, IDR with Rp formatting

---

## Module 2: Point of Sale (POS)

**Purpose:** Touchscreen-optimized in-store sales terminal replacing standalone cash registers.

### Core [P0]
- POS session management (open/close register, opening cash balance, reconciliation)
- Product search by barcode scan, SKU, or name with autocomplete
- Shopping cart with line items (product, variant, qty, price, discount, subtotal)
- Price calculation engine (base price, discounts, PPN tax)
- Multiple payment methods per transaction (split payment)
- Cash payment with change calculation (rounded to nearest Rp100)
- Real-time inventory deduction on sale completion
- Receipt generation (thermal 58mm/80mm format)
- Transaction void/cancel (permission check + reason required)
- Daily sales summary per register/cashier

### Important [P1]
- Barcode scanner integration (USB HID / keyboard wedge)
- Hold and recall transactions (park a sale)
- Customer assignment to transaction (loyalty/history linkage)
- Quick-add favorite items (configurable shortcut buttons)
- Return/refund processing with original transaction lookup
- Cash drawer integration
- Offline mode with local queue and sync

### Phase 2+ [P2-P3]
- Multi-currency acceptance (for tourist areas)
- Weighted item support (digital scale integration)
- Combo/bundle selling
- Customer-facing display
- Self-checkout kiosk mode

### Indonesia-Specific
- [P0] QRIS payment integration (QR display + confirmation callback)
- [P0] Cash rounding to nearest Rp100
- [P0] PPN line item on receipt per DJP requirement
- [P1] GoPay, OVO, ShopeePay, Dana, LinkAja e-wallet integration
- [P1] EDC/debit card terminal integration (BCA, Mandiri, BRI, BNI)
- [P1] Struk Pajak format compliance

---

## Module 3: Product & Catalog Management

**Purpose:** Central product master data for POS, e-commerce, inventory, and reporting.

### Core [P0]
- Product CRUD (name, description, SKU auto-generate/manual)
- Product types: Simple, Variable, Service, Bundle/Kit
- Product variants (Size, Color) with independent SKU/price/stock
- Category hierarchy (multi-level)
- Product pricing (cost price, selling price, minimum floor price)
- Multiple product images with ordering
- Barcode/EAN management (EAN-13, UPC-A, internal)
- Unit of measure (pcs, kg, liter, meter, box, carton, dozen)
- Product status: Active, Inactive, Draft, Discontinued
- Bulk import via CSV/Excel

### Important [P1]
- Brand management
- Product tags/labels
- Multi-price lists (wholesale, retail, online, member)
- Product weight and dimensions (shipping calculation)
- Product duplication/cloning

### Phase 2+ [P2-P3]
- Custom product attributes engine
- Related products / cross-sell / up-sell
- Product reviews aggregation
- AI product description generation (Bahasa Indonesia)

### Indonesia-Specific
- [P0] Bilingual names (Bahasa Indonesia + English)
- [P0] IDR pricing "Rp 1.500.000" format
- [P1] Halal certification tracking (MUI certificate)
- [P1] BPOM registration number for food/cosmetics
- [P1] SNI certification tracking
- [P2] Tokopedia/Shopee/Lazada category mapping

---

## Module 4: Advanced Inventory Management

**Purpose:** Multi-warehouse stock tracking with batch/lot, serial numbers, expiry, and auto-reorder.

### Core [P0]
- Multi-warehouse stock tracking (extend existing Warehouse)
- Stock-on-hand per product/variant per warehouse (real-time)
- Stock movements ledger (immutable: sale, purchase, transfer, adjustment, return)
- Stock receiving (goods receipt from PO: scan/enter items, qty, batch, expiry)
- Stock adjustment (manual corrections with reason codes)
- Stock transfer between warehouses/locations
- Inventory valuation: FIFO (default), weighted average
- Low stock alerts (configurable min level per product per warehouse)
- Batch/Lot tracking (assign on receipt, track through to sale)
- Expiry date tracking with FEFO picking logic
- Barcode operations (scan to receive/pick/adjust/transfer)

### Important [P1]
- Serial number tracking for high-value items
- Stock reservation/allocation for online orders
- Auto-reorder point (trigger purchase requisition)
- Safety stock calculation
- Stocktake/inventory counting (count session, variance report)
- Bin/location management (aisle-rack-shelf-bin)

### Phase 2+ [P2-P3]
- Cycle counting
- Warehouse zone management
- Pick-pack-ship workflow for e-commerce
- Stock forecasting (sales velocity + seasonal trends)
- ABC analysis
- Multi-UOM inventory
- Demand planning with ML

### Indonesia-Specific
- [P0] FEFO mandatory for food products per BPOM
- [P1] Indonesian logistics hub cities as warehouse locations
- [P1] Batch recall support with traceability chain

---

## Module 5: Purchase & Procurement

**Purpose:** End-to-end purchasing from supplier management through PO, goods receipt, and invoice matching.

### Core [P0]
- Supplier master data (name, contact, NPWP, bank account, payment terms)
- Purchase Order creation (supplier, line items, delivery date, warehouse)
- PO approval workflow (auto-approve under threshold)
- PO lifecycle: Draft → Submitted → Approved → Received → Closed
- Goods Receipt Note (receive against PO, record batch/expiry/serial)
- Partial receiving
- Purchase invoice matching (3-way: PO + GRN + invoice)
- Cost price update on receipt

### Important [P1]
- Purchase requisition (internal request before PO)
- Supplier price lists and quotation comparison
- Purchase return (return to supplier + credit note)
- Landed cost calculation (freight, customs, insurance)
- PO PDF email to supplier
- Recurring purchase orders

### Indonesia-Specific
- [P0] Supplier NPWP validation (mandatory for PPN tax credit)
- [P0] PPN on purchase invoices (Faktur Pajak Masukan)
- [P1] PPh 23 withholding tax on service purchases (2%)
- [P1] Terbilang: amount in Indonesian words

---

## Module 6: Sales & Order Management

**Purpose:** Unified order management for POS, e-commerce, and manual orders.

### Core [P0]
- Sales Order creation (customer, line items, shipping address)
- Order source tracking (POS, E-commerce, Manual, Marketplace)
- Order lifecycle: Pending → Confirmed → Processing → Shipped → Delivered → Completed
- Order cancellation with inventory release and refund
- Automatic inventory reservation on confirmation
- Order fulfillment workflow (pick list, packing, shipping)
- Auto-sequential numbering (SO-2026-00001)
- Sales invoice auto-generation

### Important [P1]
- Partial fulfillment and backorder management
- Return/refund workflow (RMA → approve → receive → refund/replace)
- Quotation / proforma invoice
- Bulk order processing (batch pick/pack)
- Customer self-service order tracking (WhatsApp/email link)

### Indonesia-Specific
- [P0] Indonesian address format (Jalan, RT/RW, Kelurahan, Kecamatan, Kota, Provinsi, Kode Pos)
- [P0] Phone format validation (+62 / 08xx)
- [P1] COD order type with special handling
- [P1] Marketplace order import (Tokopedia, Shopee, Lazada)

---

## Module 7: E-commerce Storefront

**Purpose:** Built-in, tenant-branded online shop with browsing, cart, checkout, and order tracking.

### Core [P0]
- Tenant-branded storefront (configurable logo, colors, banners)
- Product listing with grid/list views, category/price filtering
- Product detail page (image gallery, variants, stock display, add-to-cart)
- Category navigation (hierarchical menu, breadcrumbs)
- Shopping cart (persistent across sessions)
- Guest checkout and registered customer checkout
- Checkout flow: cart → address → shipping → payment → confirmation
- Customer registration/login (separate from back-office)
- Customer dashboard (order history, tracking, address book, profile)
- Product search with keyword matching
- Responsive design (mobile-first — 70%+ Indonesian mobile traffic)

### Important [P1]
- Search autocomplete/suggestions
- Wishlist / save for later
- Product reviews and ratings
- SEO (meta titles, clean URLs, sitemap.xml, JSON-LD structured data)
- Social sharing (WhatsApp share is critical)
- Static pages (About, Contact, FAQ, Kebijakan Privasi, Syarat & Ketentuan)

### Phase 2+ [P2-P3]
- Blog/content management
- Product recommendation engine
- Flash sale page with countdown
- PWA for mobile app-like experience
- Live chat (WhatsApp Business API)

### Indonesia-Specific
- [P0] Bahasa Indonesia default language
- [P0] IDR format "Rp 1.500.000"
- [P0] Courier display (JNE, J&T, SiCepat, AnterAja, GoSend, GrabExpress)
- [P0] WhatsApp "Chat dengan Penjual" button
- [P1] Ongkos kirim calculator (origin kota → destination kota)
- [P1] Holiday theming (Ramadan, Lebaran, Harbolnas 12.12)
- [P1] COD (Bayar di Tempat) checkout option

---

## Module 8: Customer Relationship Management (CRM)

**Purpose:** Unified customer database across offline and online channels.

### Core [P0]
- Customer master data (name, email, phone, addresses, type)
- Unified profile across POS and e-commerce (phone/email matching)
- Purchase history aggregation across channels
- Customer search (name, phone, email, code)
- Customer groups/segments (VIP, Regular, Wholesale, New, Inactive)

### Important [P1]
- Customer notes and interaction log
- Multiple shipping/billing addresses
- Credit limit for B2B/wholesale
- Customer balance tracking (store credit)
- CSV/Excel import

### Indonesia-Specific
- [P0] Indonesian phone as primary identifier (+62 / 08xx, 10-13 digits)
- [P0] WhatsApp number field
- [P1] KTP number for B2B/wholesale
- [P1] Indonesian address structure

---

## Module 9: Omnichannel Management

**Purpose:** Unify inventory, pricing, and customer experience across physical and online channels.

### Core [P0]
- Unified inventory pool with channel-level allocation
- Consistent pricing across channels (with optional overrides)
- Single customer identity across channels
- Order source visibility throughout system

### Important [P1]
- Buy Online, Pick Up In Store (BOPIS / Click & Collect)
- Channel-specific stock visibility
- Unified return policy (return in-store regardless of channel)

### Phase 2+ [P2-P3]
- Ship-from-store
- Marketplace integration hub (Tokopedia, Shopee, Lazada, TikTok Shop)
- Inventory sync with marketplaces
- Unified loyalty across channels

---

## Module 10: Invoicing & Billing

**Purpose:** Invoice generation, management, and tracking for sales transactions.

### Core [P0]
- Auto-generate invoice from completed order/POS transaction
- Manual invoice creation for ad-hoc billing
- Invoice fields: number, date, due date, customer, lines, subtotal, discount, PPN, total
- Invoice status: Draft → Sent → Partially Paid → Paid → Overdue → Cancelled
- Invoice PDF generation with tenant branding
- Configurable numbering format (INV/2026/03/00001)
- Payment terms: Due on receipt, Net 7/14/30/60, Custom

### Important [P1]
- Recurring invoices
- Credit note / debit note
- Invoice email delivery
- AR aging report (current, 1-30, 31-60, 61-90, 90+ days)
- Payment reminder scheduling (automated email/WhatsApp)

### Indonesia-Specific
- [P0] Faktur Pajak format compliance
- [P0] PPN separated on invoice (DPP + PPN line)
- [P0] NPWP of both seller and buyer
- [P0] Terbilang: total in Indonesian words
- [P1] e-Faktur integration (NSFP allocation)
- [P1] Tax invoice numbering per DJP format
- [P1] Materai digital for invoices above Rp 5.000.000

---

## Module 11: Payment Processing

**Purpose:** Receive and record payments with Indonesian payment gateway integration.

### Core [P0]
- Payment recording (amount, date, method, reference, linked invoices)
- All methods: Cash, Bank Transfer, QRIS, VA, E-Wallet, Card, COD
- Midtrans / Xendit gateway integration for online payments
- QRIS generation and confirmation
- Virtual Account per bank (BCA, Mandiri, BRI, BNI, Permata, CIMB)
- E-wallet: GoPay, OVO, ShopeePay, Dana, LinkAja
- Payment status: Pending → Processing → Completed → Failed → Refunded
- Webhook handling for payment callbacks
- Partial payment support
- Over/underpayment handling

### Important [P1]
- Bank statement reconciliation
- Refund processing via gateway
- Manual payment verification (bukti transfer upload)
- Payment receipt generation

### Indonesia-Specific
- [P0] QRIS mandatory (Bank Indonesia regulation)
- [P0] VA for major banks
- [P1] Convenience store payment (Alfamart, Indomaret)
- [P2] Paylater (Kredivo, Akulaku)

---

## Module 12: Tax Management (PPN, e-Faktur)

**Purpose:** Indonesian tax calculation, tracking, and DJP compliance.

### Core [P0]
- PPN calculation engine (11%, configurable for 12%)
- Tax-inclusive and tax-exclusive pricing modes
- Product tax categories (Standard PPN, Exempt, Not Collected, Non-PPN)
- Output PPN tracking (collected on sales)
- Input PPN tracking (paid on purchases)
- PPN summary report (monthly output vs input, net payable)
- Tax period management (Masa Pajak)

### Important [P1]
- Faktur Pajak Keluaran generation
- Faktur Pajak Masukan recording
- NSFP management (request range, allocate, track)
- e-Faktur CSV export (DJP compatible)
- PPh 23 withholding on service purchases

### Indonesia-Specific
- [P0] PPN rate 11% per UU HPP
- [P0] DPP calculation rules
- [P0] PKP compliance (mandatory > Rp 4.8B annual revenue)
- [P1] PPh Final 0.5% for UMKM (< Rp 4.8B per PP 55/2022)
- [P2] Coretax DJP integration
- [P2] NPWP 16-digit (NIK-based) migration

---

## Module 13: Expense Management

**Purpose:** Track business expenses, reimbursements, and petty cash.

### Core [P0]
- Expense entry (date, category, amount, description, vendor, payment method)
- Customizable expense categories
- Expense status: Draft → Submitted → Approved → Paid → Rejected
- Receipt/document upload
- Approval workflow (configurable per amount)

### Important [P1]
- Petty cash (kas kecil) management
- Employee expense reimbursement
- Recurring expenses (monthly rent, utilities)
- Expense by branch/department/cost center

---

## Module 14: Financial Reporting & Analytics

**Purpose:** Essential financial visibility without requiring accounting expertise.

### Core [P0]
- Sales summary (daily/weekly/monthly/yearly, by channel/branch/category)
- Revenue report (gross, discounts, returns, net, PPN collected)
- Simplified P&L (revenue - COGS - expenses)
- Cash flow summary
- AR summary (outstanding customer invoices)
- AP summary (outstanding supplier invoices)
- Dashboard with key metrics

### Important [P1]
- COGS report based on inventory valuation
- Gross margin by product/category/brand
- Tax summary report
- Period-over-period comparison
- Export to Excel/PDF

### Indonesia-Specific
- [P0] IDR formatting throughout
- [P1] PPh Final 0.5% UMKM calculation
- [P2] Export to Accurate Online / Jurnal.id format

---

## Module 15: Human Resource Management (Basic)

### Important [P1]
- Employee master data (name, NIK/KTP, NPWP, position, department, branch)
- Employee status: Active, Probation, Resigned, Terminated
- Department and position management
- Branch assignment
- Basic attendance (clock in/out)
- Leave management (Cuti Tahunan, Sakit, Izin)
- Employee document storage

### Phase 2+ [P2]
- Payroll calculation (salary, tunjangan, BPJS, PPh 21)
- Payslip generation
- Shift scheduling for retail
- THR calculation
- Overtime per UU Cipta Kerja

---

## Module 16: Multi-Branch Management

### Core [P0]
- Branch CRUD (name, code, address, phone, manager, hours, status)
- Branch hierarchy (HQ → regional → branch)
- Branch-warehouse association
- Branch-level POS registers
- User assignment to branch(es)
- Branch-level data filtering
- Branch-level inventory

### Important [P1]
- Branch-level sales reporting
- Inter-branch stock transfer
- Consolidated multi-branch dashboard
- Branch-specific settings (receipt header, hours, payment methods)

---

## Module 17: Logistics & Delivery (Enhancement of Existing)

### Core [P0]
- Order-to-shipment linking (extend existing Shipment)
- Shipping method selection: self-delivery (fleet) or third-party courier
- Indonesian courier API (JNE, J&T, SiCepat, AnterAja)
- Shipping rate calculation (ongkos kirim: origin → destination, weight)
- AWB/resi number generation/retrieval
- Tracking unification (courier + internal, extend ShipmentTracking)

### Important [P1]
- Shipping label printing (A6 thermal)
- Bulk label generation
- Courier pickup request
- Delivery cost management (free shipping threshold, flat/weight-based)
- COD shipment tracking and reconciliation

### Indonesia-Specific
- [P0] RajaOngkir API for multi-courier rates
- [P0] Shipping zones (Jawa, Sumatera, Kalimantan, Sulawesi, Bali/NTT/NTB, Papua)
- [P0] Volumetric weight (P x L x T / 6000)
- [P1] Shipping insurance (asuransi pengiriman)

---

## Module 18: Promotions, Discounts & Loyalty

### Core [P0]
- Discount types: percentage, fixed amount, buy-X-get-Y, bundle pricing
- Discount scope: entire order, specific products/categories/customer groups
- Coupon/voucher codes (unique codes, usage limits, expiry)
- Promotion scheduling (start/end datetime, active days)
- Stacking rules (combinable vs exclusive)
- Auto-application at POS and e-commerce checkout

### Important [P1]
- Minimum purchase condition
- Flash sale management (time-limited + stock allocation)
- Free shipping promotions
- Member pricing tiers

### Phase 2+ [P2]
- Loyalty points (earn per Rupiah, redeem for discounts)
- Membership tiers (Bronze, Silver, Gold, Platinum)
- Referral program

### Indonesia-Specific
- [P0] Paket hemat (bundle) — common in Indonesian retail
- [P1] Harbolnas campaign management (10.10, 11.11, 12.12)
- [P1] Ramadan/Lebaran promotion scheduling
- [P1] Hadiah langsung (buy X get free gift)

---

## Module 19: Notification & Communication

### Core [P0]
- In-app notification center (bell icon, unread count, mark as read)
- Email notifications (order confirmation, shipping, invoice, password reset)
- Customizable notification templates with merge fields
- Notification preferences per user/channel

### Important [P1]
- WhatsApp notifications (order, shipping, payment reminders)
- Low stock alerts to purchasing manager
- New order alerts to warehouse team
- Approval request notifications

### Indonesia-Specific
- [P0] WhatsApp as primary channel (90%+ Indonesian internet users)
- [P0] Bahasa Indonesia templates
- [P1] WhatsApp Business API integration

---

## Module 20: Reporting & Business Intelligence

### Core [P0]
- Executive dashboard (today's sales, MTD, YTD, orders, customers, stock value)
- Sales reports (by period/product/category/channel/branch)
- Inventory reports (on hand, movements, slow/fast-moving, near-expiry)
- Customer reports (new vs returning, top customers, acquisition)

### Important [P1]
- Product performance (top sellers, margin analysis, velocity)
- Trend charts (daily/weekly/monthly with YoY comparison)
- Report scheduling (auto-email)
- Export to Excel/PDF

---

## Module 21: Audit Trail & Compliance

### Core [P0]
- Comprehensive audit log (who, what, when, where, which)
- All CRUD on critical entities logged
- Authentication events logged
- Immutable append-only log
- Audit viewer with search/filter

### Important [P1]
- Before/after value tracking (field-level changes)
- POS audit (voids, refunds, discount overrides)
- 10-year retention per UU KUP

---

## Module 22: Settings & Configuration

### Core [P0]
- Company profile (name, address, NPWP, NIB, logo)
- Regional (timezone WIB/WITA/WIT, date format, IDR)
- Tax (PKP status, PPN rate, NSFP range)
- Invoice (numbering, payment terms, template)
- POS (receipt header/footer, payment methods, cash rounding)
- Inventory (valuation method, low stock defaults, auto-reorder)

### Important [P1]
- Email/SMTP settings
- Notification preferences
- Payment gateway API keys
- Shipping settings (origin address, courier keys, free shipping threshold)

---

## Module 23: API & Integration Platform

### Core [P0]
- RESTful API for all modules
- JWT + API key authentication
- Rate limiting per tenant/key
- API versioning (URL-based /v1/)
- Configurable outbound webhooks (order.created, payment.received, stock.low)
- Swagger/OpenAPI documentation

### Important [P1]
- Payment gateway abstraction (pluggable Midtrans, Xendit, DOKU)
- Courier abstraction (pluggable JNE, J&T, SiCepat, AnterAja)
- Idempotency keys for critical operations

### Phase 2+ [P2]
- Marketplace adapters (Tokopedia, Shopee, Lazada, TikTok Shop)
- Accounting integration (Jurnal.id, Accurate Online)
- GraphQL for complex queries
- Bulk API endpoints

---

## Recommended MVP Build Order

| Order | Module | Why |
|-------|--------|-----|
| 1 | Multi-Tenancy | Foundation for everything |
| 2 | Product & Catalog | Core data model |
| 3 | Advanced Inventory | Stock backbone |
| 4 | Sales & Orders | Transaction engine |
| 5 | Invoicing + Tax | Financial compliance |
| 6 | Payment Processing | Revenue collection |
| 7 | POS | Physical store revenue |
| 8 | E-commerce Storefront | Online revenue |
| 9 | Multi-Branch | Multi-location support |
| 10 | CRM | Customer unification |
| 11 | Purchase & Procurement | Supply chain |
| 12 | Expense + Financial Reporting | Financial visibility |
| 13 | Logistics Enhancement | Courier integration |
| 14 | Promotions & Loyalty | Growth tools |
| 15 | Notifications + Audit + Settings + API | Cross-cutting (built incrementally) |

---

*This feature breakdown should be used alongside the System Architecture Document and Product Strategy Canvas for implementation planning.*
