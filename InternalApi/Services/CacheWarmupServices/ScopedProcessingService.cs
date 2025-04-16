using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Services.CacheWarmupServices;

public class ScopedProcessingService : IScopedProcessingService
{
    private const CurrencyCode CacheBase = CurrencyCode.Usd;

    private readonly ILogger<ScopedProcessingService> _logger;
    private readonly ICachedCurrencyApiService _cachedCurrencyApiService;
    
    private readonly CurrencyCode[] _currenciesToWarm = [
        CurrencyCode.Usd, CurrencyCode.Rub, CurrencyCode.Kzt, CurrencyCode.Eur, CurrencyCode.Gbp, 
        CurrencyCode.Jpy, CurrencyCode.Cny, CurrencyCode.Chf, CurrencyCode.Inr, CurrencyCode.Aud];

    public ScopedProcessingService(
        ILogger<ScopedProcessingService> logger, 
        ICachedCurrencyApiService cachedCurrencyApiService)
    {
        _logger = logger;
        _cachedCurrencyApiService = cachedCurrencyApiService;
    }
    
    public async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var currency in _currenciesToWarm)
            {
                try
                {
                    var result = await _cachedCurrencyApiService.GetCurrentCurrencyAsync(
                        CacheBase, 
                        currency, 
                        stoppingToken);
                    
                    _logger.LogInformation("Cache warmed for {Currency}: {Rate}", currency, result.Rate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error warming cache for {Currency}", currency);
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
        }
    }
}