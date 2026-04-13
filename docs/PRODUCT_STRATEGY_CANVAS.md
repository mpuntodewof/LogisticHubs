# Product Strategy Canvas: NiagaOne ERP Platform
## SaaS Multi-Tenant ERP for Indonesian Mid-Market Retail & E-Commerce

**Document Version:** 1.0
**Date:** 2026-03-25
**Status:** Strategic Planning

---

## Executive Summary

NiagaOne is being transformed from a logistics management system into a full-featured, multi-tenant SaaS ERP platform purpose-built for Indonesian mid-market retail businesses operating both physical stores and e-commerce channels. The platform targets the gap between simple POS tools (Moka, Majoo) and enterprise solutions (SAP Business One, HashMicro) that are either too basic or too expensive for businesses with 20-1000 employees.

**What already exists:** A .NET 8 Clean Architecture foundation with shipment lifecycle management, warehouse operations, fleet management, RBAC with JWT authentication, and a Blazor frontend. This provides a strong technical spine — authentication, authorization, entity management patterns, and API infrastructure — that can be extended into ERP modules.

**What needs to be built:** Inventory management, POS, sales orders, purchasing, basic accounting/finance, e-commerce storefront, omnichannel inventory sync, Indonesian tax (PPN/PPh) compliance, and local payment integration.

---

## Section 1: Vision

### Product Vision Statement

> **Make enterprise-grade retail operations accessible to every Indonesian mid-market business** — unifying physical stores, online shops, warehouses, and logistics into one platform that speaks Bahasa Indonesia, calculates PPN automatically, and costs less than a single SAP consultant's monthly fee.

### 3-Year North Star

| Horizon | Goal |
|---|---|
| **Year 1 (2026)** | Launch omnichannel MVP (Store + Online Shop) with 50 paying tenants. Prove product-market fit in Jakarta-Bandung corridor. |
| **Year 2 (2027)** | Reach 500 tenants, IDR 500M ARR. Launch Clinic configuration. Build marketplace integrations (Tokopedia, Shopee). Enable multi-branch operations. |
| **Year 3 (2028)** | Reach 2,000 tenants, IDR 2.5B ARR. Expand to Surabaya, Medan, Makassar. Begin Southeast Asia exploration (Malaysia, Philippines). Hire first employees. |

### Position in Indonesian SME/Mid-Market Tech Landscape

Indonesia's SME tech ecosystem has a clear gap:

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  SIMPLE / CHEAP                              COMPLEX / EXPENSIVE        │
│                                                                         │
│  ┌──────────┐  ┌──────────┐  ┌───── GAP ─────┐  ┌───────────────────┐ │
│  │ Moka POS │  │  Majoo   │  │               │  │ SAP Business One  │ │
│  │ iReap    │  │  Pawoon  │  │  NiagaOne  │  │ HashMicro         │ │
│  │          │  │          │  │  ERP targets   │  │ Oracle NetSuite   │ │
│  │ < IDR 500K  │ IDR 500K- │  │  this space   │  │ IDR 10M+/month   │ │
│  │  /month  │  │  2M/month│  │               │  │                   │ │
│  └──────────┘  └──────────┘  └───────────────┘  └───────────────────┘ │
│                                                                         │
│  1-5 employees   5-20 employees  20-1000 employees  1000+ employees    │
│  Single store    1-3 locations   1-10+ locations    Enterprise          │
│  POS only        POS + basic     Full operations    Full ERP suite      │
│                  inventory       + e-commerce                           │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

The mid-market (20-1000 employees) is underserved. These businesses have outgrown Moka/Majoo but cannot justify SAP/HashMicro's cost or implementation complexity. They need:
- Multi-branch inventory that actually works
- Omnichannel (physical + online) as default, not add-on
- Indonesian tax compliance built-in (PPN 12%, PPh, e-Faktur)
- Local payment methods (QRIS, GoPay, OVO, ShopeePay, Virtual Account)
- Bahasa Indonesia as first-class language, not a translation

### Competitive Positioning

| Competitor | Strength | Weakness | NiagaOne ERP Advantage |
|---|---|---|---|
| **Jurnal by Mekari** | Strong accounting, large user base | Weak inventory & POS, no omnichannel, no logistics | Unified operations (not just books); built-in logistics from Day 1 |
| **iReap** | Good inventory for retail | Dated UI, limited e-commerce, weak multi-branch | Modern tech stack, real omnichannel, logistics backbone |
| **Majoo** | Easy POS, growing fast | SME-focused (too simple for mid-market), no logistics | Scales to 200+ employees; ERP-grade depth |
| **Moka POS (GoTo)** | Slick POS, GoTo ecosystem | POS-centric, no warehouse/logistics, GoTo lock-in | Full supply chain; vendor-neutral payment |
| **HashMicro** | Full ERP, Singapore-based | Expensive (IDR 10M+/mo), slow implementation, enterprise-focused | 10x cheaper, self-serve onboarding, mid-market fit |
| **SAP Business One** | Global standard, comprehensive | IDR 20M+/mo + consultants, overkill for mid-market | Purpose-built for Indonesian mid-market; no consultants needed |

**Core positioning statement:** "The ERP that Indonesian mid-market retailers actually need — not a POS pretending to be an ERP, and not an enterprise system pretending to be affordable."

---

## Section 2: Target Segments (Detailed)

### Primary Segment Definition

**Indonesian mid-market retail businesses operating physical stores AND e-commerce channels.**

| Attribute | Range |
|---|---|
| Employee count | 20-200 (primary), scaling to 200-1000 (secondary) |
| Annual revenue | IDR 2B - 50B (approx. USD 125K - 3M) |
| Locations | 1-10 physical stores/warehouses |
| Online presence | Own website/storefront + marketplace (Tokopedia, Shopee, Lazada) |
| Geography | Java-first (Jakarta, Bandung, Surabaya, Semarang, Yogyakarta) |

### Sub-Segments by Industry

| Sub-Segment | Size Estimate (Indonesia) | Complexity | PPN Profile | E-Commerce Adoption | Priority |
|---|---|---|---|---|---|
| **Fashion & Apparel** | ~45,000 mid-market businesses | Medium-High (sizes, colors, seasons) | Standard 12% | Very High (60%+ online) | **#1 — Launch** |
| **Health & Beauty** | ~25,000 | Medium (expiry dates, BPOM compliance) | Standard 12% + some exempt | High (50%+ online) | #2 |
| **Electronics & Gadgets** | ~15,000 | High (serial numbers, warranties) | Standard 12% | Very High (70%+ online) | #3 |
| **General Merchandise** | ~30,000 | Low-Medium | Standard 12% | Medium (30%+ online) | #4 |
| **F&B (Packaged/Retail)** | ~35,000 | High (perishability, batch tracking) | Mixed (some exempt) | Medium (25%+ online) | #5 |

### Why Fashion & Apparel First

1. **Highest omnichannel adoption** — Fashion retailers already sell both offline and online; they feel the pain of disconnected inventory daily
2. **SKU complexity is a differentiator** — Variants (size S/M/L/XL, color Red/Blue/Black) are poorly handled by simple POS systems; mid-market fashion retailers are actively looking for better tools
3. **High switching motivation** — Fashion businesses lose real money from overselling online when store stock sells out, or from dead stock they can't see across channels
4. **Aspirational brand identity** — Fashion businesses invest in technology that looks modern; they're willing to pay for a good product
5. **Concentrated geography** — Fashion mid-market is concentrated in Tanah Abang (Jakarta), Bandung (factory outlets, Cihampelas), and Surabaya — easy to reach

