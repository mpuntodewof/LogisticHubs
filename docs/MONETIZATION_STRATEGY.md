# StockLedger — Pricing & Monetization Strategy

> Version 2.0 | Updated: 2026-04-13
> Focused pricing for Inventory + Finance SaaS

---

## 1. Pricing Philosophy

- **Flat monthly pricing** — retailers hate per-transaction fees (that's why they leave marketplaces)
- **Simple to understand** — a retailer should know their cost in 10 seconds
- **14-day free trial with full access** — value must be obvious before asking for money
- **No hidden fees, no setup costs, no lock-in contracts**

---

## 2. Subscription Tiers

| | Starter | Professional | Enterprise |
|---|---------|-------------|------------|
| **Monthly** | **Rp299.000** | **Rp599.000** | **Rp999.000** |
| **Annual (save 20%)** | Rp239.000/mo | Rp479.000/mo | Rp799.000/mo |
| **Target** | Small retailer, 1-2 channels | Growing retailer, 3+ channels | Multi-branch operation |
| **Active SKUs** | Up to 500 | Up to 5,000 | Unlimited |
| **Warehouses** | 1 | Up to 5 | Unlimited |
| **CSV imports/month** | 20 | 100 | Unlimited |
| **Users** | 3 | 10 | Unlimited |
| **Stock history** | 6 months | 2 years | Unlimited |
| **P&L report** | Basic | Per-channel detail | Custom + export |
| **Balance sheet** | - | Yes | Yes |
| **Custom roles** | - | Yes | Yes |
| **API access** | - | - | Yes |
| **Support** | Email (48h) | Email + WA (12h) | Dedicated (4h) |
| **Onboarding** | Self-serve docs | 1x video call | Dedicated setup |

### Why These Prices

| Tier | ~USD | Rationale |
|------|------|-----------|
| Starter Rp299K | ~$18 | Below "just try it" threshold. Comparable to Moka Starter. |
| Professional Rp599K | ~$37 | Sweet spot. 3-5x cheaper than Jubelio. Unlocks multi-warehouse + per-channel P&L. |
| Enterprise Rp999K | ~$62 | Premium but 10x cheaper than SAP/HashMicro. API access for integrations. |

---

## 3. Revenue Projections

### SaaS Revenue (Primary)

```
Month 1-3:   Validation (free trials)
             Revenue: Rp0
             Focus: 20 tenants, validate CSV import

Month 4-6:   First paying customers
             15 tenants: 8 Starter + 5 Pro + 2 Enterprise
             MRR: ~Rp6.3M

Month 7-12:  Growth
             50 tenants: 25 Starter + 18 Pro + 7 Enterprise
             MRR: ~Rp25M | ARR: ~Rp300M

Year 2:      Scale
             250 tenants: 100 Starter + 110 Pro + 40 Enterprise
             MRR: ~Rp135M | ARR: ~Rp1.6B
```

### Enterprise Licenses (Secondary — Month 6+)

Only pursue if inbound demand exists. Max 2-3 deals per quarter.

| License | Price | Includes |
|---------|-------|----------|
| Standard | Rp15M - 25M | Core app, 1yr updates, email support |
| Professional | Rp30M - 50M | Multi-branch, priority support, 1yr updates |
| Enterprise | Rp50M - 150M | Source code, custom deploy, training, dedicated support |

Annual maintenance renewal: 20% of license price/year.

---

## 4. Free Trial Design

### 14-Day Full Professional Access

| Aspect | Detail |
|--------|--------|
| Access level | Full Professional tier |
| Credit card required | No — just email + company name |
| Guided onboarding | Create warehouse > Import CSV > See stock dashboard |
| Conversion target | 25-35% |
| Post-expiry | Read-only for 30 days. Data preserved. |

### Automated Trial Emails

| Day | Trigger | Message |
|-----|---------|---------|
| 0 | Signup | Welcome + quickstart (import first CSV in 5 min) |
| 1 | No import yet | Step-by-step: export from Tokopedia/Shopee |
| 3 | First import done | "Your stock dashboard is live!" |
| 7 | Mid-trial | "Users save 8 hours/month on stock reconciliation" |
| 12 | 2 days left | "Trial expires soon — choose a plan" |
| 14 | Expired | "Data safe for 30 days — upgrade anytime" |

---

## 5. Cost Structure

### Monthly Fixed Costs

| Item | Cost |
|------|------|
| Cloud hosting (VPS) | Rp300K - 800K |
| MySQL (managed or self-hosted) | Rp200K - 500K |
| Domain (stockledger.io) | ~Rp15K/mo |
| SSL | Free (Let's Encrypt) |
| Transactional email | Rp0 - 200K |
| **Total** | **~Rp500K - 1.5M/month** |

### Breakeven

| Goal | Tenants Needed |
|------|---------------|
| Cover hosting | 2-3 (any tier) |
| Cover hosting + dev living cost (Rp15M) | ~30 Starter or ~25 Professional |
| Sustainable (Rp30M/mo) | ~60 mixed tenants |

---

## 6. Pricing Page FAQ

| Question | Answer |
|----------|--------|
| Is there a free plan? | 14-day free trial with full Professional features. Plans start at Rp299K/mo after. |
| Can I switch plans? | Yes, anytime. Changes on next billing cycle. |
| Is there a contract? | No. Monthly = cancel anytime. Annual = 20% discount, billed yearly. |
| What happens if I cancel? | Read-only access for 30 days. Export everything as CSV anytime. |
| Do you charge per transaction? | Never. Flat monthly fee regardless of order volume. |
| Is my data safe? | HTTPS, tenant-isolated, full audit trails, automatic backups. |

---

## 7. Future Revenue Add-Ons (Year 2+)

| Add-On | Price | Description |
|--------|-------|-------------|
| Marketplace API sync | +Rp200K/channel/mo | Direct Tokopedia/Shopee auto-sync |
| Advanced analytics | +Rp100-200K/mo | Custom dashboards, trend forecasting |
| Multi-currency | +Rp100K/mo | Import purchases in CNY/USD |
| Accountant portal | Free | Read-only for external accounting firms (drives referrals) |

---

## Decision Log

| Decision | Why |
|----------|-----|
| Flat tiers, not per-transaction | Users hate per-tx fees — that's why they leave marketplaces |
| 14-day trial, no credit card | Longer trial = more value seen. Low CC penetration in Indonesia. |
| 3 tiers (not 4+) | Cognitively simple. Free tier attracts non-serious users. |
| Professional as trial default | Show full power. Downgrade to Starter is still a paying customer. |
| Annual 20% discount | Meaningful incentive without leaving too much on table. |
| Enterprise license Month 6+ | Each deal = weeks of support time. Validate SaaS first. |
