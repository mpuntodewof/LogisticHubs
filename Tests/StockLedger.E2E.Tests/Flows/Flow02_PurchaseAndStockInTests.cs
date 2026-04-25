using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow02_PurchaseAndStockInTests : StockLedgerTestBase
{
    // ── NOT YET IMPLEMENTED in API ─────────────────────────────────────────
    // Journey 2 ("Stock the Shelves") depends on four subsystems that don't
    // have controllers yet:
    //   - /api/v1/suppliers          (no SuppliersController)
    //   - /api/v1/purchase-orders    (no PurchaseOrdersController)
    //   - /api/v1/goods-receipts     (no GoodsReceiptsController)
    //   - Approval/receiving workflow
    // Until those ship, stock-in is only exercisable via the initial-stock
    // CSV import path (covered by Flow03) and manual stock-movement creation
    // (covered by Flow06). Re-enable this test once Journey 2 is implemented.

    [Fact(Skip = "Purchase Orders / Goods Receipts subsystem not implemented — see Journey 2")]
    public Task Manager_Should_Create_PO_And_Warehouse_Should_Receive_Stock() => Task.CompletedTask;
}
