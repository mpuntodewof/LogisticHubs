using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class FinanceClient : BaseApiClient
    {
        public FinanceClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        #region Chart of Accounts

        public async Task<ApiResult<PagedResult<ChartOfAccountDto>>> GetChartOfAccountsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/chart-of-accounts?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ChartOfAccountDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ChartOfAccountDto>>();
            return ApiResult<PagedResult<ChartOfAccountDto>>.Ok(data!);
        }

        public async Task<ApiResult<ChartOfAccountDto>> CreateChartOfAccountAsync(CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/chart-of-accounts", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ChartOfAccountDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
            return ApiResult<ChartOfAccountDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateChartOfAccountAsync(Guid id, UpdateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/chart-of-accounts/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Journal Entries

        public async Task<ApiResult<PagedResult<JournalEntryDto>>> GetJournalEntriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/journal-entries?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<JournalEntryDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<JournalEntryDto>>();
            return ApiResult<PagedResult<JournalEntryDto>>.Ok(data!);
        }

        #endregion

        #region Invoices

        public async Task<ApiResult<PagedResult<InvoiceDto>>> GetInvoicesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/invoices?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<InvoiceDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<InvoiceDto>>();
            return ApiResult<PagedResult<InvoiceDto>>.Ok(data!);
        }

        #endregion

        #region Tax Rates

        public async Task<ApiResult<PagedResult<TaxRateDto>>> GetTaxRatesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/tax-rates?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<TaxRateDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<TaxRateDto>>();
            return ApiResult<PagedResult<TaxRateDto>>.Ok(data!);
        }

        public async Task<ApiResult<TaxRateDto>> CreateTaxRateAsync(CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/tax-rates", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<TaxRateDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<TaxRateDto>();
            return ApiResult<TaxRateDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateTaxRateAsync(Guid id, CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/tax-rates/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Payment Terms

        public async Task<ApiResult<PagedResult<PaymentTermDto>>> GetPaymentTermsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/payment-terms?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<PaymentTermDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<PaymentTermDto>>();
            return ApiResult<PagedResult<PaymentTermDto>>.Ok(data!);
        }

        public async Task<ApiResult<PaymentTermDto>> CreatePaymentTermAsync(CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/payment-terms", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PaymentTermDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PaymentTermDto>();
            return ApiResult<PaymentTermDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePaymentTermAsync(Guid id, CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/payment-terms/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion
    }
}
