using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Reports;
using Application.UseCases.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportUseCase _reportUseCase;

        public ReportsController(ReportUseCase reportUseCase)
        {
            _reportUseCase = reportUseCase;
        }

        [HttpGet("profit-and-loss")]
        [RequirePermission(Permissions.ChartOfAccounts.Read)]
        public async Task<ActionResult<ProfitAndLossReport>> GetProfitAndLoss(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var fromDate = from ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var toDate = to ?? DateTime.UtcNow;
            var report = await _reportUseCase.GetProfitAndLossAsync(fromDate, toDate);
            return Ok(report);
        }

        [HttpGet("dashboard")]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<DashboardSummary>> GetDashboard()
        {
            var dashboard = await _reportUseCase.GetDashboardAsync();
            return Ok(dashboard);
        }

        // Finance-focused landing view for Accountants. Scoped to permissions the
        // accountant role already has: chart-of-accounts.read + invoices.read.
        [HttpGet("finance-dashboard")]
        [RequirePermission(Permissions.ChartOfAccounts.Read)]
        public async Task<ActionResult<FinanceDashboardSummary>> GetFinanceDashboard()
        {
            var dashboard = await _reportUseCase.GetFinanceDashboardAsync();
            return Ok(dashboard);
        }
    }
}