### Customer Personas

#### Persona 1: Rina — Business Owner (Pemilik Usaha)
- **Age:** 35-50
- **Background:** Built business from small shop, now has 3-8 stores + Tokopedia/Shopee presence
- **Frustrations:** Uses 4+ different tools (Excel for inventory, Moka for POS, separate Tokopedia seller center, WhatsApp for coordination). Can't get real-time view of total stock. Loses money on overselling.
- **Goal:** One system to see everything. Wants to open more branches without the operational chaos.
- **Decision criteria:** Price (must be under IDR 5M/month total), ease of use (staff can learn in 1 day), Bahasa Indonesia UI
- **Tech literacy:** Medium. Uses smartphone fluently, familiar with basic apps, but not technical.

#### Persona 2: Budi — Store Manager (Kepala Toko)
- **Age:** 25-35
- **Background:** Manages 1-2 store locations, reports to owner
- **Frustrations:** Manual stock counting, discrepancies between physical and system stock, no visibility into other branches
- **Goal:** Accurate stock counts, easy transfers between branches, simple daily reports for the owner
- **Tech literacy:** Medium. Comfortable with apps on a tablet.

#### Persona 3: Sari — Cashier (Kasir)
- **Age:** 20-30
- **Background:** Front-line staff processing 50-200 transactions/day
- **Frustrations:** Slow POS systems, complicated interfaces, payment method confusion
- **Goal:** Fast checkout. System tells her what to do, not the other way around.
- **Tech literacy:** Low-Medium. Needs an interface that works in under 3 taps per transaction.

#### Persona 4: Andi — Warehouse Staff (Staf Gudang)
- **Age:** 20-40
- **Background:** Handles receiving, put-away, picking, packing for 50-500 orders/day
- **Frustrations:** Paper-based picking lists, miscounts, no barcode scanning
- **Goal:** Scan-based workflow. Receive goods, scan, system updates. Pick order, scan, pack, done.
- **Tech literacy:** Low. Needs mobile-first, scan-heavy interface.

#### Persona 5: Dewi — Online Shop Admin (Admin Olshop)
- **Age:** 22-30
- **Background:** Manages product listings, order processing, customer chat across Tokopedia, Shopee, and brand website
- **Frustrations:** Manually updating stock across 3+ platforms, copy-pasting tracking numbers, inconsistent product info
- **Goal:** Single dashboard for all online channels. Auto-sync stock. Auto-push tracking numbers.
- **Tech literacy:** High. Digital native, comfortable with multiple platforms.

### Segment Sizing (Indonesia)

| Metric | Estimate | Source/Basis |
|---|---|---|
| Total Indonesian MSMEs | 65.5 million | Kemenkop UKM 2025 |
| Mid-market businesses (20-200 employees) | ~800,000 | ~1.2% of total MSMEs transitioning to formal mid-market |
| Mid-market retail specifically | ~200,000 | ~25% of mid-market is retail/wholesale |
| Mid-market retail with e-commerce | ~80,000 | ~40% have some online presence |
| Addressable (Java-based, fashion/beauty/electronics) | ~35,000 | Focused geography + target sub-segments |
| Realistic serviceable (Year 1-2) | ~5,000 | Can realistically reach with solo dev + digital marketing |
| Target penetration Year 1 | 50 tenants (1%) | Conservative for solo operation |
| Target penetration Year 2 | 500 tenants (10%) | With proven product and word-of-mouth |

---

## Section 3: Cost Structure

### Development Costs (Solo Developer)

| Item | Monthly Cost (IDR) | Notes |
|---|---|---|
| Developer salary (opportunity cost) | IDR 0 (self) | Solo dev building their own product; real opportunity cost ~IDR 25-40M/month for senior .NET dev in Indonesia |
| Development tools (JetBrains Rider / VS) | IDR 0 - 2.5M | VS Community free; Rider ~IDR 2.5M/year |
| GitHub Pro | IDR 60K | ~USD 4/month |
| Domain name (.id or .co.id) | IDR 150K/month | ~IDR 1.8M/year amortized |
| Design tools (Figma free tier) | IDR 0 | Free tier sufficient initially |
| **Subtotal** | **~IDR 210K/month** | Excluding opportunity cost |

### Infrastructure Costs

| Item | Monthly Cost (IDR) | Provider | Notes |
|---|---|---|---|
| Application server (VPS) | IDR 300K - 800K | IDCloudHost / DigitalOcean SGP | 2 vCPU, 4GB RAM sufficient for MVP |
| Database (MySQL managed) | IDR 400K - 1M | PlanetScale free tier → IDCloudHost | Start free, upgrade when needed |
| Redis (caching/sessions) | IDR 0 - 200K | Upstash free tier → managed | Free tier for MVP |
| CDN for storefront | IDR 0 - 300K | Cloudflare free tier | Free tier handles significant traffic |
| SSL certificates | IDR 0 | Let's Encrypt | Free |
| Email service (transactional) | IDR 0 - 150K | SendGrid free tier (100/day) | Sufficient for MVP |
| Object storage (product images) | IDR 50K - 200K | Cloudflare R2 / IDCloudHost S3 | R2 has free egress |
| Monitoring (uptime, errors) | IDR 0 | Better Stack free / Sentry free | Free tiers adequate |
| **Subtotal** | **IDR 750K - 2.65M/month** | | Scale with tenants |

### Payment Gateway Fees (Pass-through to Tenant)

| Provider | Transaction Fee | Settlement | Notes |
|---|---|---|---|
| **Midtrans** | QRIS: 0.7%, CC: 2.9% + IDR 2K, VA: IDR 4K flat | T+1 to T+2 | Most common in Indonesia |
| **Xendit** | QRIS: 0.7%, CC: 2.9% + IDR 2K, VA: IDR 4.5K, eWallet: 1.5-2% | T+1 to T+2 | Good API, popular with SaaS |
| **GoPay (via Midtrans/Xendit)** | 1-2% | Via aggregator | Through payment gateway |
| **OVO (via Xendit)** | 1.5-2% | Via aggregator | Through payment gateway |
| **ShopeePay** | 1.5% | Via aggregator | Through payment gateway |

**Note:** Payment gateway fees are charged to the tenant's customers or absorbed by the tenant. NiagaOne takes no cut of payment processing initially — we make money from SaaS subscription only.

### Support Costs

| Item | Monthly Cost (IDR) | Notes |
|---|---|---|
| Customer support (solo dev Year 1) | IDR 0 | Self-handled via ticket system + WhatsApp |
| Help desk tool | IDR 0 | Freshdesk free tier (up to 10 agents) |
| WhatsApp Business | IDR 0 | Free for basic use |
| Knowledge base / docs | IDR 0 | GitBook free tier or built-in |
| First support hire (Year 2) | IDR 5-7M/month | When reaching ~100 tenants |
| **Subtotal Year 1** | **IDR 0** | |
| **Subtotal Year 2** | **IDR 5-7M/month** | |

### Marketing Costs

