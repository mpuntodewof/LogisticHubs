# StockLedger Testing Scenarios

## Test Coverage Summary

| Layer | Project | Tests | Status |
|-------|---------|-------|--------|
| Domain entities | StockLedger.Unit.Tests/Domain/ | 15 | Pass |
| Use cases (unit) | StockLedger.Unit.Tests/UseCases/ | 37 | Pass |
| E2E integration | StockLedger.E2E.Tests/Flows/ | 4 flows | Requires running API |
| **Total unit tests** | | **52** | **All passing** |

---

## Unit Tests — Domain Layer

### InvoiceTests (9 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | Issue_FromDraft_SetsStatusAndTimestamp | Draft -> Issued transition works |
| 2 | Issue_FromPaid_ThrowsInvalidOperation | Cannot issue a paid invoice |
| 3 | MarkPaid_FromIssued_SetsStatusAndTimestamp | Issued -> Paid transition works |
| 4 | MarkPaid_FromDraft_ThrowsInvalidOperation | Cannot pay a draft invoice |
| 5 | Cancel_FromDraft_Succeeds | Draft -> Cancelled with reason |
| 6 | Cancel_FromIssued_Succeeds | Issued -> Cancelled allowed |
| 7 | Cancel_FromPaid_ThrowsInvalidOperation | Cannot cancel a paid invoice |
| 8 | EnsureCanDelete_Draft_DoesNotThrow | Only drafts are deletable |
| 9 | EnsureCanDelete_Issued_Throws | Cannot delete issued invoice |

### JournalEntryTests (6 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | Post_FromDraft_SetsStatusAndTimestamp | Draft -> Posted transition |
| 2 | Post_FromPosted_Throws | Cannot double-post |
| 3 | Void_FromPosted_SetsStatusAndReason | Posted -> Voided with reason |
| 4 | Void_FromDraft_Throws | Cannot void a draft |
| 5 | EnsureCanDelete_Draft_DoesNotThrow | Drafts are deletable |
| 6 | EnsureCanDelete_Posted_Throws | Cannot delete posted entries |

---

## Unit Tests — Use Cases

### AuthUseCaseTests (6 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | LoginAsync_ValidCredentials_ReturnsLoginResponse | Happy path login |
| 2 | LoginAsync_InvalidPassword_ThrowsUnauthorized | Wrong password rejected |
| 3 | LoginAsync_NonexistentUser_StillCallsVerify_PreventsTimingAttack | BCrypt always runs (timing attack fix) |
| 4 | LoginAsync_DisabledAccount_ThrowsUnauthorized | Inactive accounts blocked |
| 5 | RegisterAsync_NewTenant_CreatesAdminRole | New company = Admin role |
| 6 | RegisterAsync_DuplicateEmail_ThrowsConflict | Email uniqueness enforced |

### StockMovementUseCaseTests (5 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | CreateMovementAsync_StockIn_IncreasesQuantity | Stock-in adds to inventory |
| 2 | CreateMovementAsync_StockOut_DecreasesQuantity | Stock-out deducts correctly |
| 3 | CreateMovementAsync_InsufficientStock_ThrowsInvalidOperation | Cannot sell more than available |
| 4 | CreateMovementAsync_NewStock_CreatesWarehouseStock | First movement creates stock record |
| 5 | CreateTransferAsync_SameWarehouse_ThrowsInvalidOperation | Transfer requires different warehouses |

### CsvParserServiceTests (7 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | ParseAsync_SimpleCsv_ReturnsRows | Basic CSV parsing |
| 2 | ParseAsync_QuotedFields_HandlesCorrectly | Commas inside quotes |
| 3 | ParseAsync_EmptyCsv_ReturnsEmpty | Empty file handling |
| 4 | ParseAsync_HeaderOnly_ReturnsEmpty | Header-only file |
| 5 | GetHeaders_ReturnsTrimmedHeaders | Whitespace trimming |
| 6 | ParseAsync_CaseInsensitiveLookup | Case-insensitive column access |
| 7 | ParseAsync_TokopediaFormat_Works | Real Tokopedia export format |

