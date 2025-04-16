using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Contracts.Services;

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
    
    Task<IEnumerable<CurrencyRateDto>> GetAllCurrentCurrenciesAsync(
        CurrencyCode baseCurrencyCode, 
        CancellationToken cancellationToken);
    
    Task<CurrenciesOnDateDto> GetAllCurrenciesOnDateAsync(
        CurrencyCode baseCurrencyCode, 
        DateOnly date, 
        CancellationToken cancellationToken);
}