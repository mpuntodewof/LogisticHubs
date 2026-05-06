using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class ReportingClient : BaseApiClient
    {
        public ReportingClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        #region Audit Logs

        public async Task<ApiResult<PagedResult<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/audit-logs?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<AuditLogDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<AuditLogDto>>();
            return ApiResult<PagedResult<AuditLogDto>>.Ok(data!);
        }

        #endregion

        #region Dashboard

        public async Task<ApiResult<DashboardSummary>> GetDashboardSummaryAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/reports/dashboard");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<DashboardSummary>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<DashboardSummary>();
            return ApiResult<DashboardSummary>.Ok(data!);
        }

        public async Task<ApiResult<FinanceDashboardSummary>> GetFinanceDashboardSummaryAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/reports/finance-dashboard");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<FinanceDashboardSummary>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<FinanceDashboardSummary>();
            return ApiResult<FinanceDashboardSummary>.Ok(data!);
        }

        #endregion

        #region Reports

        public async Task<ApiResult<ProfitAndLossReport>> GetProfitAndLossAsync(DateTime? from = null, DateTime? to = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/reports/profit-and-loss";
            var queryParams = new List<string>();
            if (from.HasValue) queryParams.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue) queryParams.Add($"to={to.Value:yyyy-MM-dd}");
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<ProfitAndLossReport>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<ProfitAndLossReport>();
            return ApiResult<ProfitAndLossReport>.Ok(data!);
        }

        public async Task<ApiResult<PpnSummaryReport>> GetPpnSummaryAsync(int year, int month)
        {
            await AttachTokenAsync();
            var url = $"{V1}/reports/ppn-summary?year={year}&month={month}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PpnSummaryReport>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PpnSummaryReport>();
            return ApiResult<PpnSummaryReport>.Ok(data!);
        }

        public async Task<ApiResult<ProductMarginReport>> GetMarginPerProductAsync(DateTime? from = null, DateTime? to = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/reports/margin-per-product";
            var queryParams = new List<string>();
            if (from.HasValue) queryParams.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue) queryParams.Add($"to={to.Value:yyyy-MM-dd}");
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<ProductMarginReport>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<ProductMarginReport>();
            return ApiResult<ProductMarginReport>.Ok(data!);
        }

        #endregion
    }
}
