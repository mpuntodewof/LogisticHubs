# StockLedger — Product Strategy Canvas

> Version 2.0 | Updated: 2026-04-13
> Pivot from full-ERP (NiagaOne) to focused Inventory + Finance SaaS

---

## 1. Vision

**StockLedger is the single source of truth for stock and money** — purpose-built for Indonesian mid-to-upper retailers who sell across multiple channels but lack unified inventory tracking and financial visibility.

**One-liner:** "Know your real stock. Know your real profit. Across every channel."

**What StockLedger is NOT:**
- Not an e-commerce storefront or marketplace
- Not a POS system (retailers keep their existing POS)
- Not a full ERP (no HRM, CRM, loyalty, notifications)
- Not a marketplace competitor (Tokopedia, Shopee stay as sales channels)

---

## 2. Customer Segments

### Primary: Multi-Channel Indonesian Retailers
| Attribute | Detail |
|-----------|--------|
| **Business size** | Mid-to-upper (Rp500M - Rp10B annual revenue) |
| **Employee count** | 10 - 200 employees |
| **SKU count** | 100 - 10,000 active SKUs |
| **Sales channels** | 2+ channels (Tokopedia + Shopee + offline toko, or similar) |
| **Tax status** | PKP (Pengusaha Kena Pajak) — legally required to issue e-Faktur |
| **Location** | Indonesia-first (Java-focused initially) |
| **Industry** | Fashion & apparel, electronics, FMCG, hard goods, general retail |

### Buyer Personas
| Persona | Role | Daily Pain | Buys Because |
|---------|------|------------|-------------|
| **Owner / Direktur** | Decision maker, pays the bill | "I don't know if I'm actually profitable per channel" | Financial visibility, tax compliance |
| **Warehouse Manager** | Daily operator | "I update stock in 3 places and still get it wrong" | Multi-channel stock sync |
| **Accountant / Finance** | Monthly reporting | "e-Faktur is manual, error-prone, and takes 2 days" | Automated PPN tracking, journal entries |

---

## 3. Problem Space

### Core Problems (ranked by daily impact)

| Rank | Problem | Frequency | Consequence | Impact Score |
|------|---------|-----------|-------------|-------------|
| **#1** | **No unified stock view** — Sells on Tokopedia, Shopee, and offline but updates stock manually in each platform. Numbers diverge within hours. | Daily | Overselling, order cancellations, marketplace penalties, lost trust | 35% |
| **#2** | **No real profitability data** — Revenue (omzet) is visible but actual margin per product, per channel is unknown. Platform fees (5-15%), shipping, returns make true profit opaque. | Monthly | Bad decisions about what to stock and where to sell | 25% |
| **#3** | **Tax compliance burden** — PKP businesses must issue e-Faktur and track PPN input/output. Done manually in DJP e-Faktur app. | Monthly | Errors lead to tax penalties. Consumes 2+ days/month. | 25% |
| **#4** | **Reactive purchasing** — No data on when to reorder. Overstock ties up cash (modal nyangkut), understock means lost sales. | Weekly | Cash flow problems, stockouts during peak demand | 15% |

### What They Use Today
| Alternative | Why It Fails |
|-------------|-------------|
| **Spreadsheets (Excel/Google Sheets)** | Manual, error-prone, no multi-user, no real-time sync |
| **Marketplace seller centers** | Each channel is a silo — no cross-channel view |
| **Jurnal.id / Accurate** | Accounting-only — no inventory depth, no purchasing, no stock movements |
| **Moka / Majoo POS** | POS-first tools — weak inventory, no multi-channel sync, no finance |
| **SAP / HashMicro** | Too expensive (Rp10M+/month), too complex, over-engineered |

---

## 4. Value Propositions

### Primary: "One place for all your stock"
Import your Tokopedia orders, Shopee sales, and offline transactions. See real-time stock across all channels and warehouses. Never oversell again.

### Secondary: "See where your money actually goes"
Automatic cost tracking, margin calculation per product and per channel. Know which products make money and which lose it after platform fees.

### Tertiary: "Tax compliance on autopilot"
PPN tracking built into every transaction. Generate e-Faktur-ready tax invoices. Journal entries created automatically. Month-end closing in hours, not days.

---

## 5. Core Features (v1 Scope)

### Foundation Layer (Built)
- Multi-tenant SaaS (single DB, TenantId isolation)
- JWT authentication + refresh token rotation
- RBAC with 70+ granular permissions
- Audit trail (immutable, field-level change tracking)
- Tenant settings & system configuration

