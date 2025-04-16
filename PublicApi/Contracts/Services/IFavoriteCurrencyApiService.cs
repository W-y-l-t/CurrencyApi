using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;

namespace Fuse8.BackendInternship.PublicApi.Contracts.Services;

public interface IFavoriteCurrencyApiService
{
    Task<FavoriteCurrencyDto> GetFavoriteByNameAsync(string name, CancellationToken cancellationToken);
    
    Task<IEnumerable<FavoriteCurrencyDto>> GetAllFavoritesAsync(CancellationToken cancellationToken);
    
    Task AddFavoriteAsync(FavoriteCurrencyDto favorite, CancellationToken cancellationToken);
    
    Task UpdateFavoriteAsync(string name, FavoriteCurrencyDto favorite, CancellationToken cancellationToken);
    
    Task DeleteFavoriteAsync(string name, CancellationToken cancellationToken);
}