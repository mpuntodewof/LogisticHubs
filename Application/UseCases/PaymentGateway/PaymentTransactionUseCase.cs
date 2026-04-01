using Application.DTOs.Common;
using Application.DTOs.PaymentGateway;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.PaymentGateway
{
    public class PaymentTransactionUseCase
    {
        private readonly IPaymentTransactionRepository _transactionRepository;
        private readonly IPaymentGatewayConfigRepository _configRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;

        public PaymentTransactionUseCase(
            IPaymentTransactionRepository transactionRepository,
            IPaymentGatewayConfigRepository configRepository,
            ISalesOrderRepository salesOrderRepository)
        {
            _transactionRepository = transactionRepository;
            _configRepository = configRepository;
            _salesOrderRepository = salesOrderRepository;
        }

        public async Task<PagedResult<PaymentTransactionDto>> GetPagedAsync(
            PagedRequest request, Guid? salesOrderPaymentId = null, string? status = null)
        {
            var result = await _transactionRepository.GetPagedAsync(request, salesOrderPaymentId, status);

            return new PagedResult<PaymentTransactionDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<PaymentTransactionDetailDto?> GetByIdAsync(Guid id)
        {
            var transaction = await _transactionRepository.GetDetailByIdAsync(id);
            return transaction == null ? null : MapToDetailDto(transaction);
        }

        public async Task<PaymentTransactionDto> InitiatePaymentAsync(InitiatePaymentRequest request)
        {
            // Load the sales order payment
            var salesOrderPayment = await _salesOrderRepository.GetPaymentByIdAsync(request.SalesOrderPaymentId)
                ?? throw new InvalidOperationException("Sales order payment not found.");

            // Load the payment gateway config
            var config = await _configRepository.GetByIdAsync(request.PaymentGatewayConfigId)
                ?? throw new InvalidOperationException("Payment gateway config not found.");

            if (!config.IsActive)
                throw new InvalidOperationException("Payment gateway config is not active.");

            // Generate transaction number
            var transactionNumber = GenerateTransactionNumber();

            var transaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                TransactionNumber = transactionNumber,
                SalesOrderPaymentId = request.SalesOrderPaymentId,
                PaymentGatewayConfigId = request.PaymentGatewayConfigId,
                Provider = config.Provider,
                PaymentMethod = salesOrderPayment.PaymentMethod,
                Amount = salesOrderPayment.Amount,
                Currency = "IDR",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Stub: In a real implementation, this is where we'd call the payment gateway API
            // and set ExternalTransactionId, PaymentUrl, etc.

            var created = await _transactionRepository.CreateAsync(transaction);
            return MapToDto(created);
        }

        public async Task UpdateStatusAsync(Guid transactionId, string newStatus)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId)
                ?? throw new InvalidOperationException("Payment transaction not found.");

            transaction.Status = newStatus;
            transaction.UpdatedAt = DateTime.UtcNow;

            if (newStatus == "Success")
            {
                transaction.PaidAt = DateTime.UtcNow;
            }
            else if (newStatus == "Failed")
            {
                // FailureReason can be set separately if needed
            }
            else if (newStatus == "Expired")
            {
                transaction.ExpiredAt = DateTime.UtcNow;
            }

            await _transactionRepository.UpdateAsync(transaction);
        }

        private static string GenerateTransactionNumber()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var suffix = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
            return $"PT-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
        }

        private static PaymentTransactionDto MapToDto(PaymentTransaction t) => new()
        {
            Id = t.Id,
            TransactionNumber = t.TransactionNumber,
            SalesOrderPaymentId = t.SalesOrderPaymentId,
            Provider = t.Provider,
            ExternalTransactionId = t.ExternalTransactionId,
            PaymentMethod = t.PaymentMethod,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            PaidAt = t.PaidAt,
            CreatedAt = t.CreatedAt
        };

        private static PaymentTransactionDetailDto MapToDetailDto(PaymentTransaction t) => new()
        {
            Id = t.Id,
            TransactionNumber = t.TransactionNumber,
            SalesOrderPaymentId = t.SalesOrderPaymentId,
            Provider = t.Provider,
            ExternalTransactionId = t.ExternalTransactionId,
            PaymentMethod = t.PaymentMethod,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            PaidAt = t.PaidAt,
            CreatedAt = t.CreatedAt,
            ExternalReferenceId = t.ExternalReferenceId,
            PaymentUrl = t.PaymentUrl,
            GatewayResponse = t.GatewayResponse,
            FailureReason = t.FailureReason,
            ExpiredAt = t.ExpiredAt,
            RefundedAt = t.RefundedAt
        };
    }
}