### CsvImportUseCaseTests (7 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | ProcessImportAsync_ValidCsv_DeductsStockAndCreatesMovements | Happy path: 2 rows, stock deducted, movements created |
| 2 | ProcessImportAsync_UnmatchedSku_MarksRowAsUnmatched | Unknown SKU handled gracefully |
| 3 | ProcessImportAsync_InsufficientStock_MarksRowAsError | Low stock marked as error, not exception |
| 4 | ProcessImportAsync_EmptyCsv_ThrowsInvalidOperation | Empty file rejected |
| 5 | ProcessImportAsync_DuplicateOrder_SkipsRow | Duplicate detection works |
| 6 | CreateChannelAsync_DuplicateSlug_ThrowsInvalidOperation | Channel name uniqueness |
| 7 | CreateChannelAsync_NewChannel_Succeeds | Channel creation happy path |

### StockReconciliationUseCaseTests (6 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | ReconcileAsync_MatchingCounts_NoAdjustments | No variance = no movements |
| 2 | ReconcileAsync_OverCount_CreatesAdjustmentMovement | Physical > system creates positive adjustment |
| 3 | ReconcileAsync_UnderCount_CreatesAdjustmentMovement | Physical < system creates negative adjustment |
| 4 | ReconcileAsync_NewStock_CreatesWarehouseStock | New stock record from reconciliation |
| 5 | ReconcileAsync_EmptyRequest_ThrowsInvalidOperation | Empty count list rejected |
| 6 | ReconcileAsync_MultipleItems_ProcessesAll | Batch processing with mixed results |

### AutoJournalEntryUseCaseTests (6 tests)
| # | Test | Validates |
|---|------|-----------|
| 1 | CreateForInvoicePaymentAsync_PaidInvoice_CreatesBalancedEntry | Debit Cash = Credit AR = GrandTotal |
| 2 | CreateForInvoicePaymentAsync_UnpaidInvoice_ReturnsNull | No entry for unpaid invoices |
| 3 | CreateForInvoicePaymentAsync_ZeroAmount_ReturnsNull | No entry for zero-amount invoices |
| 4 | CreateForInvoicePaymentAsync_MissingAccounts_ReturnsNull | Graceful handling when CoA not set up |
| 5 | CreateForSalesBatchAsync_WithFees_CreatesThreeLineEntry | AR + Fee debit = Revenue credit (balanced) |
| 6 | CreateForSalesBatchAsync_ZeroRevenue_ReturnsNull | No entry for zero revenue |

---

## E2E Test Scenarios (Integration)

These require a running API + database. Organized by business journey.

### Flow 03: Record Sales (CSV Import Journey)
| # | Step | Expected |
|---|------|----------|
| 1 | Create sales channel "Tokopedia" with 5% fee | 201 Created |
| 2 | Create sales channel "Shopee" with 8% fee | 201 Created |
| 3 | Upload CSV preview with Tokopedia format | Returns column headers |
| 4 | Process import with valid column mapping | Returns summary with success count |
| 5 | Verify stock movements created (Type=Out, Reason=Sale) | Movements exist with correct quantities |
| 6 | Verify warehouse stock decremented | QuantityOnHand reduced |
| 7 | Re-upload same CSV | Duplicates detected, rows skipped |
| 8 | Upload CSV with unmatched SKUs | Rows marked as Unmatched |
| 9 | Upload CSV with insufficient stock | Rows marked as Error |
| 10 | GET import batches | Returns history with all batches |

### Flow 04: Stock Reconciliation
| # | Step | Expected |
|---|------|----------|
| 1 | Create warehouse + stock some products | Stock exists |
| 2 | POST reconcile with physical counts matching system | MatchedItems = N, AdjustedItems = 0 |
| 3 | POST reconcile with discrepancy (over-count) | Adjustment movement created, stock updated |
| 4 | POST reconcile with discrepancy (under-count) | Adjustment movement created, stock reduced |
| 5 | Verify stock movements have Reason=StockTake | Correct movement type |

### Flow 06: Manual Sales
| # | Step | Expected |
|---|------|----------|
| 1 | POST manual-sale with valid product and quantity | 201, stock decremented |
| 2 | POST manual-sale with insufficient stock | 409 Conflict |
| 3 | Verify movement created with Type=Out, Reason=Sale | Movement exists |
| 4 | Verify ReferenceDocumentType is "Manual-Sale" | Correct reference type |

