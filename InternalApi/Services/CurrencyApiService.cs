using System.Text.Json;
using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.Exceptions.ApiExceptions;
using Fuse8.BackendInternship.InternalApi.Exceptions.BusinessLogicExceptions;
using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.InternalApi.Models.Responses;
using Fuse8.BackendInternship.InternalApi.Models.Types;
using Fuse8.BackendInternship.InternalApi.Settings;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.InternalApi.Services;

public class CurrencyApiService : ICurrencyApiService
{
    private const string StatusPath = "status";
    private const string LatestPath = "latest";
    private const string OnDatePath = "historical";

    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<CurrencySettings> _currencySettingsMonitor;

    public CurrencyApiService(
        HttpClient httpClient,
        IOptionsMonitor<ApiSettings> apiSettingsSnapshot,
        IOptionsMonitor<CurrencySettings> currencySettingsSnapshot)
    {
        _httpClient = httpClient;
        _currencySettingsMonitor = currencySettingsSnapshot;

        _httpClient.BaseAddress = new Uri(apiSettingsSnapshot.CurrentValue.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("apikey", apiSettingsSnapshot.CurrentValue.ApiKey);
    }
    
    public async Task<SettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var statusResponse = await GetRequestInfoAsync(cancellationToken);
        
        if (statusResponse.Quotas?.Month is null)
        {
            throw new HttpRequestException("Unexpected /status response structure");
        }
        
        var currencySettings = _currencySettingsMonitor.CurrentValue;

        return new SettingsDto(
            statusResponse.Quotas.Month.Total,
            statusResponse.Quotas.Month.Used,
            currencySettings.CurrencyRoundCount
        );
    }

    public async Task<CurrencyRateDto> GetCurrencyRateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        CancellationToken cancellationToken)
    {
        await CheckRequestLimitAsync(cancellationToken);

        var key = ParseEnum(currencyCode);
        var currencySettings = _currencySettingsMonitor.CurrentValue;
        var url = 
            $"{LatestPath}?" +
            $"currencies={key}&" +
            $"base_currency={ParseEnum(baseCurrencyCode)}&";

        var model = await SendCurrencyRequestAsync(url, cancellationToken);
        
        if (model.Data is null || !model.Data.TryGetValue(key, out var currencyItem))
        {
            throw new HttpRequestException($"Unexpected /latest? response structure, no '{key}' in data.");
        }

        var roundedValue = RoundValue(currencyItem.Value, currencySettings.CurrencyRoundCount);

        return new CurrencyRateDto(currencyCode, roundedValue);
    }

    public async Task<CurrencyRateOnDateDto> GetCurrencyRateOnDateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        DateOnly date,
        CancellationToken cancellationToken)
    {
        await CheckRequestLimitAsync(cancellationToken);
        
        var key = ParseEnum(currencyCode);
        var currencySettings = _currencySettingsMonitor.CurrentValue;
        var url =
            $"{OnDatePath}?" + 
            $"currencies={ParseEnum(currencyCode)}&" +
            $"date={date.ToString("yyyy-MM-dd")}&" +
            $"base_currency={ParseEnum(baseCurrencyCode)}";
        
        var model = await SendCurrencyRequestAsync(url, cancellationToken);
        
        if (model.Data is null || !model.Data.TryGetValue(key, out var currencyItem))
        {
            throw new HttpRequestException($"Unexpected /latest? response structure, no '{key}' in data.");
        }
        
        var roundedValue = RoundValue(currencyItem.Value, currencySettings.CurrencyRoundCount);

        return new CurrencyRateOnDateDto(currencyCode, roundedValue, date);
    }

    public async Task<IEnumerable<CurrencyRateDto>> GetAllCurrentCurrenciesAsync(
        CurrencyCode baseCurrencyCode, 
        CancellationToken cancellationToken)
    {
        await CheckRequestLimitAsync(cancellationToken);
            
        var url = $"{LatestPath}?base_currency={ParseEnum(baseCurrencyCode)}";

        var model = await SendCurrencyRequestAsync(url, cancellationToken);

        if (model.Data is null)
        {
            throw new HttpRequestException("Unexpected /latest? response structure");
        }

        var currencySettings = _currencySettingsMonitor.CurrentValue;
        var result = new List<CurrencyRateDto>();
        
        foreach (var (codeName, currencyItem) in model.Data)
        {
            if (!Enum.TryParse<CurrencyCode>(codeName, ignoreCase: true, out var code))
            {
                continue;
            }

            var rounded = RoundValue(currencyItem.Value, currencySettings.CurrencyRoundCount);
            result.Add(new CurrencyRateDto(code, rounded));
        }
        
        return result;
    }

    public async Task<CurrenciesOnDateDto> GetAllCurrenciesOnDateAsync(
        CurrencyCode baseCurrencyCode, 
        DateOnly date, 
        CancellationToken cancellationToken)
    {
        await CheckRequestLimitAsync(cancellationToken);
            
        var url =
            $"{OnDatePath}?" +
            $"date={date.ToString("yyyy-MM-dd")}&" +
            $"base_currency={ParseEnum(baseCurrencyCode)}";
        
        var model = await SendCurrencyRequestAsync(url, cancellationToken);

        if (model.Data is null)
        {
            throw new HttpRequestException("Unexpected /historical? response structure.");
        }

        var currencySettings = _currencySettingsMonitor.CurrentValue;
        var result = new List<CurrencyRateDto>();

        foreach (var (codeName, currencyItem) in model.Data)
        {
            if (!Enum.TryParse<CurrencyCode>(codeName, ignoreCase: true, out var code))
            {
                continue;
            }

            var rounded = RoundValue(currencyItem.Value, currencySettings.CurrencyRoundCount);
            result.Add(new CurrencyRateDto(code, rounded));
        }

        return new CurrenciesOnDateDto(date, result);
    }

    private async Task<StatusResponse> GetRequestInfoAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(StatusPath, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var statusResponse = JsonSerializer.Deserialize<StatusResponse>(content);
        
        if (statusResponse?.Quotas?.Month is null)
        {
            throw new HttpRequestException("Unexpected /status response structure");
        }

        return statusResponse;
    }

    private async Task CheckRequestLimitAsync(CancellationToken cancellationToken)
    {
        var statusResponse = await GetRequestInfoAsync(cancellationToken);

        if (statusResponse.Quotas?.Month?.Total < statusResponse.Quotas?.Month?.Used)
        {
            throw new ApiRequestLimitException();
        }
    }

    private async Task<CurrencyApiResponse> SendCurrencyRequestAsync(string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
    
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorApiResponse>(cancellationToken);

            if (error is { Message: "Validation error", Errors: not null } &&
                error.Errors.TryGetValue("currencies", out var currencies) &&
                currencies.Contains("The selected currencies is invalid."))
            {
                throw new CurrencyNotFoundException();
            }
        }

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        var model = JsonSerializer.Deserialize<CurrencyApiResponse>(responseContent);

        if (model?.Data is null)
        {
            throw new HttpRequestException("Unexpected response structure: no 'data' property or it's null.");
        }

        return model;
    }

    private static decimal RoundValue(decimal value, int precision)
    {
        return Math.Round(value, precision);
    }

    private static string ParseEnum(CurrencyCode currencyCode)
    {
        return currencyCode.ToString().ToUpper();
    }
}