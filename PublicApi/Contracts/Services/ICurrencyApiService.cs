using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Contracts.Services;

public interface ICurrencyApiService
{
    Task<SettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
    
    Task<CurrencyRateDto> GetCurrencyRateAsync(
        CurrencyCode baseCurrencyCode, 
        CurrencyCode currencyCode, 
        CancellationToken cancellationToken);
    
    Task<CurrencyRateOnDateDto> GetCurrencyRateOnDateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        DateOnly date, 
        CancellationToken cancellationToken);
}