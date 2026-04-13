# NiagaOne ERP — 5 Core Business Journeys

> 📌 **Purpose**: End-to-end reference for how data flows through the NiagaOne platform.
> Each journey maps the complete business process from trigger to outcome.

---

## Table of Contents

- [Journey 1: "Open the Store" — Retail Setup](#journey-1-open-the-store--retail-setup)
- [Journey 2: "Stock the Shelves" — Purchase & Receiving](#journey-2-stock-the-shelves--purchase--receiving)
- [Journey 3a: "Sell at the Counter" — POS Sale](#journey-3a-sell-at-the-counter--pos-sale)
- [Journey 3b: "Sell Online + Ship" — E-Commerce Sale](#journey-3b-sell-online--ship--e-commerce-sale)
- [Journey 4: "Run Marketing" — Promotions & Loyalty](#journey-4-run-marketing--promotions--loyalty)
- [Journey 5: "Manage People" — HRM Operations](#journey-5-manage-people--hrm-operations)
- [System Flow Diagram](#system-flow-diagram)
- [Cross-Journey Data Dependencies](#cross-journey-data-dependencies)
- [Untested Scenarios (Critical Gaps)](#untested-scenarios-critical-gaps)

---

## Journey 1: "Open the Store" — Retail Setup

| Property | Value |
| --- | --- |
| **Actor** | Admin |
| **Trigger** | New tenant onboarded, store ready to launch |
| **Outcome** | Complete catalog, warehouse, branch, tax & payment config |
| **Modules Touched** | Product Catalog, Warehouse, Branch, Tax, Payment Terms, CRM |

### Step-by-Step Flow

| Step | Action | API Endpoint | Result |
| --- | --- | --- | --- |
| 1 | Admin logs in | `POST /api/auth/login` | JWT access token |
| 2 | Create parent categories | `POST /api/categories` | Electronics, Fashion |
| 3 | Create child categories | `POST /api/categories` | Smartphones (→Electronics), T-Shirts (→Fashion) |
| 4 | Create brands | `POST /api/brands` | Samsung, Nike |
| 5 | Create units of measure | `POST /api/units` | Piece, Box |
| 6 | Create products | `POST /api/products` | Samsung Galaxy S24, Nike T-Shirt |
| 7 | Create product variants | `POST /api/products/{id}/variants` | Galaxy 128GB, Galaxy 256GB, Nike Red M, Nike Blue M |
| 8 | Create warehouse | `POST /api/warehouses` | Gudang Utama Jakarta |
| 9 | Create branch | `POST /api/branches` | Jakarta Store (linked to warehouse) |
| 10 | Create delivery zones | `POST /api/delivery-zones` | JABODETABEK, JATIM |
| 11 | Create tax rate | `POST /api/tax-rates` | PPN 11% |
| 12 | Create payment terms | `POST /api/payment-terms` | COD, NET30 |
| 13 | Create customer groups | `POST /api/customer-groups` | Regular (0%), VIP (5% discount) |
| 14 | Verify setup | `GET /api/products?page=1&pageSize=50` | ≥ 2 products visible |

### Entities Created

- 4 Categories (2 parent + 2 child)
- 2 Brands
- 2 Units of Measure
- 2 Products → 4 Variants
- 1 Warehouse
- 1 Branch
- 2 Delivery Zones
- 1 Tax Rate
- 2 Payment Terms
- 2 Customer Groups

### Data Flow Diagram

```
Categories ──┐
Brands ──────┤
Units ───────┼──► Products ──► Variants
             │
Warehouse ───┼──► Branch
             │
Tax Rate ────┤
Payment Terms┤
Cust. Groups─┘
```

> ⚠️ **This journey is the foundation. Every other journey depends on data created here.**

---

## Journey 2: "Stock the Shelves" — Purchase & Receiving

| Property | Value |
| --- | --- |
| **Actors** | Manager, Warehouse Staff |
| **Trigger** | Store needs inventory before it can sell |
| **Outcome** | Products are in stock and available for sale |
| **Modules Touched** | Purchase, Inventory, Warehouse |

### Step-by-Step Flow

| Step | Actor | Action | API Endpoint | Result |
| --- | --- | --- | --- | --- |
| 1 | Manager | Log in | `POST /api/auth/login` | Manager token |
| 2 | Manager | Create supplier | `POST /api/suppliers` | PT Samsung Indonesia |
| 3 | Manager | Create purchase order | `POST /api/purchase-orders` | PO with 2 line items: Galaxy 128GB ×50, Galaxy 256GB ×30 |
| 4 | Manager | Submit PO | `POST /api/purchase-orders/{id}/submit` | Status: Draft → **Submitted** |
| 5 | Manager | Approve PO | `POST /api/purchase-orders/{id}/approve` | Status: Submitted → **Approved** |
| 6 | Manager | Verify PO | `GET /api/purchase-orders/{id}` | Status = "Approved", 2 line items |
| 7 | Warehouse | Log in | `POST /api/auth/login` | Warehouse token |
| 8 | Warehouse | Create goods receipt | `POST /api/goods-receipts` | GR linked to PO, copies line items |
| 9 | Warehouse | Confirm goods receipt | `POST /api/goods-receipts/{id}/confirm` | **Stock created/updated** |
| 10 | Warehouse | Verify stock | `GET /api/warehouse-stock?warehouseId={id}` | Stock records exist |

### What Happens on Goods Receipt Confirm (Step 9)

This is the **critical step** — multiple things happen:

```
For each GR line item:
  ├─ Find or create WarehouseStock record
  ├─ WarehouseStock.QuantityOnHand += received quantity
  ├─ Create StockMovement (type: In, reason: Purchase)
  └─ SaveChangesAsync() ← called per item (not transactional!)

Then:
  ├─ Check if all PO items fully received
  ├─ If yes → PO.Status = "Received"
  └─ GoodsReceipt.Status = "Confirmed"
```

### Status Transitions

```
Purchase Order:  Draft → Submitted → Approved → Received
Goods Receipt:   Draft → Confirmed
```

### Data Flow Diagram

```
Supplier ──► Purchase Order ──► Goods Receipt ──► StockMovement (In)
                  │                                      │
                  │                                      ▼
                  └── PO Items ──────────────► WarehouseStock (+qty)
```

> 🔴 **Known Risk**: Goods receipt confirm executes 5+ `SaveChangesAsync()` calls with NO transaction wrapper. Partial failure = inconsistent stock.

---

## Journey 3a: "Sell at the Counter" — POS Sale

| Property | Value |
| --- | --- |
| **Actors** | Cashier, Accountant |
| **Trigger** | Walk-in customer at physical store |
| **Outcome** | Sale completed, stock deducted, invoice issued, journal posted |
| **Modules Touched** | Sales, Inventory, Finance, Tax |

### Step-by-Step Flow

| Step | Actor | Action | API Endpoint | Result |
| --- | --- | --- | --- | --- |
| 1 | Cashier | Log in | `POST /api/auth/login` | Cashier token |
| 2 | Cashier | Create customer | `POST /api/customers` | Siti Rahayu (Individual) |
| 3 | Cashier | Create POS order | `POST /api/sales-orders` | Order with items, type: POS, status: **Draft** |
| 4 | Cashier | Confirm order | `POST /api/sales-orders/{id}/confirm` | Status: Draft → **Confirmed** ← ⚡ STOCK DEDUCTED HERE |
| 5 | Cashier | Get total | `GET /api/sales-orders/{id}` | grandTotal calculated |
| 6 | Cashier | Add payment | `POST /api/sales-orders/{id}/payments` | Cash payment, status hardcoded to "Paid" |
| 7 | Cashier | Verify order | `GET /api/sales-orders/{id}` | Order with payment attached |
| — | — | *— Handoff to Accountant —* | — | — |
| 8 | Accountant | Log in | `POST /api/auth/login` | Accountant token |
| 9 | Accountant | Create GL accounts | `POST /api/chart-of-accounts` | AR (Asset), Revenue, Tax Payable (Liability) |
| 10 | Accountant | Create invoice | `POST /api/invoices` | Invoice from sales order, status: **Draft** |
| 11 | Accountant | Issue invoice | `POST /api/invoices/{id}/issue` | Status: Draft → **Issued** |
| 12 | Accountant | Assign tax number | `PUT /api/invoices/{id}/tax-number` | e-Faktur number assigned |
| 13 | Accountant | Mark paid | `POST /api/invoices/{id}/mark-paid` | Status: Issued → **Paid** |
| 14 | Accountant | Create journal entry | `POST /api/journal-entries` | Balanced entry (debit = credit) |
| 15 | Accountant | Post journal | `POST /api/journal-entries/{id}/post` | Status: Draft → **Posted** |
| 16 | Accountant | Verify balance | `GET /api/journal-entries/{id}` | totalDebit == totalCredit |

### What Happens on Order Confirm (Step 4)

This is the **most dangerous step** in the entire system:

```
For each order item:
  ├─ Find WarehouseStock by warehouse + variant
  ├─ if (stock.QuantityOnHand < item.Quantity) → throw "Insufficient stock"
  ├─ stock.QuantityOnHand -= item.Quantity
  ├─ Create StockMovement (type: Out, reason: Sale)
  ├─ SaveChangesAsync() ← per item, in a loop!
  └─ SaveChangesAsync() ← for movement record

Then:
  └─ order.Status = "Confirmed"
  └─ SaveChangesAsync() ← final save
```

### Sample Order Calculation

```
2× Galaxy 128GB   @ Rp 12,999,000  =  Rp 25,998,000
1× Nike Red M     @ Rp    299,000  =  Rp    299,000
                         SubTotal   =  Rp 26,297,000
                         Tax (11%)  =  Rp  2,892,670
                         GrandTotal =  Rp 29,189,670
```

### Status Transitions

```
Sales Order:   Draft → Confirmed → (Cancelled)
Payment:       Created with status "Paid" (hardcoded)
Invoice:       Draft → Issued → Paid (or Cancelled)
Journal Entry: Draft → Posted (or Voided)
```

### Data Flow Diagram

```
Customer ──► Sales Order ──► Confirm ──► StockMovement (Out)
                  │              │              │
                  │              │              ▼
                  │              └────► WarehouseStock (-qty)
                  │
                  ├──► SalesOrderPayment (Cash)
                  │
                  ├──► Invoice ──► Issue ──► Mark Paid
                  │         │
                  │         └──► InvoiceItems (tax recalculated)
                  │
                  └──► JournalEntry ──► Post
                            │
                            ├─ Debit:  AR          10,000,000
                            ├─ Credit: Revenue      9,009,009
                            └─ Credit: Tax Payable    990,991
```

> 🔴 **Known Risks**:
> - Stock deduction loop: multiple `SaveChangesAsync()` with no transaction
> - Payment status hardcoded to "Paid" — no real gateway validation
> - Invoice tax recalculated independently from order tax (potential mismatch)
> - Journal entry created manually — no automatic generation from invoice

---

## Journey 3b: "Sell Online + Ship" — E-Commerce Sale

| Property | Value |
| --- | --- |
| **Actors** | Manager, Driver |
| **Trigger** | Online order from e-commerce storefront |
| **Outcome** | Sale completed, shipped, tracked, delivered |
| **Modules Touched** | Sales, Inventory, Logistics |

### Step-by-Step Flow

| Step | Actor | Action | API Endpoint | Result |
| --- | --- | --- | --- | --- |
| 1 | Manager | Log in | `POST /api/auth/login` | Manager token |
| 2 | Manager | Create company customer | `POST /api/customers` | PT Maju Bersama (Company) |
| 3 | Manager | Create online order | `POST /api/sales-orders` | Order type: Online, status: **Draft** |
| 4 | Manager | Confirm order | `POST /api/sales-orders/{id}/confirm` | Status → **Confirmed**, stock deducted |
| 5 | Manager | Get total | `GET /api/sales-orders/{id}` | grandTotal calculated |
| 6 | Manager | Add payment | `POST /api/sales-orders/{id}/payments` | Bank Transfer payment |
| 7 | Manager | Create shipment | `POST /api/shipments` | Shipment linked to warehouse, courier: JNE |
| 8 | System | Tracking: PickedUp | `POST /api/shipments/{id}/tracking-events` | eventDate: 2 hours ago |
| 9 | System | Tracking: InTransit | `POST /api/shipments/{id}/tracking-events` | eventDate: 1 hour ago |
| 10 | System | Tracking: Delivered | `POST /api/shipments/{id}/tracking-events` | eventDate: now |
| 11 | System | Verify delivery | `GET /api/shipments/{id}` | Status = **"Delivered"** |
| — | — | *— Driver operational notes —* | — | — |
| 12 | Driver | Log in | `POST /api/auth/login` | Driver token |
| 13 | Driver | Add delivery note | `POST /api/shipments/{id}/notes` | Instruction/observation note |

### What Happens on Tracking Event (Steps 8-10)

```
Create ShipmentTracking record
  ├─ Id = new Guid
  ├─ ShipmentId, Status, Location, Description, EventDate
  ├─ SaveChangesAsync() ← tracking record saved
  │
  Then:
  ├─ shipment.Status = tracking.Status (synced to latest event)
  └─ SaveChangesAsync() ← shipment status updated separately
```

### Shipment Status Transitions

```
Pending → Assigned → PickedUp → InTransit → OutForDelivery → Delivered
                                                              (or Failed / Cancelled)
```

### Data Flow Diagram

```
Customer ──► Sales Order ──► Confirm ──► StockMovement (Out)
                  │              │              │
                  │              │              ▼
                  │              └────► WarehouseStock (-qty)
                  │
                  ├──► SalesOrderPayment (BankTransfer)
                  │
                  └──► Shipment (JNE) ──► TrackingEvent: PickedUp
                            │             TrackingEvent: InTransit
                            │             TrackingEvent: Delivered
                            │                    │
                            │                    ▼
                            │             Shipment.Status = "Delivered"
                            │
                            └──► ShipmentNote (driver notes)
```

> 🔴 **Known Risks**:
> - Tracking events NOT idempotent — duplicate webhooks create duplicate records
> - Tracking save + shipment status update = 2 separate `SaveChangesAsync()`
> - Shipment status does NOT sync back to Sales Order status
> - No state machine validation — can skip states (e.g., Pending → Delivered)

---

## Journey 4: "Run Marketing" — Promotions & Loyalty

| Property | Value |
| --- | --- |
| **Actor** | Marketing |
| **Trigger** | Campaign planning, customer retention strategy |
| **Outcome** | Active promotions, coupons available, loyalty program running |
| **Modules Touched** | Promotions, Loyalty, CRM |

### Step-by-Step Flow

| Step | Action | API Endpoint | Result |
| --- | --- | --- | --- |
| 1 | Log in as Marketing | `POST /api/auth/login` | Marketing token |
| 2 | Create promotion | `POST /api/promotions` | "Flash Sale Electronics" — 20% discount, 7 days |
| 3 | Add promotion rule | `POST /api/promotions/{id}/rules` | Rule: SpecificCategories → Electronics |
| 4 | Activate promotion | `POST /api/promotions/{id}/activate` | Status: Draft → **Active** |
| 5 | Create coupon | `POST /api/coupons` | Code: WELCOME-E2E, 10% off, min Rp 100k, cap 100 uses |
| 6 | Create loyalty program | `POST /api/loyalty-programs` | "NiagaOne Rewards" — 0.001 points per IDR spent |
| 7 | Create Bronze tier | `POST /api/loyalty-programs/{id}/tiers` | Multiplier: 1.0× |
| 8 | Create Silver tier | `POST /api/loyalty-programs/{id}/tiers` | Multiplier: 1.5×, requires 1,000 pts |
| 9 | Create Gold tier | `POST /api/loyalty-programs/{id}/tiers` | Multiplier: 2.0×, requires 5,000 pts |
| 10 | Enroll customer | `POST /api/loyalty-programs/{id}/members` | Customer enrolled in program |

### Promotion Rule Engine

```
Promotion
  ├─ Type: BuyXGetY | BundleDiscount | FlashSale | PercentageDiscount
  ├─ Rules
  │   ├─ RuleType: MinimumPurchase | SpecificCategories | SpecificProducts
  │   └─ Conditions + Actions (JSON-based)
  ├─ Products (direct product linkage)
  └─ Usage tracking (per customer, per promotion)
```

### Loyalty Point System

```
Loyalty Program
  ├─ PointsPerCurrencyUnit: 0.001 (1 point per Rp 1,000)
  ├─ Tiers
  │   ├─ Bronze (1.0×) — entry level
  │   ├─ Silver (1.5×) — 1,000+ points
  │   └─ Gold   (2.0×) — 5,000+ points
  └─ Memberships
      └─ Customer → Tier assignment → Point balance
```

> ⚠️ **Gap**: Promotions, coupons, and loyalty points are **created** but never **applied to a real checkout** in the E2E tests. The connection between marketing entities and the sales order flow is untested.

---

## Journey 5: "Manage People" — HRM Operations

| Property | Value |
| --- | --- |
| **Actor** | HR Manager |
| **Trigger** | Daily workforce management |
| **Outcome** | Departments structured, employee tracked, attendance logged, leave approved |
| **Modules Touched** | HRM |

### Step-by-Step Flow

| Step | Action | API Endpoint | Result |
| --- | --- | --- | --- |
| 1 | Log in as HR | `POST /api/auth/login` | HR token |
| 2 | Create departments | `POST /api/departments` | Engineering, Sales, Operations |
| 3 | Create employee | `POST /api/employees` | Software Engineer, salary Rp 15M |
| 4 | Clock in | `POST /api/attendance/clock-in` | Attendance record created |
| 5 | *(wait)* | — | Simulates work time |
| 6 | Clock out | `POST /api/attendance/{id}/clock-out` | Duration captured |
| 7 | Create leave request | `POST /api/leave-requests` | Annual leave, 2 days |
| 8 | Approve leave | `POST /api/leave-requests/{id}/approve` | Status: Pending → **Approved** |
| 9 | Verify | `GET /api/leave-requests/{id}` | Status = "Approved" |

### Status Transitions

```
Leave Request:  Pending → Approved (or Rejected)
Employee:       Active | OnLeave | Terminated | Probation
Attendance:     ClockIn → ClockOut (with duration)
```

### Data Flow Diagram

```
Department ──► Employee ──► Attendance (clock in/out)
                   │
                   └──► Leave Request ──► Approve/Reject
```

> ℹ️ **Standalone module** — no data dependency on other journeys.

---

## System Flow Diagram

How all 5 journeys connect into one system:

```
┌──────────────────────────────────────────────────────────────────────┐
│                                                                      │
│   JOURNEY 1: "Open the Store"                                        │
│   ┌─────────────────────────────────────────────────────────┐        │
│   │ Categories → Brands → Products → Variants               │        │
│   │ Warehouse → Branch → Zones → Tax → Payment Terms        │        │
│   └──────────────────────┬──────────────────────────────────┘        │
│                          │ (base data)                               │
│            ┌─────────────┼─────────────┐                             │
│            ▼             ▼             ▼                              │
│   ┌────────────┐  ┌───────────┐  ┌───────────┐                      │
│   │ JOURNEY 2  │  │JOURNEY 3a │  │JOURNEY 3b │                      │
│   │ "Stock"    │  │ "POS"     │  │ "Online"  │                      │
│   │            │  │           │  │           │                      │
│   │ Supplier   │  │ Customer  │  │ Customer  │                      │
│   │ → PO       │  │ → Order   │  │ → Order   │                      │
│   │ → GR       │  │ → Confirm │  │ → Confirm │                      │
│   │ → Stock IN │  │ → Pay     │  │ → Pay     │                      │
│   └─────┬──────┘  │ → Invoice │  │ → Ship    │                      │
│         │         │ → Journal │  │ → Track   │                      │
│         │         └─────┬─────┘  │ → Deliver │                      │
│         │               │        └─────┬─────┘                      │
│         │               │              │                             │
│         ▼               ▼              ▼                             │
│   ┌─────────────────────────────────────────────┐                    │
│   │              INVENTORY (shared)              │                    │
│   │                                              │                    │
│   │  Stock IN (from Purchase)                    │                    │
│   │  Stock OUT (from Sales — POS or Online)      │                    │
│   │  WarehouseStock = running balance            │                    │
│   │  StockMovement = immutable audit ledger      │                    │
│   └─────────────────────────────────────────────┘                    │
│                                                                      │
│   ┌────────────┐  ┌────────────┐                                     │
│   │ JOURNEY 4  │  │ JOURNEY 5  │                                     │
│   │ "Marketing"│  │ "HRM"      │                                     │
│   │            │  │            │    ← These two are independent      │
│   │ Promos     │  │ Employees  │    ← Not connected to sales flow    │
│   │ Coupons    │  │ Attendance │       in current implementation     │
│   │ Loyalty    │  │ Leaves     │                                     │
│   └────────────┘  └────────────┘                                     │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Cross-Journey Data Dependencies

| Source Journey | Data Created | Used By Journey | Specific Fields |
| --- | --- | --- | --- |
| Journey 1 | Product Variants | Journey 2, 3a, 3b | `galaxy128VariantId`, `galaxy256VariantId`, `nikeRedMVariantId` |
| Journey 1 | Warehouse | Journey 2, 3a, 3b, 4 | `warehouseId` |
| Journey 1 | Branch | Journey 3a, 3b | `branchId` |
| Journey 1 | Categories | Journey 4 | `electronicsCategoryId` |
| Journey 2 | Warehouse Stock | Journey 3a, 3b | Stock must exist before sales can confirm |
| Journey 3a | Sales Order | Journey 3a (Finance) | `salesOrderId` → used to create Invoice |
| Journey 3a | Customer | Journey 4 | `customerId` → enrolled in loyalty program |
| Journey 3b | Shipment | Journey 3b (Driver) | `shipmentId` → driver adds notes & tracking |

---

## Untested Scenarios (Critical Gaps)

> These are real business scenarios that are **not covered** by current E2E tests.

| # | Scenario | Business Impact | Related Journey |
| --- | --- | --- | --- |
| 1 | Order cancellation + stock reversal | Does stock come back correctly? | Journey 3a/3b |
| 2 | Partial delivery (7 of 10 items shipped) | How to handle backorders? | Journey 3b |
| 3 | Inter-warehouse stock transfer | Branch A overstocked → Branch B | Journey 2 |
| 4 | Coupon applied at checkout | Does discount calculate correctly? | Journey 4 → 3a |
| 5 | Loyalty points earned from purchase | Point accrual after payment? | Journey 4 → 3a |
| 6 | Concurrent sales depleting same stock | Two cashiers, one item left | Journey 3a |
| 7 | Payment gateway callback (webhook) | External charge → system update | Journey 3a/3b |
| 8 | Goods receipt with quantity mismatch | PO: 50, received: 40 | Journey 2 |
| 9 | Invoice tax ≠ order tax | Compliance risk with DJP | Journey 3a (Finance) |
| 10 | Shipment "Delivered" → Order still "Confirmed" | No status sync between modules | Journey 3b |

---

## Additional Security Flows (Tested)

### Permission Enforcement (Flow09)

| Test | Actor | Action | Expected |
| --- | --- | --- | --- |
| Cashier cannot access GL | Cashier | `GET /api/chart-of-accounts` | **403 Forbidden** |
| Driver cannot create products | Driver | `POST /api/products` | **403 Forbidden** |
| Accountant cannot manage employees | Accountant | `POST /api/employees` | **403 Forbidden** |
| Unauthenticated request | None | `GET /api/products` | **401 Unauthorized** |
| Viewer cannot delete orders | Viewer | `DELETE /api/sales-orders/{id}` | **403 Forbidden** |

### Multi-Tenant Isolation (Flow10)

| Step | Action | Expected |
| --- | --- | --- |
| 1 | Register Tenant B | New tenant created |
| 2 | Create product in Tenant B | Product visible to Tenant B |
| 3 | Search from Tenant A | **Tenant B product NOT visible** (totalCount = 0) |

---

> 📝 **Last Updated**: 2026-04-03
> 🔗 **Related Docs**: [ERP_FEATURE_BREAKDOWN.md](ERP_FEATURE_BREAKDOWN.md) · [ERP_MODULES_NOTION.md](ERP_MODULES_NOTION.md) · [ERP_SYSTEM_DESIGN.md](ERP_SYSTEM_DESIGN.md)
