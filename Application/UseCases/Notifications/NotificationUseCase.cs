using Application.DTOs.Notifications;
using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases.Notifications
{
    public class NotificationUseCase
    {
        private readonly IReportRepository _reportRepository;

        public NotificationUseCase(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<NotificationSummary> GetActiveNotificationsAsync()
        {
            var notifications = new List<NotificationDto>();

            // Low stock alerts
            var lowStockAlerts = await _reportRepository.GetLowStockAlertsAsync(50);
            foreach (var alert in lowStockAlerts)
            {
                var severity = alert.QuantityOnHand == 0 ? "critical" : "warning";
                var title = alert.QuantityOnHand == 0
                    ? $"Out of Stock: {alert.Sku}"
                    : $"Low Stock: {alert.Sku}";

                notifications.Add(new NotificationDto
                {
                    Id = alert.WarehouseStockId,
                    Type = "low-stock",
                    Title = title,
                    Message = $"{alert.ProductName} at {alert.WarehouseName}: {alert.QuantityOnHand} units (reorder point: {alert.ReorderPoint ?? 0})",
                    Severity = severity,
                    EntityType = "warehouse-stock",
                    EntityId = alert.WarehouseStockId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Overdue invoices
            var financeSummary = await _reportRepository.GetFinanceSummaryAsync();
            if (financeSummary.OverdueInvoices > 0)
            {
                notifications.Add(new NotificationDto
                {
                    Id = Guid.NewGuid(),
                    Type = "overdue-invoice",
                    Title = $"{financeSummary.OverdueInvoices} Overdue Invoice(s)",
                    Message = $"Total overdue: Rp {financeSummary.OverdueAmount:N0}. Review and follow up on outstanding payments.",
                    Severity = financeSummary.OverdueAmount > 10_000_000 ? "critical" : "warning",
                    EntityType = "invoice",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return new NotificationSummary
            {
                TotalUnread = notifications.Count,
                Notifications = notifications.OrderByDescending(n => n.Severity == "critical")
                    .ThenByDescending(n => n.Severity == "warning")
                    .ThenByDescending(n => n.CreatedAt)
                    .ToList()
            };
        }
    }
}
