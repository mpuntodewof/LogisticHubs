# StockLedger — Demo & Ship Readiness Tracker

> Living document. Updated alongside [PRODUCTION_READINESS_TRACKER.md](PRODUCTION_READINESS_TRACKER.md).
> **Last updated:** 2026-04-28 (5.4 margin report shipped) | **Maintained by:** Henoch Hernanda + Claude

---

## Why this doc exists

The main readiness tracker scores *engineering completeness* across 45 items. That's the right view for planning work, but it answers the wrong question when someone asks **"is the product ready?"** — because "ready" means two different things:

1. **Demo-ready** — can you walk a prospect through the product convincingly?
2. **Ship-ready** — can a paying customer use it daily without you babysitting?

Demo-ready almost always lands first. Ship-ready takes longer because it has to handle the boring 80% of edge cases, billing, observability, and trust signals that don't show up in a 20-minute demo. This doc tracks both bars *separately* so the gap is visible.

> **Out of scope:** deployment, domain, hosting infra, and CI/CD targets. Those live in Phase 1 of the main tracker (1.4–1.6, 1.8). They affect whether the product is *running*, not whether it's *ready*.

---

## Current Score

| Aspect | Score | Last assessed |
|--------|------:|---------------|
| 🎬 **Demo-ready** | **82%** | 2026-04-28 (after 5.4 margin report) |
| 🚢 **Ship-ready (solid product)** | **50%** | 2026-04-28 |

### What "demo-ready" means here

A prospect (Tokopedia/Shopee seller, owner persona) can sit next to you for 20 minutes and see the core value loop work end-to-end on a fresh tenant with sample data, without:

- Visible crashes or stack traces
- "This feature is coming soon" excuses on any of the 5 core journeys' headline screens
- Manual database fixups mid-demo
- Embarrassing UI states (broken layout, placeholder text, untranslated English-only screens where Bahasa is expected)

### What "ship-ready" means here

A paying customer can run their daily/weekly/monthly business on StockLedger **for 30 days** without you touching their tenant, where:

- All P0/P1 items in the main tracker are ✅
- Email + error tracking are live (you find out about bugs before they call you)
- Billing/subscription/quota work
- Onboarding gets a non-technical user from signup to first import in <15 minutes
- Tax/finance numbers are accountant-validated (e-Faktur audit)
- Failure modes return useful errors, not 500s

---

## Per-Journey Scoring