| Channel | Monthly Cost (IDR) | Expected Result |
|---|---|---|
| Google Ads (search: "software toko", "aplikasi kasir") | IDR 2-5M | CPC IDR 3-8K, ~500-1500 clicks/month |
| Meta Ads (Facebook/Instagram targeting SME owners) | IDR 1-3M | Brand awareness + retargeting |
| Content marketing (blog, YouTube tutorial) | IDR 0 (self) | SEO long-term; YouTube is huge in Indonesia |
| Community (WhatsApp group, Telegram) | IDR 0 | Direct feedback loop, word-of-mouth |
| Offline events (bazaar, business expo) | IDR 1-2M occasionally | Tanah Abang, Bandung trade shows |
| **Subtotal** | **IDR 3-8M/month** | Conservative digital-first approach |

### Realistic Monthly Burn Rate

| Phase | Monthly Burn (IDR) | Monthly Burn (USD approx.) |
|---|---|---|
| **Pre-launch (Month 1-6)** | IDR 1-3M | ~USD 65-190 |
| **Post-launch MVP (Month 7-12)** | IDR 5-12M | ~USD 320-770 |
| **Growth (Year 2)** | IDR 15-30M | ~USD 960-1,920 |

**Break-even estimate:** At IDR 2M/month average subscription, need 8-15 tenants to cover post-launch costs. Achievable within 3-6 months of launch.

---

## Section 4: Value Propositions

### For Store Owners (Pemilik Toko Fisik)

| Pain Point | Current Solution | NiagaOne ERP Solution |
|---|---|---|
| Manual stock counting wastes 2-3 hours/day | Excel spreadsheets, manual tally | Real-time inventory with barcode scanning, auto stock alerts |
| Can't see stock across branches | WhatsApp photos between branch managers | Unified multi-branch dashboard, instant stock visibility |
| Tax reporting is painful | Hire tax consultant (IDR 2-5M/month) or manual calc | Auto PPN 12% calculation, e-Faktur ready export |
| Staff steal or make mistakes, can't prove it | No audit trail | Full transaction audit trail, per-user RBAC, shift reports |
| Accepting payments is fragmented | Separate EDC, QRIS standee, manual reconciliation | Unified payment acceptance: cash, QRIS, eWallet, VA — one reconciliation |

### For Online Shop Owners (Pemilik Toko Online)

| Pain Point | Current Solution | NiagaOne ERP Solution |
|---|---|---|
| Overselling when stock runs out | Manual stock updates on each marketplace | Auto-sync stock to Tokopedia, Shopee, own storefront |
| Order processing takes too long | Copy-paste from marketplace to shipping label | Auto-pull orders, generate shipping labels, push tracking numbers |
| No idea which channel is profitable | Separate analytics per platform | Unified P&L by channel, customer acquisition cost per channel |
| Returns are chaos | WhatsApp + manual Excel tracking | Structured return workflow with inventory auto-adjustment |

### For Omnichannel Businesses (Toko Fisik + Online)

| Pain Point | Current Solution | NiagaOne ERP Solution |
|---|---|---|
| **The #1 Pain: Stock does not sync** | Manually reconcile at end of day | Real-time inventory sync across ALL channels (store POS, website, Tokopedia, Shopee) |
| Customer buys online, wants to return in store | "Sorry, you have to return via the same channel" | Omnichannel returns: buy online, return in-store (and vice versa) |
| Can't do click-and-collect | Not offered | Buy Online, Pickup In-Store (BOPIS) built-in |
| Promotions don't sync | Different promo on each channel | Unified promotion engine across all channels |
| No single view of customer | Different customer databases everywhere | Unified customer profile across all touchpoints |

### Unique Value vs. Competitors

