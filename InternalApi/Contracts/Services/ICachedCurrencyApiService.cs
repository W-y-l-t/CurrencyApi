using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Contracts.Services;

public interface ICachedCurrencyApiService
{
    Task<CurrencyRateDto> GetCurrentCurrencyAsync(
        CurrencyCode baseCurrencyCode, 
        CurrencyCode currencyCode, 
        CancellationToken cancellationToken);
    
    Task<CurrencyRateDto> GetCurrencyOnDateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        DateOnly date, 
        CancellationToken cancellationToken);
    
    Task<MinimalSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
}