Score = max(weakest sub-step's percentage). One broken step in a journey caps the whole journey, because that's how a demo or a real customer experiences it.

| Journey | Demo % | Ship % | Headline gap |
|---------|------:|------:|--------------|
| 1. Set Up My Store | 90% | 75% | No guided wizard — feels like 8 separate forms |
| 2. Stock the Shelves | 90% | 80% | Auto-AP journal entry from goods receipt is `[Future]` |
| 3. Record My Sales | **88%** | 73% | Auto-mapper hardened with pinned tests for likely real-export shapes; final real-account export pinning still open |
| 4. Close the Books | 75% | 45% | PPN summary + balance sheet not built; e-Faktur not audited (margin-per-product shipped 2026-04-28) |
| 5. Check My Business | 75% | 55% | Action-item recommendations + email alerts not built |

---

## Demo-readiness roadmap — what gets us to 90%

Three items, ~3–4 weeks of focused work. After this, you can pitch friendly customers without flinching.

| Order | Tracker ID | Item | Effort | Demo-impact |
|------:|-----------|------|--------|-------------|
| 1 | 5.1a | Validate parsers against real Tokopedia + Shopee exports | S (3–5 days) | **🟨 Partially done 2026-04-28** — auto-mapper extracted to testable `CsvHeaderAutoMapper` class, precedence bug fixed, 16 pinned tests for likely real-export shapes pass. Remaining: drop in actual seller-account export and add as final pinned cases. |
| 2 | 5.4 | Margin per product / per channel report | M (1–2 wk) | **✅ Shipped 2026-04-28** — `/reports/margin` page with per-product and per-product×channel tabs, worst-margin-first sort, loss-making row highlighting, CSV export. Cost source: variant CostPrice point-in-time (caveat surfaced in UI). 8 use-case tests pin the math. |
| 3 | 5.6 | PPN input/output summary for DJP | M (1–2 wk) | Accountant persona's #1 question. Data exists in invoices + tax rates. Without this, the *finance* product loses credibility in the first finance demo. |

After these three: Demo-ready ≈ **90%**. Remaining 10% gap is polish (Bahasa coverage, action-item recommendations, balance sheet) — none of it blocks a confident demo to early customers.

---

## Ship-readiness roadmap — what gets us to 65%

Demo-readiness items above **plus** the items below. ~6–10 additional weeks on top.

| Order | Tracker ID | Item | Effort | Ship-impact |
|------:|-----------|------|--------|-------------|
| 4 | 1.11 | Onboarding wizard ("value in <15 min") | L (2–3 wk) | Trial-conversion lever. Without this, ~50% of signups don't reach Journey 2. Moved to Phase 1 on 2026-04-28 because it's a launch-blocker, not post-launch hardening. |
| 5 | 1.9 | PPN / e-Faktur correctness audit | M (1–2 wk) | Find an Indonesian accountant for 1 day of review. Selling tax features without sign-off is legal-risk and credibility-risk. Cheap insurance. |
| 6 | 3.1 | Error tracking (Sentry / App Insights) | S (2 hrs) | Free, 2-hour install, lights up the moment a real user hits a bug. Without this, you find out from the customer's WhatsApp message — too late. |
| 7 | 2.8 | `IEmailSender` + transactional email provider | M (1–2 wk) | Unlocks welcome email, password reset, trial expiry, receipts, low-stock alerts. Almost every other Phase 2 item depends on this. |
| 8 | 5.5 | Balance sheet | L (2–3 wk) | Important for accountants but *not* first-demo-critical. Accountants accept "next month" if the rest is solid. |

After items 1–8: Ship-ready ≈ **65%**. The remaining 35% is mostly **billing** (Phase 2 items 2.1–2.10) — which is required before *charging* customers but not required to *use* the product. That's the "first paying customer" bar, separate from "solid product" bar.

---

## What's NOT counted in these scores

These are real work but they're explicitly excluded so the scores answer the product-readiness question, not the infra/business question:

- **Infrastructure / hosting / domain** — items 1.4 (deploy pipeline), 1.5 (migrations in deploy), 1.6 (backups), 1.8 (secrets management). User has explicitly deferred these pending budget.
- **Billing / subscription mechanics** — Phase 2 items 2.1–2.12. Required to charge money; not required for a demo or for early-access friendly customers paying via manual invoice.
- **Pre-scale hardening** — Phase 4 items 4.1–4.5. Defer until ~customer 50–100.

---

## Update protocol

- **When to bump scores:** any time an item in the roadmaps above changes status, or a code audit reveals the actual state diverges from the tracker. Re-score the affected journey(s) and the headline percentages.
- **Where the math comes from:** the per-journey percentages are judgment calls anchored to "what would a prospect/customer notice." Document the reasoning in the Progress Log so the next score-update has a baseline to argue with.
- **Don't game the score.** If the only way to bump the demo % is to delete a journey from scope, update [CORE_BUSINESS_JOURNEYS.md](CORE_BUSINESS_JOURNEYS.md) explicitly — don't quietly redefine "demo-ready."

---

## Progress Log

> Append a dated entry whenever scores change or a major journey state shifts. Newest at top.

| Date | Aspect | Change | Notes |
|------|--------|--------|-------|
| 2026-04-28 | demo +4, ship +3 | 5.4 margin per product shipped | Demo 78% → 82%; Ship 47% → 50%. Journey 4 score 60% → 75% — owner-persona "which SKU loses money on Shopee" question now has a dedicated screen. New `/reports/margin` page (per-product + per-product×channel tabs, worst-margin-first, loss-row highlighting, CSV export). Cost basis is variant CostPrice point-in-time with caveat surfaced. |
| 2026-04-28 | demo +3, ship +2 | 5.1a auto-mapper hardened | Demo 75% → 78%; Ship 45% → 47%. Journey 3 score 85% → 88%. New `CsvHeaderAutoMapper` (Application layer, 16 pinned tests) replaces in-line frontend logic; fixes a precedence bug that silently dropped `"Order ID"` headers. Score ceiling on Journey 3 still capped pending real-account-export pinning. |
| 2026-04-28 | both | Initial scoring | Demo 75% / Ship 45%. Triggered by code audit that found Journey 3 (sales import) is shipped end-to-end (parsers, stock deduction, channel attribution, platform fees) — ahead of where the main tracker had it (5.1/5.2 ⚠️). 5.1/5.2 closed; new item 5.1a created for real-export validation. 3.9 onboarding wizard moved to Phase 1 as 1.11 (launch-blocker). |