```
┌──────────────────────────────────────────────────────────────────────┐
│                    NiagaOne ERP Unique Value                       │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  1. LOGISTICS DNA                                                    │
│     Built FROM a logistics system. Shipping, tracking, warehouse     │
│     management are core — not bolted on. No competitor at this       │
│     price point has this.                                            │
│                                                                      │
│  2. TRUE OMNICHANNEL AS DEFAULT                                      │
│     Not "POS + e-commerce add-on." The system IS omnichannel.        │
│     Inventory is one pool, allocated across channels.                │
│                                                                      │
│  3. MID-MARKET FIT                                                   │
│     Complex enough for 200+ employee operations.                     │
│     Simple enough that a kasir can learn in 30 minutes.              │
│     Priced for IDR 2-5M/month, not IDR 20M.                         │
│                                                                      │
│  4. INDONESIA-NATIVE                                                 │
│     PPN 12% built into every transaction. e-Faktur export.          │
│     QRIS/GoPay/OVO/VA as first-class payment methods.               │
│     Bahasa Indonesia is the PRIMARY language, not a translation.     │
│     IDR currency with proper thousand-separator (titik, not koma).   │
│                                                                      │
│  5. MODERN TECH, SOLO-DEV EFFICIENT                                  │
│     .NET 8 + Blazor = one language (C#) for API + frontend.         │
│     Clean Architecture = maintainable as it scales.                  │
│     Multi-tenant from Day 1 = operational efficiency.                │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### Why Mid-Market? (The Gap Argument)

Indonesian mid-market retail businesses (20-200 employees, IDR 2-50B revenue) face a "software no-man's land":

- **Moka/Majoo** max out at ~20 employees. They're POS systems, not ERPs. When Rina opens her 4th branch, Moka can't handle inter-branch transfers, consolidated purchasing, or warehouse-to-store allocation.
- **SAP Business One** starts at IDR 200M+ implementation + IDR 20M/month. Plus requires a consultant for 3-6 months. For a business doing IDR 10B/year revenue, this is absurd.
- **HashMicro** is better priced (IDR 10M+/month) but still requires implementation, targets larger companies, and has a reputation for slow onboarding.
- **Jurnal by Mekari** is accounting-first. It's great at books, terrible at operations. No POS, no warehouse management, no logistics.

NiagaOne ERP sits in this gap: **operationally complete, Indonesian-native, self-serve onboarding, priced at IDR 2-5M/month.**

---

## Section 5: Strategic Trade-offs

### What We ARE Building vs. What We Are NOT

| WE ARE BUILDING | WE ARE NOT BUILDING |
|---|---|
| Inventory management (multi-location, variants, barcode) | Full manufacturing/production planning (MRP) |
| Point of Sale (touch-optimized, offline-capable) | Hardware POS terminal (use tablets + receipt printer) |
| Sales orders & invoicing with PPN | Full double-entry accounting (integrate with Jurnal/Accurate) |
| Purchasing & supplier management | Supplier portal / EDI |
| E-commerce storefront (basic, functional) | Shopify-competitor storefront builder |
| Marketplace integration (Tokopedia, Shopee) | Marketplace itself |
| Warehouse management (receive, pick, pack, ship) | Warehouse automation / robotics integration |
| Shipping & tracking (core DNA) | Own delivery fleet service (3PL) |
| Basic financial reports (P&L, cash flow) | Audit-grade financial statements |
| QRIS/eWallet/VA payment acceptance | Payment gateway (we use Midtrans/Xendit) |
| Customer database & basic loyalty | Full CRM / marketing automation |
| Bahasa Indonesia + English | 10+ languages |
| Multi-branch within Indonesia | Multi-country / multi-currency (Year 3+) |

### Build vs. Integrate Decisions

| Capability | Decision | Rationale |
|---|---|---|
| **Authentication & RBAC** | BUILD (already done) | Core to multi-tenancy; already built in NiagaOne |
| **Inventory management** | BUILD | Core differentiator; must be tightly integrated |
| **POS** | BUILD | Must be fast, offline-capable, deeply integrated with inventory |
| **Sales & purchasing** | BUILD | Core ERP functionality |
| **E-commerce storefront** | BUILD (basic) | Must sync with inventory in real-time; basic is sufficient |
| **Accounting** | INTEGRATE | Let Jurnal/Accurate/BukuKas handle GL. We export journal entries. |
| **Payment processing** | INTEGRATE (Midtrans/Xendit) | Not a payments company; use established gateways |
| **Marketplace sync** | INTEGRATE (Tokopedia/Shopee API) | Use official seller APIs |
| **Shipping/logistics** | BUILD (core DNA) + INTEGRATE (JNE, J&T, SiCepat APIs) | Extend existing shipment system; integrate carrier APIs |
| **Email/notifications** | INTEGRATE (SendGrid, WA Business API) | Commodity service |
| **Tax filing (e-Faktur)** | BUILD (export format) | Generate CSV/XML for DJP upload; don't build filing portal |
| **Barcode scanning** | INTEGRATE (browser API) | Use device camera via Web API; no custom hardware |

### Depth vs. Breadth Trade-offs

**Year 1: Depth over breadth.**

Do fewer things excellently rather than many things poorly. The priority stack:

1. **Inventory management** — Must be rock-solid. Real-time, multi-location, variant-aware. This is the foundation.
2. **POS** — Must be fast (< 2 second transaction). This is what cashiers use 8 hours/day.
3. **Sales orders** — Online order processing, invoicing.
4. **Basic e-commerce storefront** — Product catalog, cart, checkout with Midtrans.
5. **Marketplace sync** — Tokopedia first, then Shopee. Stock sync + order pull.

What gets postponed to Year 2: Purchasing/procurement, advanced reporting, loyalty programs, multi-warehouse optimization, clinic configuration.

### Indonesia-First: Why Local Wins

| Global-First Approach (Why it fails) | Local-First Approach (Why it wins) |
|---|---|
| Build in English, translate later | Build in Bahasa Indonesia from Day 1; English as secondary |
| USD-based pricing, convert to IDR | IDR-native: pricing, invoicing, reports all in IDR |
| Generic tax system, add PPN later | PPN 12% baked into every transaction calculation |
| Standard payment methods (Stripe) | QRIS, GoPay, OVO, ShopeePay, Virtual Account from Day 1 |
| Global shipping carriers | JNE, J&T Express, SiCepat, AnterAja, Pos Indonesia |
| Ignore local business practices | Support nota/kwitansi format, cash drawer management, WhatsApp order notifications |

Global competitors entering Indonesia always struggle with these details. A local-first approach creates immediate trust and usability.

### Solo Dev Constraints → Product Shaping

Being a solo developer is a constraint that shapes the product in specific (sometimes advantageous) ways:

| Constraint | How It Shapes the Product |
|---|---|
| Can't build everything at once | Forces ruthless prioritization → better core product |
| Can't do custom implementations per client | Forces true multi-tenant SaaS → scalable architecture |
| Can't provide phone support | Forces self-serve UX → better product, lower support burden |
| Can't build native mobile apps | Forces progressive web app (PWA) → one codebase, works on any device |
| Can't hire QA team | Forces automated testing → higher code quality |
| Can't do enterprise sales | Forces product-led growth → sign up, try, buy online |
| .NET 8 + Blazor = one language | Solo dev can build full stack in C# → faster development |

---

## Section 6: Key Metrics

### North Star Metric

**Weekly Active Tenants Processing Transactions (WATPT)**

A tenant is "active" if they processed at least 1 sales transaction (POS or online order) in the past 7 days. This metric captures:
- Acquisition (they signed up)
- Activation (they set up products and started selling)
- Retention (they keep using it week over week)
- Value (transactions = real business happening on the platform)

### Metrics by Category

#### Acquisition Metrics
| Metric | MVP Stage Target | Growth Stage Target |
|---|---|---|
| Website visitors/month | 1,000 | 20,000 |
| Sign-up conversion rate | 5% (50 sign-ups/month) | 8% (1,600/month) |
| Cost per sign-up | IDR 50K | IDR 30K |
| Sign-up to paid conversion | 10% | 15% |
| Customer acquisition cost (CAC) | IDR 500K | IDR 300K |

#### Activation Metrics
| Metric | Definition | Target |
|---|---|---|
| Time to first product created | Sign-up → first product in catalog | < 10 minutes |
| Time to first transaction | Sign-up → first POS sale or online order | < 24 hours |
| Setup completion rate | % who complete: products + payment + first sale | > 40% |
| Onboarding NPS | Net Promoter Score after first week | > 40 |

#### Retention Metrics
| Metric | MVP Stage Target | Growth Stage Target |
|---|---|---|
| Week 1 retention | 70% | 80% |
| Month 1 retention | 50% | 65% |
| Month 3 retention | 35% | 55% |
| Month 12 retention | 25% | 45% |
| Monthly churn rate | < 8% | < 4% |

#### Revenue Metrics
| Metric | MVP Stage | Growth Stage |
|---|---|---|
| Monthly Recurring Revenue (MRR) | IDR 100M (50 tenants x IDR 2M) | IDR 1B (500 tenants x IDR 2M avg) |
| Average Revenue Per Tenant (ARPT) | IDR 2M/month | IDR 2.5M/month |
| LTV (Lifetime Value) | IDR 24M (12-month avg retention) | IDR 60M (24-month avg retention) |
| LTV:CAC ratio | 48:1 | 200:1 |
| Net Revenue Retention (NRR) | 95% | 110% (expansion via branches) |

#### Operational Metrics
| Metric | Target |
|---|---|
| API uptime | 99.5% (MVP) → 99.9% (Growth) |
| API response time (p95) | < 500ms |
| POS transaction time | < 2 seconds end-to-end |
| Inventory sync latency | < 5 seconds across channels |
| Support ticket response time | < 4 hours (business hours) |
| Support tickets per tenant/month | < 3 (indicates good UX) |

### What to Measure When

| Stage | Focus | Key Dashboard |
|---|---|---|
| **MVP (Month 1-6)** | "Are people signing up and actually using it?" | Sign-ups, activation rate, Week 1 retention, NPS |
| **Product-Market Fit (Month 7-12)** | "Do they keep coming back and paying?" | Month 3 retention, MRR, churn rate, NPS > 40 |
| **Growth (Year 2)** | "Can we grow efficiently?" | CAC, LTV:CAC, NRR, MRR growth rate |

---

## Section 7: Growth Strategy

### Go-To-Market for Indonesian Mid-Market

#### Phase 1: Product-Led, Community-Driven (Month 1-12)

**Primary channel: Digital inbound + WhatsApp community**

Indonesian SME owners discover software through:
1. **Google Search** — "aplikasi kasir toko" (POS app for store), "software stok barang" (inventory software) — SEO + SEM
2. **YouTube** — Tutorial videos ("Cara kelola stok toko baju dengan mudah") — Content marketing
3. **WhatsApp groups** — Business owner communities (Komunitas Pedagang Tanah Abang, etc.) — Community
4. **Instagram/TikTok** — Before/after demonstrations — Social proof
5. **Word of mouth** — "Toko sebelah pakai apa?" (What does the shop next door use?) — Product-led

**Go-to-market sequence:**
1. Build 10 detailed YouTube tutorials in Bahasa Indonesia (free)
2. SEO-optimized blog: "Panduan Lengkap Mengelola Toko Retail" series
3. Google Ads targeting: "aplikasi kasir", "software toko online", "inventory management toko"
4. Create WhatsApp community group for beta users
5. Offer first 20 tenants free for 3 months (beta pricing) in exchange for feedback + testimonials
6. After 20 beta tenants: launch referral program (1 month free for referrer + referee)

#### Phase 2: Partnerships + Local Presence (Year 2)

1. **Accounting firm partnerships** — Offer white-label or referral commission to accounting firms serving mid-market retail. They recommend our ERP, we export clean data to their accounting software.
2. **POS hardware bundling** — Partner with hardware distributors (receipt printers: Epson TM-T82, barcode scanners: Honeywell) for recommended bundles.
3. **Trade show presence** — Booth at SIAL InterFood (F&B), Indonesia Fashion Week, APKASI Otonomi Expo.
4. **Government SME programs** — Register as approved digital solution under Kemenkop UKM's digitalization programs.

### Pricing Strategy

**Tiered subscription, per-branch pricing (not per-user)**

| Plan | Monthly Price (IDR) | Annual Price (IDR) | Includes | Target |
|---|---|---|---|---|
| **Starter** | IDR 799K/branch | IDR 7.99M/branch (save 17%) | 1 branch, POS, inventory, 3 users, 1,000 SKUs | Single-store owners testing the waters |
| **Business** | IDR 1.99M/branch | IDR 19.9M/branch | Up to 5 branches, omnichannel, unlimited users, 10,000 SKUs, marketplace sync | Core target: multi-branch retail |
| **Enterprise** | IDR 3.99M/branch | IDR 39.9M/branch | Unlimited branches, API access, custom reports, dedicated support, unlimited SKUs | Upper mid-market scaling |

**Why per-branch, not per-user:**
- Indonesian businesses have high employee turnover; per-user pricing creates friction
- Branch count correlates with business value and operational complexity
- Simpler to understand: "IDR 2M per toko" (per store)
- Natural expansion revenue: as business grows, they add branches

**Additional pricing elements:**
- **Free trial:** 14 days, full Business plan features
- **E-commerce storefront add-on:** +IDR 500K/month (includes hosting, CDN, custom domain)
- **Marketplace integration:** Included in Business and Enterprise plans
- **Transaction fees:** IDR 0 from NiagaOne (payment gateway fees are separate, paid directly to Midtrans/Xendit)

### Distribution Channels

| Channel | Role | Cost |
|---|---|---|
| **Direct (website sign-up)** | Primary acquisition channel | SEM/SEO cost only |
| **WhatsApp** | Sales + support + community | Free |
| **YouTube** | Education + trust-building | Time investment only |
| **Referral program** | Organic growth multiplier | 1 month free per referral |
| **Accounting firm partners** | Warm introductions | 10-15% revenue share |
| **Hardware resellers** | Bundled sales | Co-marketing |
| **Tokopedia/Shopee seller programs** | Access to marketplace sellers | Partnership/integration |

### Partnership Opportunities

| Partner Type | Specific Companies | Value Exchange |
|---|---|---|
| **Payment gateways** | Midtrans, Xendit | They list us as recommended ERP; we drive transaction volume |
| **Accounting software** | Jurnal (Mekari), Accurate, BukuKas | Bidirectional integration; mutual referrals |
| **Shipping carriers** | JNE, J&T Express, SiCepat, AnterAja | API integration; co-marketing to their merchant base |
| **Marketplaces** | Tokopedia (Seller Center API), Shopee (Open Platform) | Official integration partner; listing in their app marketplace |
| **POS hardware** | Epson Indonesia, Honeywell, Sunmi | Certified hardware list; bundled packages |
| **Telco/digital banks** | BCA Digital, Jago, GoPay Merchant | Payment + lending integrations |
| **Industry associations** | APRINDO (retail association), Hipmi | Speaking opportunities, member benefits |

### Expansion Path

```
Year 1: Jakarta + Bandung corridor (Java)
         ↓
