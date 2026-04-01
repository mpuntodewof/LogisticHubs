using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Purchase
{
    public class GoodsReceiptUseCase
    {
        private readonly IGoodsReceiptRepository _goodsReceiptRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;
        private readonly IStockMovementRepository _stockMovementRepository;

        public GoodsReceiptUseCase(
            IGoodsReceiptRepository goodsReceiptRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IWarehouseStockRepository warehouseStockRepository,
            IStockMovementRepository stockMovementRepository)
        {
            _goodsReceiptRepository = goodsReceiptRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _stockMovementRepository = stockMovementRepository;
        }

        public async Task<PagedResult<GoodsReceiptDto>> GetPagedAsync(PagedRequest request, Guid? purchaseOrderId = null)
        {
            var result = await _goodsReceiptRepository.GetPagedAsync(request, purchaseOrderId);

            return new PagedResult<GoodsReceiptDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<GoodsReceiptDetailDto?> GetByIdAsync(Guid id)
        {
            var receipt = await _goodsReceiptRepository.GetDetailByIdAsync(id);
            return receipt == null ? null : MapToDetailDto(receipt);
        }

        public async Task<GoodsReceiptDto> CreateAsync(CreateGoodsReceiptRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Goods receipt must have at least one item.");

            var po = await _purchaseOrderRepository.GetDetailByIdAsync(request.PurchaseOrderId)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (po.Status != "Approved")
                throw new InvalidOperationException("Goods receipts can only be created for approved purchase orders.");

            var receiptNumber = await GenerateReceiptNumberAsync();

            var receipt = new GoodsReceipt
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = receiptNumber,
                PurchaseOrderId = po.Id,
                WarehouseId = po.WarehouseId,
                Status = "Draft",
                ReceivedDate = DateTime.UtcNow,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var items = new List<GoodsReceiptItem>();

            foreach (var itemRequest in request.Items)
            {
                var poItem = po.Items.FirstOrDefault(i => i.Id == itemRequest.PurchaseOrderItemId)
                    ?? throw new InvalidOperationException($"Purchase order item with ID '{itemRequest.PurchaseOrderItemId}' not found.");

                var item = new GoodsReceiptItem
                {
                    Id = Guid.NewGuid(),
                    GoodsReceiptId = receipt.Id,
                    PurchaseOrderItemId = poItem.Id,
                    ProductVariantId = poItem.ProductVariantId,
                    QuantityReceived = itemRequest.QuantityReceived,
                    QuantityRejected = itemRequest.QuantityRejected ?? 0,
                    Notes = itemRequest.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                items.Add(item);
            }

            receipt.Items = items;

            var created = await _goodsReceiptRepository.CreateAsync(receipt);
            return MapToDto(created);
        }

        public async Task ConfirmAsync(Guid id)
        {
            var receipt = await _goodsReceiptRepository.GetDetailByIdAsync(id)
                ?? throw new InvalidOperationException("Goods receipt not found.");

            if (receipt.Status != "Draft")
                throw new InvalidOperationException("Only draft goods receipts can be confirmed.");

            receipt.Status = "Confirmed";
            receipt.UpdatedAt = DateTime.UtcNow;

            var warehouseId = receipt.WarehouseId;

            foreach (var item in receipt.Items)
            {
                // Get or create warehouse stock
                var stock = await _warehouseStockRepository.GetByWarehouseAndVariantAsync(warehouseId, item.ProductVariantId);
                bool isNewStock = stock == null;

                if (isNewStock)
                {
                    stock = new WarehouseStock
                    {
                        Id = Guid.NewGuid(),
                        WarehouseId = warehouseId,
                        ProductVariantId = item.ProductVariantId,
                        QuantityOnHand = 0,
                        QuantityReserved = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                var quantityBefore = stock!.QuantityOnHand;
                stock.QuantityOnHand += item.QuantityReceived;

                var movement = new StockMovement
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = warehouseId,
                    ProductVariantId = item.ProductVariantId,
                    MovementType = StockMovementType.In.ToString(),
                    Reason = StockMovementReason.Purchase.ToString(),
                    Quantity = item.QuantityReceived,
                    QuantityBefore = quantityBefore,
                    QuantityAfter = stock.QuantityOnHand,
                    ReferenceDocumentType = "GoodsReceipt",
                    ReferenceDocumentId = receipt.Id,
                    ReferenceDocumentNumber = receipt.ReceiptNumber,
                    CreatedAt = DateTime.UtcNow
                };

                if (isNewStock)
                    await _warehouseStockRepository.CreateAsync(stock);
                else
                    await _warehouseStockRepository.UpdateAsync(stock);

                await _stockMovementRepository.CreateAsync(movement);

                // Update PO item received quantity
                if (item.PurchaseOrderItem != null)
                {
                    item.PurchaseOrderItem.ReceivedQuantity += item.QuantityReceived;
                }
            }

            // Check if all PO items are fully received
            var po = await _purchaseOrderRepository.GetDetailByIdAsync(receipt.PurchaseOrderId);
            if (po != null)
            {
                var allReceived = po.Items.All(i => i.ReceivedQuantity >= i.Quantity);
                if (allReceived)
                {
                    po.Status = "Received";
                    po.UpdatedAt = DateTime.UtcNow;
                    await _purchaseOrderRepository.UpdateAsync(po);
                }
            }

            await _goodsReceiptRepository.UpdateAsync(receipt);
        }

        public async Task DeleteAsync(Guid id)
        {
            var receipt = await _goodsReceiptRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Goods receipt not found.");

            if (receipt.Status != "Draft")
                throw new InvalidOperationException("Only draft goods receipts can be deleted.");

            await _goodsReceiptRepository.DeleteAsync(receipt);
        }

        private async Task<string> GenerateReceiptNumberAsync()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string receiptNumber;

            do
            {
                var suffix = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                receiptNumber = $"GR-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            }
            while (await _goodsReceiptRepository.ReceiptNumberExistsAsync(receiptNumber));

            return receiptNumber;
        }

        private static GoodsReceiptDto MapToDto(GoodsReceipt r) => new()
        {
            Id = r.Id,
            ReceiptNumber = r.ReceiptNumber,
            PurchaseOrderId = r.PurchaseOrderId,
            PoNumber = r.PurchaseOrder?.PoNumber,
            WarehouseId = r.WarehouseId,
            WarehouseName = r.Warehouse?.Name,
            Status = r.Status,
            ReceivedDate = r.ReceivedDate,
            CreatedAt = r.CreatedAt
        };

        private static GoodsReceiptDetailDto MapToDetailDto(GoodsReceipt r) => new()
        {
            Id = r.Id,
            ReceiptNumber = r.ReceiptNumber,
            PurchaseOrderId = r.PurchaseOrderId,
            PoNumber = r.PurchaseOrder?.PoNumber,
            WarehouseId = r.WarehouseId,
            WarehouseName = r.Warehouse?.Name,
            Status = r.Status,
            ReceivedDate = r.ReceivedDate,
            CreatedAt = r.CreatedAt,
            Items = r.Items?.Select(MapItemToDto).ToList() ?? new List<GoodsReceiptItemDto>(),
            Notes = r.Notes
        };

        private static GoodsReceiptItemDto MapItemToDto(GoodsReceiptItem i) => new()
        {
            Id = i.Id,
            GoodsReceiptId = i.GoodsReceiptId,
            PurchaseOrderItemId = i.PurchaseOrderItemId,
            ProductVariantId = i.ProductVariantId,
            ProductName = i.PurchaseOrderItem?.ProductName ?? string.Empty,
            VariantName = i.PurchaseOrderItem?.VariantName ?? string.Empty,
            Sku = i.PurchaseOrderItem?.Sku ?? string.Empty,
            QuantityReceived = i.QuantityReceived,
            QuantityRejected = i.QuantityRejected,
            Notes = i.Notes
        };
    }
}
