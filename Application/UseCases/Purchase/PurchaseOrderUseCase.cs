using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Purchase
{
    public class PurchaseOrderUseCase
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public PurchaseOrderUseCase(
            IPurchaseOrderRepository purchaseOrderRepository,
            IProductVariantRepository productVariantRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<PagedResult<PurchaseOrderDto>> GetPagedAsync(
            PagedRequest request, string? status = null, Guid? supplierId = null, Guid? warehouseId = null)
        {
            var result = await _purchaseOrderRepository.GetPagedAsync(request, status, supplierId, warehouseId);

            return new PagedResult<PurchaseOrderDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<PurchaseOrderDetailDto?> GetByIdAsync(Guid id)
        {
            var order = await _purchaseOrderRepository.GetDetailByIdAsync(id);
            return order == null ? null : MapToDetailDto(order);
        }

        public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Purchase order must have at least one item.");

            var poNumber = await GeneratePoNumberAsync();

            var order = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                PoNumber = poNumber,
                SupplierId = request.SupplierId,
                WarehouseId = request.WarehouseId,
                BranchId = request.BranchId,
                Status = "Draft",
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                DiscountAmount = 0,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var items = new List<PurchaseOrderItem>();

            foreach (var itemRequest in request.Items)
            {
                var variant = await _productVariantRepository.GetByIdAsync(itemRequest.ProductVariantId)
                    ?? throw new InvalidOperationException($"Product variant with ID '{itemRequest.ProductVariantId}' not found.");

                var discountAmount = itemRequest.DiscountAmount ?? 0m;
                var lineTotal = (itemRequest.UnitCost * itemRequest.Quantity) - discountAmount;

                var item = new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = order.Id,
                    ProductVariantId = variant.Id,
                    ProductName = variant.Product?.Name ?? string.Empty,
                    VariantName = variant.Name,
                    Sku = variant.Sku,
                    Quantity = itemRequest.Quantity,
                    UnitCost = itemRequest.UnitCost,
                    DiscountAmount = discountAmount,
                    TaxAmount = 0,
                    LineTotal = lineTotal,
                    ReceivedQuantity = 0,
                    Notes = itemRequest.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                items.Add(item);
            }

            order.Items = items;
            order.SubTotal = items.Sum(i => i.LineTotal);
            order.TaxAmount = order.SubTotal * 0.11m;
            order.GrandTotal = order.SubTotal + order.TaxAmount - order.DiscountAmount;

            var created = await _purchaseOrderRepository.CreateAsync(order);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdatePurchaseOrderRequest request)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (order.Status != "Draft")
                throw new InvalidOperationException("Only draft purchase orders can be updated.");

            if (request.ExpectedDeliveryDate.HasValue) order.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
            if (request.DiscountAmount.HasValue) order.DiscountAmount = request.DiscountAmount.Value;
            if (request.Notes != null) order.Notes = request.Notes;

            if (request.DiscountAmount.HasValue)
            {
                order.GrandTotal = order.SubTotal + order.TaxAmount - order.DiscountAmount;
            }

            order.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.UpdateAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (order.Status != "Draft" && order.Status != "Cancelled")
                throw new InvalidOperationException("Only draft or cancelled purchase orders can be deleted.");

            await _purchaseOrderRepository.DeleteAsync(order);
        }

        public async Task SubmitAsync(Guid id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (order.Status != "Draft")
                throw new InvalidOperationException("Only draft purchase orders can be submitted.");

            order.Status = "Submitted";
            order.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.UpdateAsync(order);
        }

        public async Task ApproveAsync(Guid id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (order.Status != "Submitted")
                throw new InvalidOperationException("Only submitted purchase orders can be approved.");

            order.Status = "Approved";
            order.ApprovedBy = null; // Will be set by SaveChanges audit via ICurrentUserService
            order.ApprovedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.UpdateAsync(order);
        }

        public async Task CancelAsync(Guid id, string reason)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Purchase order not found.");

            if (order.Status != "Draft" && order.Status != "Submitted")
                throw new InvalidOperationException("Only draft or submitted purchase orders can be cancelled.");

            order.Status = "Cancelled";
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason;
            order.UpdatedAt = DateTime.UtcNow;

            await _purchaseOrderRepository.UpdateAsync(order);
        }

        private async Task<string> GeneratePoNumberAsync()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string poNumber;

            do
            {
                var suffix = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                poNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            }
            while (await _purchaseOrderRepository.PoNumberExistsAsync(poNumber));

            return poNumber;
        }

        private static PurchaseOrderDto MapToDto(PurchaseOrder o) => new()
        {
            Id = o.Id,
            PoNumber = o.PoNumber,
            SupplierId = o.SupplierId,
            SupplierName = o.Supplier?.CompanyName,
            WarehouseId = o.WarehouseId,
            WarehouseName = o.Warehouse?.Name,
            BranchId = o.BranchId,
            BranchName = o.Branch?.Name,
            Status = o.Status,
            OrderDate = o.OrderDate,
            ExpectedDeliveryDate = o.ExpectedDeliveryDate,
            SubTotal = o.SubTotal,
            TaxAmount = o.TaxAmount,
            DiscountAmount = o.DiscountAmount,
            GrandTotal = o.GrandTotal,
            CreatedAt = o.CreatedAt
        };

        private static PurchaseOrderDetailDto MapToDetailDto(PurchaseOrder o) => new()
        {
            Id = o.Id,
            PoNumber = o.PoNumber,
            SupplierId = o.SupplierId,
            SupplierName = o.Supplier?.CompanyName,
            WarehouseId = o.WarehouseId,
            WarehouseName = o.Warehouse?.Name,
            BranchId = o.BranchId,
            BranchName = o.Branch?.Name,
            Status = o.Status,
            OrderDate = o.OrderDate,
            ExpectedDeliveryDate = o.ExpectedDeliveryDate,
            SubTotal = o.SubTotal,
            TaxAmount = o.TaxAmount,
            DiscountAmount = o.DiscountAmount,
            GrandTotal = o.GrandTotal,
            CreatedAt = o.CreatedAt,
            Items = o.Items?.Select(MapItemToDto).ToList() ?? new List<PurchaseOrderItemDto>(),
            Notes = o.Notes,
            ApprovedBy = o.ApprovedBy,
            ApprovedAt = o.ApprovedAt
        };

        private static PurchaseOrderItemDto MapItemToDto(PurchaseOrderItem i) => new()
        {
            Id = i.Id,
            PurchaseOrderId = i.PurchaseOrderId,
            ProductVariantId = i.ProductVariantId,
            ProductName = i.ProductName,
            VariantName = i.VariantName,
            Sku = i.Sku,
            Quantity = i.Quantity,
            UnitCost = i.UnitCost,
            DiscountAmount = i.DiscountAmount,
            TaxAmount = i.TaxAmount,
            LineTotal = i.LineTotal,
            ReceivedQuantity = i.ReceivedQuantity
        };
    }
}
