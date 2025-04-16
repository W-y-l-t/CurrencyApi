using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.DataAccess.DbContexts;
using Fuse8.BackendInternship.InternalApi.Exceptions.DataBaseExceptions;
using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.InternalApi.Models.Entities;
using Fuse8.BackendInternship.InternalApi.Models.Types;
using Fuse8.BackendInternship.InternalApi.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.InternalApi.Services;

public class CachedCurrencyApiService : ICachedCurrencyApiService
{
    private const CurrencyCode CacheBase = CurrencyCode.Usd;

    private readonly TimeSpan _freshnessPeriod = TimeSpan.FromHours(2);

    private readonly ICurrencyApiService _currencyApiService;
    private readonly CurrencyCacheContext _db;
    private readonly IOptionsMonitor<CurrencySettings> _currencySettingsMonitor;

    public CachedCurrencyApiService(
        ICurrencyApiService currencyApiService, 
        CurrencyCacheContext db,
        IOptionsMonitor<CurrencySettings> currencySettingsMonitor)
    {
        _currencyApiService = currencyApiService;
        _db = db;
        _currencySettingsMonitor = currencySettingsMonitor;
    }

    public async Task<CurrencyRateDto> GetCurrentCurrencyAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow; 
        var today = DateOnly.FromDateTime(now);

        var entryCurrency = await _db.CurrencyCacheEntries
            .Where(e => e.BaseCurrency == CacheBase &&
                        e.Currency == currencyCode &&
                        e.TargetDate == today)
            .OrderByDescending(e => e.CachedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var needUpdate = entryCurrency is null || now - entryCurrency.CachedAt > _freshnessPeriod;

        if (needUpdate)
        {
            var allRates = await _currencyApiService.GetAllCurrentCurrenciesAsync(CacheBase, cancellationToken);

            foreach (var rate in allRates)
            {
                var newEntry = new CurrencyCacheEntry
                {
                    BaseCurrency = CacheBase,
                    Currency = rate.Code,
                    Rate = rate.Rate,
                    CachedAt = now,
                    TargetDate = today
                };
                
                await _db.CurrencyCacheEntries.AddAsync(newEntry, cancellationToken);
            }
            
            await _db.SaveChangesAsync(cancellationToken);

            entryCurrency = await _db.CurrencyCacheEntries
                .Where(e => e.BaseCurrency == CacheBase &&
                            e.Currency == currencyCode &&
                            e.TargetDate == today)
                .OrderByDescending(e => e.CachedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var precision = _currencySettingsMonitor.CurrentValue.CurrencyRoundCount;

        if (baseCurrencyCode == CacheBase)
        {
            if (entryCurrency is null)
            {
                throw new DataNotFoundException(currencyCode);
            }
            
            return new CurrencyRateDto(currencyCode, RoundValue(entryCurrency.Rate, precision));
        }
        
        var entryBase = await _db.CurrencyCacheEntries
            .Where(e => e.BaseCurrency == CacheBase &&
                        e.Currency == baseCurrencyCode &&
                        e.TargetDate == today)
            .OrderByDescending(e => e.CachedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (entryCurrency is null || entryBase is null)
        {
            throw new DataNotFoundException(currencyCode, baseCurrencyCode);
        }

        var convertedRate = entryCurrency.Rate / entryBase.Rate;
    
        return new CurrencyRateDto(currencyCode, RoundValue(convertedRate, precision));
    }

    public async Task<CurrencyRateDto> GetCurrencyOnDateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var entryCurrency = await _db.CurrencyCacheEntries
            .Where(e => e.BaseCurrency == CacheBase &&
                        e.Currency == currencyCode &&
                        e.TargetDate == date)
            .OrderByDescending(e => e.CachedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (entryCurrency is null)
        {
            var allRates = await _currencyApiService.GetAllCurrenciesOnDateAsync(CacheBase, date, cancellationToken);

            var now = DateTime.UtcNow;
            
            foreach (var rate in allRates.Rates)
            {
                var newEntry = new CurrencyCacheEntry
                {
                    BaseCurrency = CacheBase,
                    Currency = rate.Code,
                    Rate = rate.Rate,
                    CachedAt = now,
                    TargetDate = date
                };
                
                await _db.CurrencyCacheEntries.AddAsync(newEntry, cancellationToken);
            }
           
            await _db.SaveChangesAsync(cancellationToken);

            entryCurrency = await _db.CurrencyCacheEntries
                .Where(e => e.BaseCurrency == CacheBase &&
                            e.Currency == currencyCode &&
                            e.TargetDate == date)
                .OrderByDescending(e => e.CachedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var precision = _currencySettingsMonitor.CurrentValue.CurrencyRoundCount;

        if (baseCurrencyCode == CacheBase)
        {
            if (entryCurrency is null)
            {
                throw new DataNotFoundException(currencyCode);
            }
            
            return new CurrencyRateDto(currencyCode, RoundValue(entryCurrency.Rate, precision));
        }
    
        var entryBase = await _db.CurrencyCacheEntries
            .Where(e => e.BaseCurrency == CacheBase &&
                        e.Currency == baseCurrencyCode &&
                        e.TargetDate == date)
            .OrderByDescending(e => e.CachedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (entryCurrency is null || entryBase is null)
        {
            throw new DataNotFoundException(currencyCode, baseCurrencyCode);
        }

        var convertedRate = entryCurrency.Rate / entryBase.Rate;
    
        return new CurrencyRateDto(currencyCode, RoundValue(convertedRate, precision));
    }

    public async Task<MinimalSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await _currencyApiService.GetSettingsAsync(cancellationToken);
        
        return new MinimalSettingsDto(settings.RequestLimit > settings.UsedRequestCount);
    }
    
    private static decimal RoundValue(decimal value, int precision)
    {
        return Math.Round(value, precision);
    }
}