Year 2: Surabaya, Semarang, Yogyakarta, Medan (major Indonesian cities)
         ↓
Year 2-3: All of Java + Sumatra + Bali
         ↓
Year 3: Malaysia (similar culture, Malay language overlap, comparable market structure)
         ↓
Year 3-4: Philippines (high e-commerce growth, English-speaking, similar SME landscape)
```

---

## Section 8: Core Capabilities Required

### Technical Capabilities

| Capability | Status | Priority | Effort (Solo Dev) |
|---|---|---|---|
| **Multi-tenant architecture** | Needs implementation (currently single-tenant) | CRITICAL | 3-4 weeks |
| **Tenant isolation (database-per-tenant or schema-per-tenant)** | Not started | CRITICAL | 2-3 weeks |
| **Inventory management module** | Not started | CRITICAL | 4-6 weeks |
| **POS module (Blazor-based)** | Not started | CRITICAL | 4-6 weeks |
| **Product catalog (variants, barcode, images)** | Not started | CRITICAL | 3-4 weeks |
| **Sales order management** | Not started | HIGH | 3-4 weeks |
| **PPN tax calculation engine** | Not started | HIGH | 1-2 weeks |
| **Payment gateway integration (Midtrans)** | Not started | HIGH | 2-3 weeks |
| **E-commerce storefront** | Not started | MEDIUM | 4-6 weeks |
| **Marketplace integration (Tokopedia API)** | Not started | MEDIUM | 3-4 weeks |
| **Shipping carrier integration (JNE, J&T)** | Partially exists (shipment tracking) | MEDIUM | 2-3 weeks |
| **Reporting engine** | Not started | MEDIUM | 2-3 weeks |
| **PWA / offline POS capability** | Not started | MEDIUM | 2-3 weeks |
| **Authentication & RBAC** | DONE | -- | -- |
| **Clean Architecture foundation** | DONE | -- | -- |
| **API infrastructure (REST + Swagger)** | DONE | -- | -- |
| **Blazor frontend foundation** | DONE | -- | -- |
| **Warehouse management** | Partially DONE (basic CRUD) | Extend | 2-3 weeks |
| **Shipment tracking** | DONE | Extend with carrier APIs | 1-2 weeks |

### What the Existing NiagaOne Provides (Reusable Assets)

The current codebase provides significant reusable infrastructure:

- **Clean Architecture pattern** — Domain/Application/Infrastructure/API layers are established. New modules (Inventory, POS, Sales) follow the same pattern.
- **RBAC with 18 permissions** — Extensible permission system. Add `inventory.read`, `pos.create`, `sales.manage`, etc. using the same `RequirePermissionAttribute` filter.
- **JWT auth with refresh token rotation** — Production-ready auth. Add tenant claims to JWT for multi-tenancy.
- **Entity patterns** — `Shipment`, `Warehouse`, `Driver`, `Vehicle` entities demonstrate the pattern. `Product`, `SalesOrder`, `PurchaseOrder` follow the same structure.
- **Repository pattern** — All repositories implement interfaces. New repositories follow the same pattern.
- **Blazor frontend** — Dashboard, auth flows, CRUD pages exist. New modules extend the same layout.
- **Shipment/tracking system** — Directly reusable for order fulfillment tracking.
- **Warehouse CRUD** — Foundation for warehouse management module.

### Operational Capabilities

| Capability | How to Address as Solo Dev |
|---|---|
| **Customer onboarding** | Self-serve: in-app tutorial, YouTube walkthrough, template data |
| **Customer support** | WhatsApp Business + Freshdesk free tier; max 2 hours/day on support |
| **Documentation** | In-app help tooltips + knowledge base (built with the product) |
| **Billing & subscription management** | Integrate Xendit recurring billing API; automate fully |
| **Tenant provisioning** | Automated: sign up → database created → seed data → ready in 60 seconds |
| **Monitoring & incident response** | Better Stack (uptime) + Sentry (errors) + PagerDuty free tier |
| **Data backup** | Automated daily MySQL dumps to object storage; tested restore process |

### Market Capabilities

| Capability | How to Address as Solo Dev |
|---|---|
| **Content marketing** | Write 2 blog posts/month, 2 YouTube videos/month in Bahasa Indonesia |
| **Paid acquisition** | Self-manage Google Ads + Meta Ads; IDR 5M/month budget |
| **Sales** | No outbound sales; purely inbound product-led |
| **Community management** | WhatsApp group; respond 2x/day; share tips/updates |
| **Competitor monitoring** | Monthly review of Moka/Majoo/iReap feature updates |
| **Customer development** | Monthly 30-min calls with 5 tenants for feedback |

### Key Gaps and Mitigation

| Gap | Risk Level | Mitigation |
|---|---|---|
| **No mobile native app** | Medium | PWA provides 90% of native experience; revisit in Year 2 |
| **No dedicated QA** | Medium | Automated test suite (unit + integration); beta testers |
| **No DevOps team** | Low | GitHub Actions CI/CD; containerized deployment |
| **No designer** | Medium | Use Tailwind CSS + proven UI component library; study competitor UX |
| **No sales team** | Low-Medium | Product-led growth; word-of-mouth; content marketing |
| **Single point of failure (solo dev)** | HIGH | Document everything; clean code; modular architecture; consider co-founder in Year 2 |
| **No legal/compliance expertise** | Medium | Consult lawyer for Terms of Service; study OJK/Kominfo requirements for SaaS |

---

## Section 9: Defensibility & Moats

### Moat Strategy (Short-term → Long-term)

```
YEAR 1                    YEAR 2                    YEAR 3+
┌──────────────┐          ┌──────────────┐          ┌──────────────────┐
│ LOCAL MARKET │          │  SWITCHING   │          │  DATA NETWORK    │
│ KNOWLEDGE    │──────────│  COSTS       │──────────│  EFFECTS         │
│              │          │              │          │                  │
│ • PPN baked in│         │ • Years of   │          │ • Industry       │
│ • QRIS/eWallet│         │   transaction│          │   benchmarks     │
│ • Bahasa first│         │   data       │          │ • Demand          │
│ • IDR native │          │ • Staff      │          │   forecasting    │
│ • Local biz  │          │   trained    │          │ • Pricing        │
│   practices  │          │ • Integrations│         │   optimization   │
│              │          │   configured │          │ • Supplier       │
│ (Easy to     │          │              │          │   network        │
│  start but   │          │ (Grows with  │          │                  │
│  hard to     │          │  each month  │          │ (Unique asset    │
│  replicate   │          │  of usage)   │          │  from aggregate  │
│  correctly)  │          │              │          │  tenant data)    │
└──────────────┘          └──────────────┘          └──────────────────┘
```

### 1. Data Network Effects

As more tenants use the platform, aggregate (anonymized) data becomes increasingly valuable:

- **Industry benchmarks:** "Your fashion store's inventory turnover is 4.2x/year. The average for similar stores in Jakarta is 5.8x." — This insight is only possible with hundreds of tenants.
- **Demand forecasting:** With enough transaction data, predict seasonal demand patterns ("Lebaran shopping starts 3 weeks before Ramadan, not 2").
- **Supplier intelligence:** "Supplier X has a 94% on-time delivery rate based on 2,000 purchase orders across our network."
- **Pricing insights:** "Stores pricing product category Y between IDR X-Z see 15% higher conversion."

Each new tenant makes the platform more valuable for all tenants. This moat compounds over time and is nearly impossible for a new competitor to replicate without similar scale.

### 2. Switching Costs

Once a retailer commits to an ERP, switching is painful:

| Switching Cost | Strength |
|---|---|
| **Transaction history** | Years of sales data, reports, tax records. Migration is terrifying. |
| **Staff training** | Retraining 20-200 employees on a new system costs real money and lost productivity. |
| **Integration configuration** | Marketplace connections, payment gateway setup, shipping carrier configuration. Each took hours. |
| **Business process adaptation** | Workflows adapted to the system. Changing ERP means changing processes. |
| **Data migration risk** | Product catalog with images, variants, pricing, supplier info. Migration always loses something. |

**Strategy:** Make data export easy (build trust), but make the product so good they never want to leave.

### 3. Integration Ecosystem

Over time, build a network of integrations that increases lock-in:

- **Accounting:** Jurnal (Mekari), Accurate, BukuKas — journal entry export
- **Payment:** Midtrans, Xendit, GoPay, OVO — unified payment
- **Shipping:** JNE, J&T, SiCepat, AnterAja, Pos Indonesia, GoSend — rate comparison + auto-booking
- **Marketplace:** Tokopedia, Shopee, Lazada, Blibli — stock sync + order management
- **Banking:** BCA, Mandiri, BNI — auto-reconciliation via Open Banking
- **Government:** e-Faktur (DJP), NIB (OSS) — compliance automation

Each integration makes the platform stickier. A competitor would need to replicate the entire ecosystem.

### 4. Local Market Knowledge (Embedded in Product)

Things that take years to learn and embed correctly:

- PPN 12% with proper e-Faktur format (XML schema that DJP actually accepts)
- Indonesian address format (RT/RW, Kelurahan, Kecamatan, Kota/Kabupaten, Provinsi, Kode Pos)
- How Indonesian retailers actually do business (cash-heavy, nota/kwitansi, WhatsApp-first communication)
- Lebaran/Ramadan seasonal patterns in inventory and purchasing
- Marketplace-specific quirks (Tokopedia's seller center API versioning, Shopee's webhook format)
- Local shipping cost calculation (volumetric weight standards differ by carrier)
- QRIS MDR (Merchant Discount Rate) rules from Bank Indonesia

A Silicon Valley startup entering Indonesia would take 1-2 years to get these details right.

### 5. Long-term Defensibility Strategy

| Timeframe | Primary Moat | Secondary Moat |
|---|---|---|
| **Year 1** | Local market knowledge + first-mover in mid-market gap | Product quality (clean UX, fast POS) |
| **Year 2** | Switching costs (data + training + integrations) | Integration ecosystem |
| **Year 3** | Data network effects (benchmarks, forecasting) | Community/brand (trusted Indonesian ERP) |
| **Year 5** | All of the above compounding | Platform effects (3rd-party app marketplace) |

---

## Phased Roadmap

### Month 1-3: Foundation & Multi-Tenant Core

**Goal:** Transform NiagaOne into a multi-tenant platform with core retail capabilities.

| Week | Deliverable | Details |
|---|---|---|
| 1-2 | Multi-tenant architecture | Tenant entity, database-per-tenant strategy, tenant resolution middleware, tenant-aware DbContext |
| 3-4 | Product catalog | Product entity with variants (size/color), categories, barcode/SKU, product images (object storage) |
| 5-6 | Inventory management | Stock tracking per location, stock movements (in/out/transfer/adjustment), real-time stock levels |
| 7-8 | Tenant onboarding flow | Sign-up → tenant provisioning → seed demo data → guided setup wizard |
| 9-10 | POS module (basic) | Product search, cart, cash payment, receipt generation, shift open/close |
| 11-12 | PPN tax engine | Tax calculation on transactions, tax-inclusive/exclusive pricing, basic tax report |

**Key milestone:** By end of Month 3, a retailer can sign up, add products, and make POS sales with proper PPN calculation.

### Month 4-6: Payments, Sales Orders & Storefront

**Goal:** Enable omnichannel sales with digital payments and basic e-commerce.

| Week | Deliverable | Details |
|---|---|---|
| 13-14 | Payment gateway integration | Midtrans integration: QRIS, credit card, VA, GoPay, ShopeePay |
| 15-16 | Sales order management | Create orders, order statuses, invoicing, payment recording |
| 17-18 | E-commerce storefront (basic) | Product catalog page, product detail, cart, checkout with Midtrans, responsive design |
| 19-20 | Inventory sync engine | Real-time stock sync between POS and storefront, stock allocation/reservation |
| 21-22 | Shipping integration | JNE + J&T Express API: rate checking, waybill generation, tracking webhook |
| 23-24 | Beta launch preparation | Landing page, documentation, onboarding flow testing, invite first 10 beta users |

**Key milestone:** By end of Month 6, an omnichannel retailer can sell in-store (POS) and online (storefront), with real-time inventory sync, digital payment acceptance, and integrated shipping.

### Month 7-12: Growth Features & Market Launch

**Goal:** Public launch, marketplace integrations, grow to 50 paying tenants.

| Month | Deliverable |
|---|---|
| **Month 7** | Tokopedia Seller API integration (product sync, stock sync, order pull, tracking push) |
| **Month 8** | Shopee Open Platform integration (same scope as Tokopedia) |
| **Month 9** | Reporting engine: daily sales, inventory valuation, P&L by channel, tax summary for SPT |
| **Month 10** | Multi-branch operations: inter-branch transfer, branch-level stock, consolidated dashboard |
| **Month 11** | Purchasing module: purchase orders, supplier management, goods receiving with stock auto-update |
| **Month 12** | Performance optimization, security audit, official public launch, pricing goes live |

**Key milestone:** By end of Month 12, platform is publicly available with full omnichannel capability, marketplace integrations, multi-branch support, and 50 paying tenants.

### Year 2: Scale & Expand

| Quarter | Focus |
|---|---|
| **Q1 Y2** | Customer management (unified customer profiles, purchase history across channels), basic loyalty program (points), WhatsApp notification integration |
| **Q2 Y2** | Accounting integration (Jurnal/Accurate export), e-Faktur generation for DJP, advanced financial reports |
| **Q3 Y2** | Clinic configuration launch (appointment scheduling, patient records, medical inventory, BPJS integration path), hire first support staff |
| **Q4 Y2** | Open API for third-party integrations, app marketplace foundation, Lazada/Blibli marketplace integration |

---

## Risk Assessment: Top 10 Risks

| # | Risk | Probability | Impact | Mitigation |
|---|---|---|---|---|
| 1 | **Solo developer burnout / bus factor** | HIGH | CRITICAL | Strict work-life balance. Modular architecture so modules can be outsourced. Seek co-founder by Month 6. Document everything in code (not in your head). |
| 2 | **Multi-tenant data breach** | LOW | CRITICAL | Database-per-tenant isolation (not row-level). Regular security audits. Encrypt sensitive data at rest. Penetration testing before public launch. |
| 3 | **Slow product-market fit discovery** | MEDIUM | HIGH | Beta users from Day 1. Weekly feedback calls. Be willing to pivot sub-segment if fashion retail doesn't convert. Measure activation, not just sign-ups. |
| 4 | **Moka/Majoo move upmarket** | MEDIUM | HIGH | They are backed by GoTo/VCs focused on volume SME. Moving upmarket requires different architecture (multi-branch, warehouse). Our logistics DNA is hard to replicate. Move fast. |
| 5 | **Payment gateway dependency** | LOW | MEDIUM | Integrate both Midtrans AND Xendit. Abstract payment layer so switching is possible. No exclusive agreements. |
| 6 | **Marketplace API changes (Tokopedia/Shopee)** | MEDIUM | MEDIUM | Abstract marketplace integration behind adapter pattern. Monitor API changelog. Build buffer in sync logic for API downtime. |
| 7 | **Indonesian regulatory changes (tax/data)** | MEDIUM | MEDIUM | Monitor DJP/Kominfo announcements. Build tax engine as configurable (rate changes easy). Ensure data residency in Indonesia (IDCloudHost or SGP). |
| 8 | **Cash flow negative for too long** | MEDIUM | HIGH | Keep burn rate under IDR 10M/month. Day job or freelancing income as runway. Target break-even at 10-15 tenants. Don't scale marketing until PMF confirmed. |
| 9 | **Performance at scale (multi-tenant)** | MEDIUM | MEDIUM | Load test with simulated tenants before launch. Database-per-tenant allows independent scaling. Use connection pooling. Cache aggressively with Redis. |
| 10 | **Competitor offers free tier that undercuts pricing** | LOW | MEDIUM | Compete on value, not price. Mid-market buyers pay for reliability and features, not cheapest option. Free tier signals "not serious" to mid-market. Offer 14-day trial, not freemium. |

---

## Competitive Analysis Matrix

### Feature Comparison: NiagaOne ERP vs. Top 5 Indonesian Competitors

| Feature | NiagaOne ERP (Planned) | Moka POS (GoTo) | Majoo | iReap POS Pro | Jurnal (Mekari) | HashMicro |
|---|---|---|---|---|---|---|
| **Target Segment** | Mid-market (20-1000 emp) | SME (1-20 emp) | SME (1-30 emp) | SME-Mid (5-100 emp) | SME-Mid (accounting focus) | Mid-Enterprise (100+) |
| **Monthly Price (IDR)** | 799K-3.99M/branch | 299K-599K/outlet | 249K-799K/outlet | 150K-500K/device | 499K-1.99M/company | 10M+ (custom) |
| | | | | | | |
| **CORE POS** | | | | | | |
| Point of Sale | Yes (planned) | Yes (core) | Yes (core) | Yes (core) | No | Yes |
| Offline POS | Yes (PWA) | Yes (native app) | Yes (native app) | Yes (Android) | N/A | Yes |
| Multi-payment (QRIS, eWallet) | Yes | Yes | Yes | Limited | N/A | Yes |
| Receipt customization | Yes | Yes | Yes | Basic | N/A | Yes |
| | | | | | | |
| **INVENTORY** | | | | | | |
| Multi-location stock | Yes (core) | Basic (add-on) | Basic | Yes | Limited | Yes |
| Product variants (size/color) | Yes | Basic | Basic | Yes | N/A | Yes |
| Barcode/SKU management | Yes | Yes | Yes | Yes | N/A | Yes |
| Stock transfer between branches | Yes | No | No | Basic | N/A | Yes |
| Warehouse management (WMS) | Yes (core DNA) | No | No | No | No | Basic |
| Inventory valuation (FIFO/AVG) | Yes | No | No | Basic | Yes (accounting) | Yes |
| | | | | | | |
| **E-COMMERCE** | | | | | | |
| Built-in storefront | Yes | No (GoStore separate) | Basic | No | No | No |
| Tokopedia integration | Yes | No | No | No | No | Limited |
| Shopee integration | Yes | No | No | No | No | Limited |
| Real-time stock sync (omnichannel) | Yes (core) | No | No | No | No | Limited |
| | | | | | | |
| **LOGISTICS & SHIPPING** | | | | | | |
| Shipping carrier integration | Yes (JNE, J&T, SiCepat) | No | Basic (GoSend) | No | No | Basic |
| Shipment tracking | Yes (core DNA) | No | No | No | No | Basic |
| Delivery management | Yes | No | No | No | No | Basic |
| Fleet management | Yes (existing) | No | No | No | No | No |
| | | | | | | |
| **FINANCE & TAX** | | | | | | |
| PPN calculation | Yes | Basic | Basic | Basic | Yes (core) | Yes |
| e-Faktur export | Yes (planned) | No | No | No | Yes | Yes |
| Basic P&L reporting | Yes | Basic | Basic | No | Yes (core) | Yes |
| Accounting integration | Yes (Jurnal, Accurate) | Limited | No | No | Native | Native |
| | | | | | | |
| **OPERATIONS** | | | | | | |
| Multi-branch management | Yes (core) | Basic (add-on) | Basic | Basic | Yes | Yes |
| RBAC (role-based access) | Yes (existing, 18 permissions) | Basic (3 roles) | Basic | Basic | Basic | Yes |
| Purchasing/procurement | Yes (Year 1) | No | No | No | Yes | Yes |
| Supplier management | Yes (Year 1) | No | No | No | Basic | Yes |
| | | | | | | |
| **TECHNOLOGY** | | | | | | |
| Multi-tenant SaaS | Yes | Yes | Yes | Semi (device license) | Yes | Semi (on-prem option) |
| API access | Yes (REST, existing) | Limited | No | No | Yes | Yes |
| Mobile app | PWA | Native (iOS/Android) | Native (Android) | Native (Android) | No | Yes |
| Bahasa Indonesia | Primary | Yes | Yes | Yes | Yes | Yes |
| White-label option | No (Year 3) | No | No | No | No | Yes |
| | | | | | | |
| **UNIQUE DIFFERENTIATOR** | Omnichannel + logistics + mid-market | GoTo ecosystem integration | All-in-one simplicity | Affordable inventory focus | Accounting strength | Full enterprise ERP |

### Competitive Summary

**NiagaOne ERP's winning formula:**

1. **vs. Moka/Majoo:** "You've outgrown POS tools. We're the next step — multi-branch, warehouse, logistics, e-commerce — without the SAP price tag."
2. **vs. iReap:** "Same inventory strength, but with modern UI, real omnichannel, and built-in logistics."
3. **vs. Jurnal:** "We handle operations (the hard part). Connect us to Jurnal for the books."
4. **vs. HashMicro/SAP:** "90% of the capability at 10% of the cost, with zero implementation time."

---

## Appendix A: Indonesian Tax Requirements (PPN/PPh)

| Requirement | Implementation |
|---|---|
| PPN (Pajak Pertambahan Nilai) | 12% on taxable goods/services (effective April 2025). Auto-calculated on every transaction. |
| Faktur Pajak | Generate e-Faktur format (XML) for upload to DJP e-Faktur application. Required for PKP businesses. |
| PPh 22 | Withholding tax on certain purchases. Track and report. |
| SPT Masa PPN | Monthly PPN return data. Export in DJP-compatible format. |
| NPWP validation | Validate customer/supplier NPWP format (15 or 16 digits per new NIK-based system). |
| Nota Retur | Standardized return note for PPN adjustment. |

## Appendix B: Indonesian Payment Landscape

| Method | Usage Share (2025) | Integration Path | Settlement |
|---|---|---|---|
| Cash | ~55% of retail | POS cash drawer management | Immediate |
| QRIS | ~20% (growing fast) | Midtrans/Xendit QRIS API | T+1 |
| Bank Transfer / VA | ~10% | Midtrans/Xendit VA API | T+1 |
| GoPay | ~5% | Via Midtrans/Xendit | T+1 to T+2 |
| OVO | ~3% | Via Xendit | T+1 to T+2 |
| ShopeePay | ~3% | Via Midtrans | T+1 to T+2 |
| Credit/Debit Card | ~3% | Midtrans/Xendit CC API | T+2 to T+3 |
| Dana | ~1% | Via Xendit | T+1 to T+2 |

## Appendix C: Key Indonesian Shipping Carriers

| Carrier | Market Share | API Availability | Strength |
|---|---|---|---|
| JNE | ~25% | Yes (REG, YES, OKE) | Most trusted, widest coverage |
| J&T Express | ~22% | Yes | Fast, e-commerce focused |
| SiCepat | ~15% | Yes | Cheapest, growing fast |
| AnterAja | ~10% | Yes | Good API, startup-friendly |
| Pos Indonesia | ~5% | Limited | Government, remote area coverage |
| GoSend / GrabExpress | ~8% | Via GoTo/Grab API | Same-day, intra-city |
| Ninja Express | ~5% | Yes | Shopee preferred partner |

## Appendix D: Multi-Tenant Architecture Decision

**Recommended: Database-per-tenant with shared application tier**

```
┌─────────────────────────────────────────────────────────┐
│                    Application Tier                       │
│         (Shared .NET 8 API + Blazor Frontend)            │
│                                                          │
│  ┌──────────────────────────────────────────────────┐    │
│  │  Tenant Resolution Middleware                     │    │
│  │  (subdomain: {tenant}.niagaone.id)            │    │
│  │  OR (header: X-Tenant-Id)                        │    │
│  │  → resolves to tenant connection string           │    │
│  └──────────────────────────────────────────────────┘    │
│                          │                               │
│  ┌──────────────────────────────────────────────────┐    │
│  │  Tenant-Aware DbContext                           │    │
│  │  (Connection string swapped per request)          │    │
│  └──────────────────────────────────────────────────┘    │
│                          │                               │
└──────────────────────────┼───────────────────────────────┘
                           │
          ┌────────────────┼────────────────┐
          │                │                │
   ┌──────┴──────┐  ┌─────┴───────┐  ┌─────┴───────┐
   │  tenant_001 │  │  tenant_002 │  │  tenant_003 │
   │  (MySQL DB) │  │  (MySQL DB) │  │  (MySQL DB) │
   │             │  │             │  │             │
   │  Products   │  │  Products   │  │  Products   │
   │  Orders     │  │  Orders     │  │  Orders     │
   │  Inventory  │  │  Inventory  │  │  Inventory  │
   │  ...        │  │  ...        │  │  ...        │
   └─────────────┘  └─────────────┘  └─────────────┘
                           │
                    ┌──────┴──────┐
                    │  master_db  │
                    │             │
                    │  Tenants    │
                    │  Plans      │
                    │  Billing    │
                    │  Users*     │
                    └─────────────┘
                    (* cross-tenant auth)
```

**Why database-per-tenant (not row-level with TenantId):**
1. **Data isolation** — Indonesian businesses are cautious about data sharing. Physical DB separation is a selling point.
2. **Performance** — No risk of cross-tenant query leaks or forgotten WHERE clauses.
3. **Backup/restore** — Can backup and restore individual tenants independently.
4. **Compliance** — Easier to demonstrate data isolation for any future Indonesian data protection regulations (UU PDP).
5. **Migration** — Can upgrade tenant schemas independently (canary deployments).

**Trade-off:** More operational overhead managing hundreds of databases. Mitigated by automation (Terraform/scripts for provisioning, automated migration runner).

---

*This document is a living strategy artifact. Review and update quarterly as market conditions and product progress evolve.*
