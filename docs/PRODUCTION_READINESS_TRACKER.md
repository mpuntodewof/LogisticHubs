# StockLedger — Production & Revenue Readiness Tracker

> Living document. Updated as work progresses.
> **Last updated:** 2026-04-25 | **Maintained by:** Henoch Hernanda + Claude

---

## How to use this tracker

- **Status icons:** ⬜ Not started · 🟨 In progress · ✅ Done · ⚠️ Partial / needs verification · ⏸️ Blocked
- **Priority:** P0 = blocks launch · P1 = blocks first paying customer · P2 = blocks scale · P3 = nice-to-have
- **Effort:** S = <3 days · M = 1–2 weeks · L = 2–4 weeks · XL = >4 weeks
- **Updating:** Edit the Status column inline. Add a dated note in the **Progress Log** at the bottom when status changes.
- **Adding items:** Append to the relevant phase. Keep IDs stable (don't renumber).

---

## Snapshot

| Phase | Total | ✅ Done | 🟨 In Progress | ⚠️ Partial | ⬜ Not Started | ⏸️ Blocked |
|-------|------:|------:|------:|------:|------:|------:|
| Phase 1 — Pre-Launch Foundation | 10 | 0 | 0 | 1 | 9 | 0 |
| Phase 2 — First Paying Customer | 12 | 0 | 0 | 0 | 12 | 0 |
| Phase 3 — Post-Launch Hardening | 9 | 0 | 0 | 0 | 9 | 0 |
| Phase 4 — Pre-Scale Hardening | 5 | 0 | 0 | 0 | 5 | 0 |
| Phase 5 — Product-Level Revenue Gaps | 8 | 0 | 0 | 1 | 7 | 0 |
| **Totals** | **44** | **0** | **0** | **2** | **42** | **0** |

---

## Phase 1 — Pre-Launch Foundation

> Must complete before any customer touches the product.

| # | Item | Priority | Effort | Status | Notes |
|---|------|---------|--------|--------|-------|
| 1.1 | Align E2E test TFM to net8.0 | P0 | S | ⬜ | E2E csproj currently targets net9.0 — CI runs on net8.0 |
| 1.2 | Add E2E job to CI workflow | P0 | S | ⬜ | All 8 E2E flows must run on every PR |
| 1.3 | Docker registry push + tag strategy | P0 | M | ⬜ | Current CI builds image then discards it |
| 1.4 | Production deploy pipeline (staging → prod) | P0 | L | ⬜ | One-command release; staging smoke-test gate |
| 1.5 | Run EF migrations in deploy pipeline | P0 | S | ⬜ | Migrations only auto-run in Development today |
| 1.6 | Automated MySQL backups + tested restore | P0 | M | ⬜ | Daily dump, 30-day retention, restore drill required |
| 1.7 | Fix CORS unsafe fallback | P0 | S | ⬜ | Empty `AllowedOrigins` falls back to `AllowAnyOrigin` |
| 1.8 | Move secrets out of env vars | P0 | M | ⬜ | JWT secret + DB creds → Key Vault / Secrets Manager |
| 1.9 | PPN / e-Faktur correctness audit | P0 | M | ⬜ | Sign-off from Indonesian accountant + property tests |
| 1.10 | Resolve 47 uncommitted files on `develop` | P0 | S | ⚠️ | Mid-sprint churn — clarify or stash before reviewing |

**Phase 1 calendar estimate:** 6–10 weeks

---

## Phase 2 — First Paying Customer Enablers

> Run in parallel with Phase 1. None of this exists in code today.

| # | Item | Priority | Effort | Status | Notes |
|---|------|---------|--------|--------|-------|
| 2.1 | Domain model: `Subscription`, `Plan`, `PlanLimit` | P1 | M | ⬜ | New entities + EF migration |
| 2.2 | Trial lifecycle (start/end, read-only post-expiry) | P1 | M | ⬜ | 14-day Pro trial → read-only on day 15 |
| 2.3 | Quota enforcement (SKU/warehouse/import caps) | P1 | M | ⬜ | Returns 402/403 with upgrade hint |
| 2.4 | Payment gateway (Midtrans **or** Xendit) | P1 | L | ⬜ | Sandbox e2e + webhook handling |
| 2.5 | Subscription-invoice generation | P1 | M | ⬜ | Separate from customer-facing PPN invoices |
| 2.6 | Self-serve plan upgrade/downgrade UI | P1 | M | ⬜ | Tenant admin can change tier |
| 2.7 | Dunning flow for failed payments | P1 | S | ⬜ | 3 retries over 7 days, suspend day 8 |
| 2.8 | `IEmailSender` + transactional email provider | P1 | M | ⬜ | Welcome, reset, trial expiry, receipts |
| 2.9 | Trial drip emails (Day 0/1/3/7/12/14) | P1 | S | ⬜ | Depends on 2.8 |
| 2.10 | Cancellation + 30-day read-only + CSV export | P1 | M | ⬜ | Promised in pricing FAQ — must implement |
| 2.11 | Privacy policy, ToS, DPA | P1 | S | ⬜ | UU PDP DPO contact required |
| 2.12 | MRR / churn / failed-payment dashboard (internal) | P1 | M | ⬜ | Internal admin panel |

**Phase 2 calendar estimate:** 8–12 weeks (parallelizable with Phase 1)

---

## Phase 3 — Post-Launch Hardening

> First 1–3 months after the first paying customer.

| # | Item | Priority | Effort | Status | Notes |
|---|------|---------|--------|--------|-------|
| 3.1 | Error tracking (Sentry / App Insights) | P1 | S | ⬜ | 2-hour setup; pays back on first incident |
| 3.2 | OpenTelemetry → Prometheus/Grafana | P2 | M | ⬜ | p50/p95/p99 latency per endpoint |
| 3.3 | Uptime monitor + alerting | P1 | S | ⬜ | Poll `/health/ready`, page on 3 failures |
| 3.4 | EF Core global query filters for `ITenantScoped` | P1 | M | ⬜ | Fail-closed tenant isolation |
| 3.5 | Async CSV import via background job + status endpoint | P2 | L | ⬜ | 10k-row imports without HTTP timeout |
| 3.6 | Document & test idempotency on financial writes | P1 | M | ⬜ | Invoice, payment, journal post — replay safe |
| 3.7 | Refresh-token revocation list | P2 | M | ⬜ | Logout/password-change invalidates tokens |
| 3.8 | Permission constants class | P3 | S | ⬜ | Replace policy string literals |
| 3.9 | Onboarding wizard ("value in <15 min") | P1 | L | ⬜ | Strategy doc Risk #2 |

**Phase 3 calendar estimate:** 6–10 weeks

---

## Phase 4 — Pre-Scale Hardening

> Defer until scale signals appear (~customer 50–100).

| # | Item | Priority | Effort | Status | Notes |
|---|------|---------|--------|--------|-------|
| 4.1 | JWT migration HS256 → RS256 + key rotation | P2 | L | ⬜ | `kid` in token header, grace window |
| 4.2 | Per-tenant rate limiting (replace per-IP) | P2 | M | ⬜ | Partition by `TenantId` once authenticated |
| 4.3 | Read replica + read/write split for reports | P3 | L | ⬜ | Heavy P&L queries off primary |
| 4.4 | Per-tenant DB sharding evaluation | P3 | XL | ⬜ | Decision doc + trigger metric |
| 4.5 | Audit log retention + archival policy | P2 | M | ⬜ | >12mo to cold storage |

**Phase 4 calendar estimate:** 6–10 weeks (deferred)

---

## Phase 5 — Product-Level Revenue Gaps

> P0 features in [PRODUCT_STRATEGY_CANVAS.md](PRODUCT_STRATEGY_CANVAS.md) still marked "To build."

| # | Item | Priority | Effort | Status | Notes |
|---|------|---------|--------|--------|-------|
| 5.1 | Tokopedia CSV parser | P0 | M | ⬜ | Headline value prop |
| 5.2 | Shopee CSV parser | P0 | M | ⬜ | Headline value prop |
| 5.3 | P&L report (basic + per-channel) | P0 | L | ⬜ | What the Owner persona pays for |
| 5.4 | Margin per product / per channel report | P0 | M | ⬜ | Same as 5.3 |
| 5.5 | Balance sheet | P1 | L | ⬜ | Standard format, ties to journals |
| 5.6 | PPN input/output summary for DJP | P1 | M | ⬜ | Monthly DJP-format export |
| 5.7 | Stock reconciliation workflow | P1 | M | ⚠️ | File modified — verify current state |
| 5.8 | Low stock dashboard & alerts | P1 | M | ⬜ | Depends on 2.8 (email sender) |

**Phase 5 calendar estimate:** 10–14 weeks

---

## Dependency Map

```
1.1 ──► 1.2                        (TFM fix unblocks E2E in CI)
1.3 ──► 1.4 ──► 1.5                (image push → deploy → migrations)
1.6 ──► launch gate                (no backups = no launch)
1.9 ──► launch gate                (no PPN audit = legal risk)
2.1 ──► 2.2 ──► 2.3                (subscription → trial → quotas)
2.4 ──► 2.5 ──► 2.7                (gateway → invoicing → dunning)
2.8 ──► 2.9, 3.3, 5.8              (email sender unlocks drip + alerts)
5.1, 5.2 ──► headline value prop   (CSV parsers = the demo)
5.3, 5.4 ──► Owner persona buys    (P&L = why the bill-payer pays)
```

---

## Suggested First Sprint (next 2 weeks)

| Pick | Why first |
|------|-----------|
| 1.1 + 1.2 | One-day fix that makes CI meaningful |
| 1.7 | Trivial security fix |
| 1.10 | Clean working tree before anything else lands |
| 1.3 | Unblocks 1.4 — the longest pole in Phase 1 |
| 3.1 (Sentry) | 2-hour setup, pays back on first incident |
| Start 1.6 (backups) | Long lead time on the restore drill |

---

## Progress Log

> Append a dated entry whenever an item changes status. Newest at top.

| Date | Item | Change | Notes |
|------|------|--------|-------|
| 2026-04-25 | — | Tracker created | Baseline from production-readiness analysis. 42 items not started, 2 partial (1.10, 5.7). |