### Flow 07: Financial Reports
| # | Step | Expected |
|---|------|----------|
| 1 | Create chart of accounts (Revenue, Expense, Asset) | Accounts exist |
| 2 | Create and post journal entries | Entries posted |
| 3 | GET profit-and-loss with date range | Report with totals |
| 4 | Verify revenue/expense breakdown by account | Accounts appear in report |
| 5 | Import CSV sales → GET P&L | Channel breakdown shows imported data |

### Flow 08: Dashboard
| # | Step | Expected |
|---|------|----------|
| 1 | GET dashboard (empty state) | All zeroes, no errors |
| 2 | Create products, stock, make sales | Data seeded |
| 3 | GET dashboard | Stock health, sales, finance all populated |
| 4 | Set reorder points, deplete stock | Low stock alerts appear |
| 5 | Create overdue invoices | Finance.OverdueInvoices > 0 |

### Flow 09: Export
| # | Step | Expected |
|---|------|----------|
| 1 | GET export/invoices | Returns CSV with invoice data |
| 2 | GET export/journal-entries | Returns CSV |
| 3 | GET export/stock-movements | Returns CSV |
| 4 | GET export/products | Returns CSV |
| 5 | Verify CSV content has correct headers | First line matches expected columns |

### Flow 11: Auto Journal Entries
| # | Step | Expected |
|---|------|----------|
| 1 | Create invoice + mark as paid | Invoice status = Paid |
| 2 | Trigger auto journal entry for payment | Entry created with balanced debits/credits |
| 3 | Verify entry references invoice | ReferenceDocumentId = invoice ID |
| 4 | Import CSV batch | Batch completed |
| 5 | Trigger auto journal for batch | Entry with channel fees as separate line |

### Flow 12: Notifications
| # | Step | Expected |
|---|------|----------|
| 1 | Set reorder points on stock items | Configured |
| 2 | Deplete stock below reorder point | Stock updated |
| 3 | GET notifications | Low stock warnings appear |
| 4 | Deplete stock to zero | Out of stock critical alert appears |
| 5 | Create overdue invoice | Overdue invoice notification appears |

---

## Security Test Scenarios

| # | Scenario | Expected |
|---|----------|----------|
| 1 | Login with wrong password 6 times in 1 minute | 429 Too Many Requests (rate limit) |
| 2 | Access API without JWT | 401 Unauthorized |
| 3 | Access endpoint without required permission | 403 Forbidden |
| 4 | Send X-Tenant-Id header mismatching JWT tenant_id | 403 "Tenant mismatch" |
| 5 | Register with weak password "12345678" | 400 validation error (missing uppercase, special char) |
| 6 | Register with duplicate email in same tenant | 409 Conflict |
| 7 | Try to delete posted journal entry | 409 "Only draft entries can be deleted" |
| 8 | Try to modify issued invoice | 409 "Only draft invoices can be modified" |

---

## Multi-Tenancy Test Scenarios

| # | Scenario | Expected |
|---|----------|----------|
| 1 | Create data in Tenant A | Data exists |
| 2 | Login as Tenant B user | JWT has different tenant_id |
| 3 | GET products (Tenant B) | Tenant A's products NOT visible |
| 4 | GET invoices (Tenant B) | Tenant A's invoices NOT visible |
| 5 | GET stock movements (Tenant B) | Tenant A's movements NOT visible |
| 6 | GET import batches (Tenant B) | Tenant A's imports NOT visible |

---

## Performance Test Scenarios

| # | Scenario | Threshold |
|---|----------|-----------|
| 1 | Import 1000-row CSV | Completes within 30 seconds |
| 2 | GET dashboard with 10K stock records | Response < 2 seconds |
| 3 | GET P&L with 1 year of journal entries | Response < 5 seconds |
| 4 | Concurrent stock movements (10 parallel) | No lost updates (optimistic concurrency) |
| 5 | Pagination with pageSize=100 | Returns correct count, no OOM |
