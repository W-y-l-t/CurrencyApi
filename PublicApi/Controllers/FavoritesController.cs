using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

[Route("favorites")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteCurrencyApiService _favoriteApiService;
    private readonly ICurrencyApiService _currencyApiService;

    public FavoritesController(IFavoriteCurrencyApiService favoriteApiService, ICurrencyApiService currencyApiService)
    {
        _favoriteApiService = favoriteApiService;
        _currencyApiService = currencyApiService;
    }

    /// <summary>
    /// Вернуть избранную торговую пару по наименованию
    /// </summary>
    /// <param name="name">Наименование торговой пары</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Успешно возвращает избранную торговую пару</response>
    /// <response code="424">Не существует торговой пары с таким наименованием</response>
    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<FavoriteCurrencyDto>> GetFavorite(
        [FromRoute] string name, 
        CancellationToken cancellationToken)
    {
        var favorite = await _favoriteApiService.GetFavoriteByNameAsync(name, cancellationToken);
        
        return Ok(favorite);
    }

    /// <summary>
    /// Вернуть все избранные торговые пары
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Успешно возвращает избранные торговые пары</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FavoriteCurrencyDto>>> GetAllFavorites(
        CancellationToken cancellationToken)
    {
        var favorites = await _favoriteApiService.GetAllFavoritesAsync(cancellationToken);
        
        return Ok(favorites);
    }

    /// <summary>
    /// Добавить торговую пару в избранное
    /// </summary>
    /// <param name="favorite">Новая торговая пара</param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Успешно добавляет торговую пару в избранное</response>
    /// <response code="417">Аналогичная торговая пара уже находится в избранном</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FavoriteCurrencyDto))]
    [ProducesResponseType(StatusCodes.Status417ExpectationFailed, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> AddFavorite(
        [FromBody] FavoriteCurrencyDto favorite, 
        CancellationToken cancellationToken)
    {
        await _favoriteApiService.AddFavoriteAsync(favorite, cancellationToken);
        
        return CreatedAtAction(nameof(GetFavorite), new { name = favorite.Name }, favorite);
    }

    /// <summary>
    /// Обновить избранную торговую пару
    /// </summary>
    /// <param name="name">Наименование торговой пары</param>
    /// <param name="favorite">Обновлённая торговая пара</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Успешно обновляет торговую пару</response>
    /// <response code="417">Аналогичная торговая пара уже находится в избранном</response>
    /// <response code="424">Не существует торговой пары с таким наименованием</response>
    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status417ExpectationFailed, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateFavorite(
        [FromRoute] string name, 
        [FromBody] FavoriteCurrencyDto favorite, 
        CancellationToken cancellationToken)
    {
        await _favoriteApiService.UpdateFavoriteAsync(name, favorite, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Удалить избранную торговую пару по наименованию
    /// </summary>
    /// <param name="name">Наименование торговой пары</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Успешно удаляет избранную торговую пару</response>
    /// <response code="424">Не существует торговой пары с таким наименованием</response>
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteFavorite([FromRoute] string name, CancellationToken cancellationToken)
    {
        await _favoriteApiService.DeleteFavoriteAsync(name, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Вернуть курс валюты для избранной торговой пары (по её наименованию)
    /// </summary>
    /// <param name="name">Наименование торговой пары</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Успешно возвращает избранную торговую пару</response>
    /// <response code="424">Не существует торговой пары с таким наименованием</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="429">Исчерпан лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    /// <response code="503">Ошибка при взаимодействии с RPC</response>
    [HttpGet("{name}/rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateDto>> GetFavoriteRate(
        [FromRoute] string name, 
        CancellationToken cancellationToken)
    {
        var favorite = await _favoriteApiService.GetFavoriteByNameAsync(name, cancellationToken);
        
        var rate = await _currencyApiService.GetCurrencyRateAsync(
            favorite.BaseCurrency, 
            favorite.Currency, 
            cancellationToken);

        return Ok(rate);
    }

    /// <summary>
    /// Вернуть курс валюты на определённую дату для избранной торговой пары (по её наименованию)
    /// </summary>
    /// <param name="name">Наименование торговой пары</param>
    /// <param name="date">Дата в формате yyyy-MM-dd</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Успешно возвращает избранную торговую пару</response>
    /// <response code="424">Не существует торговой пары с таким наименованием</response>
    /// <response code="400">Некорректный формат даты</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="429">Превышен лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    /// <response code="503">Ошибка при взаимодействии с RPC</response>
    [HttpGet("{name}/rate/{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ObjectResult))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateOnDateDto>> GetFavoriteRateByDate(
        [FromRoute] string name, 
        [FromRoute] DateOnly date, 
        CancellationToken cancellationToken)
    {
        var favorite = await _favoriteApiService.GetFavoriteByNameAsync(name, cancellationToken);

        var rate = await _currencyApiService.GetCurrencyRateOnDateAsync(
            favorite.BaseCurrency,
            favorite.Currency, 
            date, 
            cancellationToken);

        return Ok(rate);
    }
}