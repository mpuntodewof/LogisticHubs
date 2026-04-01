using Application.DTOs.Common;
using Application.DTOs.Customers;
using Application.DTOs.Sales;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Sales
{
    public class SalesOrderUseCase
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IBranchRepository _branchRepository;

        public SalesOrderUseCase(
            ISalesOrderRepository salesOrderRepository,
            IProductVariantRepository productVariantRepository,
            IWarehouseStockRepository warehouseStockRepository,
            IStockMovementRepository stockMovementRepository,
            IBranchRepository branchRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _productVariantRepository = productVariantRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _stockMovementRepository = stockMovementRepository;
            _branchRepository = branchRepository;
        }

        public async Task<PagedResult<SalesOrderDto>> GetPagedAsync(
            PagedRequest request, string? status = null, Guid? customerId = null, Guid? branchId = null)
        {
            var result = await _salesOrderRepository.GetPagedAsync(request, status, customerId, branchId);

            return new PagedResult<SalesOrderDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<SalesOrderDetailDto?> GetByIdAsync(Guid id)
        {
            var order = await _salesOrderRepository.GetDetailByIdAsync(id);
            return order == null ? null : MapToDetailDto(order);
        }

        public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Sales order must have at least one item.");

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            var order = new SalesOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderType = request.OrderType.ToString(),
                Status = SalesOrderStatus.Draft.ToString(),
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                ShippingAddressId = request.ShippingAddressId,
                DiscountAmount = request.DiscountAmount,
                Notes = request.Notes,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var items = new List<SalesOrderItem>();

            foreach (var itemRequest in request.Items)
            {
                var variant = await _productVariantRepository.GetByIdAsync(itemRequest.ProductVariantId)
                    ?? throw new InvalidOperationException($"Product variant with ID '{itemRequest.ProductVariantId}' not found.");

                var discountAmount = itemRequest.DiscountAmount ?? 0m;
                var lineTotal = (variant.SellingPrice * itemRequest.Quantity) - discountAmount;

                var item = new SalesOrderItem
                {
                    Id = Guid.NewGuid(),
                    SalesOrderId = order.Id,
                    ProductVariantId = variant.Id,
                    ProductName = variant.Product?.Name ?? string.Empty,
                    VariantName = variant.Name,
                    Sku = variant.Sku,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = variant.SellingPrice,
                    DiscountAmount = discountAmount,
                    TaxAmount = 0,
                    LineTotal = lineTotal,
                    CreatedAt = DateTime.UtcNow
                };

                items.Add(item);
            }

            order.Items = items;
            order.SubTotal = items.Sum(i => i.LineTotal);
            order.TaxAmount = (order.SubTotal - order.DiscountAmount) * 0.11m;
            order.ShippingCost = 0;
            order.GrandTotal = order.SubTotal - order.DiscountAmount + order.TaxAmount + order.ShippingCost;

            var created = await _salesOrderRepository.CreateAsync(order);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateSalesOrderRequest request)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Draft.ToString())
                throw new InvalidOperationException("Only draft orders can be updated.");

            if (request.ShippingAddressId.HasValue) order.ShippingAddressId = request.ShippingAddressId;
            if (request.DiscountAmount.HasValue) order.DiscountAmount = request.DiscountAmount.Value;
            if (request.ShippingCost.HasValue) order.ShippingCost = request.ShippingCost.Value;
            if (request.Notes != null) order.Notes = request.Notes;

            // Recalculate totals if discount or shipping changed
            if (request.DiscountAmount.HasValue || request.ShippingCost.HasValue)
            {
                order.TaxAmount = (order.SubTotal - order.DiscountAmount) * 0.11m;
                order.GrandTotal = order.SubTotal - order.DiscountAmount + order.TaxAmount + order.ShippingCost;
            }

            order.UpdatedAt = DateTime.UtcNow;

            await _salesOrderRepository.UpdateAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Draft.ToString() && order.Status != SalesOrderStatus.Cancelled.ToString())
                throw new InvalidOperationException("Only draft or cancelled orders can be deleted.");

            await _salesOrderRepository.DeleteAsync(order);
        }

        public async Task ConfirmAsync(Guid id)
        {
            var order = await _salesOrderRepository.GetDetailByIdAsync(id)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Draft.ToString())
                throw new InvalidOperationException("Only draft orders can be confirmed.");

            order.Status = SalesOrderStatus.Confirmed.ToString();
            order.UpdatedAt = DateTime.UtcNow;

            // Deduct stock if branch has a warehouse
            if (order.BranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAsync(order.BranchId.Value);
                if (branch?.WarehouseId != null)
                {
                    var warehouseId = branch.WarehouseId.Value;

                    foreach (var item in order.Items)
                    {
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

                        if (stock.QuantityOnHand < item.Quantity)
                            throw new InvalidOperationException($"Insufficient stock for '{item.ProductName} - {item.VariantName}'. Available: {stock.QuantityOnHand}, Required: {item.Quantity}.");

                        stock.QuantityOnHand -= item.Quantity;

                        var movement = new StockMovement
                        {
                            Id = Guid.NewGuid(),
                            WarehouseId = warehouseId,
                            ProductVariantId = item.ProductVariantId,
                            MovementType = StockMovementType.Out.ToString(),
                            Reason = StockMovementReason.Sale.ToString(),
                            Quantity = item.Quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = stock.QuantityOnHand,
                            ReferenceDocumentType = "SalesOrder",
                            ReferenceDocumentId = order.Id,
                            ReferenceDocumentNumber = order.OrderNumber,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (isNewStock)
                            await _warehouseStockRepository.CreateAsync(stock);
                        else
                            await _warehouseStockRepository.UpdateAsync(stock);

                        await _stockMovementRepository.CreateAsync(movement);
                    }
                }
            }

            await _salesOrderRepository.UpdateAsync(order);
        }

        public async Task CancelAsync(Guid id, string reason)
        {
            var order = await _salesOrderRepository.GetDetailByIdAsync(id)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Draft.ToString() && order.Status != SalesOrderStatus.Confirmed.ToString())
                throw new InvalidOperationException("Only draft or confirmed orders can be cancelled.");

            var wasConfirmed = order.Status == SalesOrderStatus.Confirmed.ToString();

            order.Status = SalesOrderStatus.Cancelled.ToString();
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason;
            order.UpdatedAt = DateTime.UtcNow;

            // Reverse stock if was confirmed
            if (wasConfirmed && order.BranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAsync(order.BranchId.Value);
                if (branch?.WarehouseId != null)
                {
                    var warehouseId = branch.WarehouseId.Value;

                    foreach (var item in order.Items)
                    {
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
                        stock.QuantityOnHand += item.Quantity;

                        var movement = new StockMovement
                        {
                            Id = Guid.NewGuid(),
                            WarehouseId = warehouseId,
                            ProductVariantId = item.ProductVariantId,
                            MovementType = StockMovementType.In.ToString(),
                            Reason = StockMovementReason.CustomerReturn.ToString(),
                            Quantity = item.Quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = stock.QuantityOnHand,
                            ReferenceDocumentType = "SalesOrder",
                            ReferenceDocumentId = order.Id,
                            ReferenceDocumentNumber = order.OrderNumber,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (isNewStock)
                            await _warehouseStockRepository.CreateAsync(stock);
                        else
                            await _warehouseStockRepository.UpdateAsync(stock);

                        await _stockMovementRepository.CreateAsync(movement);
                    }
                }
            }

            await _salesOrderRepository.UpdateAsync(order);
        }

        public async Task<SalesOrderPaymentDto> AddPaymentAsync(Guid orderId, CreateSalesOrderPaymentRequest request)
        {
            var order = await _salesOrderRepository.GetByIdAsync(orderId)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Confirmed.ToString() && order.Status != SalesOrderStatus.Processing.ToString())
                throw new InvalidOperationException("Payments can only be added to confirmed or processing orders.");

            var payment = new SalesOrderPayment
            {
                Id = Guid.NewGuid(),
                SalesOrderId = orderId,
                PaymentMethod = request.PaymentMethod.ToString(),
                PaymentStatus = PaymentStatus.Paid.ToString(),
                Amount = request.Amount,
                ReferenceNumber = request.ReferenceNumber,
                PaidAt = DateTime.UtcNow,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _salesOrderRepository.AddPaymentAsync(payment);
            return MapPaymentToDto(created);
        }

        public async Task<IEnumerable<SalesOrderPaymentDto>> GetPaymentsAsync(Guid orderId)
        {
            var payments = await _salesOrderRepository.GetPaymentsAsync(orderId);
            return payments.Select(MapPaymentToDto);
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string orderNumber;

            do
            {
                var suffix = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                orderNumber = $"SO-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            }
            while (await _salesOrderRepository.OrderNumberExistsAsync(orderNumber));

            return orderNumber;
        }

        private static SalesOrderDto MapToDto(SalesOrder o) => new()
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderType = o.OrderType,
            Status = o.Status,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.Name,
            BranchId = o.BranchId,
            BranchName = o.Branch?.Name,
            SubTotal = o.SubTotal,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            ShippingCost = o.ShippingCost,
            GrandTotal = o.GrandTotal,
            OrderDate = o.OrderDate,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt
        };

        private static SalesOrderDetailDto MapToDetailDto(SalesOrder o) => new()
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderType = o.OrderType,
            Status = o.Status,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.Name,
            BranchId = o.BranchId,
            BranchName = o.Branch?.Name,
            SubTotal = o.SubTotal,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            ShippingCost = o.ShippingCost,
            GrandTotal = o.GrandTotal,
            OrderDate = o.OrderDate,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            Items = o.Items?.Select(MapItemToDto).ToList() ?? new List<SalesOrderItemDto>(),
            Payments = o.Payments?.Select(MapPaymentToDto).ToList() ?? new List<SalesOrderPaymentDto>(),
            ShippingAddress = o.ShippingAddress != null ? MapAddressToDto(o.ShippingAddress) : null
        };

        private static SalesOrderItemDto MapItemToDto(SalesOrderItem i) => new()
        {
            Id = i.Id,
            SalesOrderId = i.SalesOrderId,
            ProductVariantId = i.ProductVariantId,
            ProductName = i.ProductName,
            VariantName = i.VariantName,
            Sku = i.Sku,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            DiscountAmount = i.DiscountAmount,
            TaxAmount = i.TaxAmount,
            LineTotal = i.LineTotal
        };

        private static SalesOrderPaymentDto MapPaymentToDto(SalesOrderPayment p) => new()
        {
            Id = p.Id,
            SalesOrderId = p.SalesOrderId,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus,
            Amount = p.Amount,
            ReferenceNumber = p.ReferenceNumber,
            PaidAt = p.PaidAt,
            Notes = p.Notes
        };

        private static CustomerAddressDto MapAddressToDto(CustomerAddress a) => new()
        {
            Id = a.Id,
            CustomerId = a.CustomerId,
            Label = a.Label,
            AddressType = a.AddressType,
            RecipientName = a.RecipientName,
            Phone = a.Phone,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsDefault = a.IsDefault,
            CreatedAt = a.CreatedAt
        };
    }
}