### Inventory Module
| Feature | Status | Priority |
|---------|--------|----------|
| Product catalog (categories, brands, variants, images) | Built | P0 |
| Units of measure + conversions | Built | P0 |
| Multi-warehouse stock tracking (on-hand, reserved, available) | Built | P0 |
| Immutable stock movement ledger | Built | P0 |
| CSV/Excel import from marketplaces | To build | P0 |
| Stock reconciliation workflow | To build | P1 |
| Low stock dashboard & alerts | To build | P1 |
| Inter-warehouse transfer | To build | P1 |

### Purchasing Module
| Feature | Status | Priority |
|---------|--------|----------|
| Supplier management | Built | P0 |
| Purchase order lifecycle (Draft > Submit > Approve > Receive) | Built | P0 |
| Goods receipt with stock auto-update | Built | P0 |
| Purchase cost tracking | To build | P1 |

### Finance Module
| Feature | Status | Priority |
|---------|--------|----------|
| Chart of accounts (double-entry) | Built | P0 |
| Journal entries (Draft > Posted > Voided) | Built | P0 |
| Invoice lifecycle (Draft > Issued > Paid) | Built | P0 |
| PPN 11% tax rates + e-Faktur numbering | Built | P0 |
| Profit & Loss report | To build | P0 |
| Margin per product / per channel report | To build | P0 |
| Balance sheet + cash flow | To build | P1 |
| PPN input/output summary for DJP | To build | P1 |

### CSV/Import Module (NEW - v1 Critical)
| Feature | Status | Priority |
|---------|--------|----------|
| CSV upload & column mapping UI | To build | P0 |
| Tokopedia order export parser | To build | P0 |
| Shopee order export parser | To build | P0 |
| Manual offline sales entry | To build | P0 |
| Import history & duplicate detection | To build | P1 |

---

## 6. Key Metrics

### North Star: Active tenants importing data weekly

| Metric | Month 3 | Month 6 | Year 1 | Year 2 |
|--------|---------|---------|--------|--------|
| Registered tenants | 20 | 80 | 200 | 800 |
| Active tenants (weekly import) | 5 | 30 | 80 | 350 |
| Paying tenants | 0 | 15 | 50 | 250 |
| MRR | Rp0 | Rp6M | Rp25M | Rp150M |

---

## 7. Competitive Positioning

| Competitor | Their Strength | Their Weakness | Our Edge |
|------------|---------------|----------------|----------|
| **Jurnal.id** | Accounting gold standard in ID | No inventory depth, no purchasing | Full purchasing + stock movement chain |
| **Jubelio** | Multi-channel sync (API) | Expensive (Rp800K+), basic finance | Cheaper with deeper finance (P&L, e-Faktur) |
| **Accurate** | Established brand | Legacy, desktop-first, complex | Cloud-native, simpler UX, modern API |
| **Moka / Majoo** | Strong POS | Weak inventory, no multi-channel | We don't compete on POS — we complement it |
| **Spreadsheets** | Free, familiar | Manual, error-prone, no automation | Our real competitor — must be 10x better |

---

## 8. Go-To-Market

| Phase | Timeline | Focus |
|-------|----------|-------|
| **Validate** | Month 1-3 | Landing page + waitlist. 10-20 beta users (free). Does CSV import solve a real problem? |
| **Launch** | Month 4-6 | Public launch with pricing. 14-day trial. SEO content: "track inventory across Tokopedia & Shopee" |
| **Grow** | Month 7-12 | Referral program. Accounting firm partnerships. Direct marketplace API (replace CSV). |

---

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Marketplace CSV format changes | Medium | User-configurable column mapping, parser versioning |
| Low trial-to-paid conversion | High | Focus onboarding — value visible in <15 minutes |
| Solo developer burnout | Critical | Ruthless scope control — only build what v1 needs |
| Competitor copies positioning | Medium | 6+ month speed advantage from existing codebase |

---

## 10. Decision Log

| Decision | Alternatives Considered | Why This Choice |
|----------|------------------------|-----------------|
| Inventory + Finance only | Full ERP (22 modules) | Solo dev can't build 22 modules. Focused wins vs spreadsheets. |
| Back-office, not storefront | E-commerce + storefront | Competing with Shopify/Tokopedia is a platform war. |
| CSV import for v1, API for v2 | Direct API from day 1 | CSV validates demand. APIs require partnerships. |
| Flat monthly pricing (3 tiers) | Per-SKU, per-transaction | Users hate per-tx fees — that's why they leave marketplaces. |
| SaaS-first, enterprise later | Both from day 1 | Enterprise = 10x support per customer. Scale SaaS first. |
| Indonesia-only Year 1 | International from start | PPN, e-Faktur, IDR localization = strong moat. |
