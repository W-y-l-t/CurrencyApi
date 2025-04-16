using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.DataAccess.DbContexts;
using Fuse8.BackendInternship.PublicApi.Exceptions.DataBaseExceptions;
using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.PublicApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Services;

public class FavoriteCurrencyApiService : IFavoriteCurrencyApiService
{
    private readonly FavoriteCurrencyContext _db;

    public FavoriteCurrencyApiService(FavoriteCurrencyContext db)
    {
        _db = db;
    }

    public async Task<FavoriteCurrencyDto> GetFavoriteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await _db.FavoriteCurrencies
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (entity is null)
        {
            throw new DataNotFoundException($"Favorite with name '{name}' not found.");
        }
        
        return new FavoriteCurrencyDto(entity.Name, entity.Currency, entity.BaseCurrency);
    }

    public async Task<IEnumerable<FavoriteCurrencyDto>> GetAllFavoritesAsync(CancellationToken cancellationToken)
    {
        var list = await _db.FavoriteCurrencies.ToListAsync(cancellationToken);

        return list.Select(e => new FavoriteCurrencyDto(e.Name, e.Currency, e.BaseCurrency));
    }

    public async Task AddFavoriteAsync(FavoriteCurrencyDto favorite, CancellationToken cancellationToken)
    {
        if (await _db.FavoriteCurrencies.AnyAsync(x => x.Name == favorite.Name, cancellationToken))
        {
            throw new DataAlreadyExistsException($"Favorite with name '{favorite.Name}' already exists.");
        }
        
        if (await _db.FavoriteCurrencies.AnyAsync(x => x.Currency == favorite.Currency &&
                                                       x.BaseCurrency == favorite.BaseCurrency, cancellationToken))
        {
            throw new DataAlreadyExistsException($"Favorite for currency '{favorite.Currency}' " +
                                                 $"with base '{favorite.BaseCurrency}' already exists.");
        }

        var entity = new FavoriteCurrency
        {
            Name = favorite.Name,
            Currency = favorite.Currency,
            BaseCurrency = favorite.BaseCurrency
        };

        await _db.FavoriteCurrencies.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateFavoriteAsync(
        string name, 
        FavoriteCurrencyDto favorite, 
        CancellationToken cancellationToken)
    {
        var entity = await _db.FavoriteCurrencies.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (entity is null)
        {
            throw new DataNotFoundException($"Favorite with name '{name}' not found.");
        }
        
        if ((entity.Currency != favorite.Currency || entity.BaseCurrency != favorite.BaseCurrency) &&
            await _db.FavoriteCurrencies.AnyAsync(x => x.Currency == favorite.Currency &&
                                                       x.BaseCurrency == favorite.BaseCurrency, cancellationToken))
        {
            throw new DataAlreadyExistsException($"Favorite for currency '{favorite.Currency}' " +
                                                 $"with base '{favorite.BaseCurrency}' already exists.");
        }

        entity.Name = favorite.Name;
        entity.Currency = favorite.Currency;
        entity.BaseCurrency = favorite.BaseCurrency;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteFavoriteAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await _db.FavoriteCurrencies.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (entity is null)
        {
            throw new DataNotFoundException($"Favorite with name '{name}' not found.");
        }

        _db.FavoriteCurrencies.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}