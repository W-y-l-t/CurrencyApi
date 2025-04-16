using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.InternalApi.Models.Types;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.InternalApi.Controllers;

/// <summary>
/// Методы для получения актуального курса валют и курса валют в произвольный день (использует кеширование)
/// </summary>
[Route("currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICachedCurrencyApiService _cachedCurrencyApiService;

    public CurrencyController(ICachedCurrencyApiService cachedCurrencyApiService)
    {
        _cachedCurrencyApiService = cachedCurrencyApiService;
    }

    /// <summary>
    /// Получить курс заданной валюты
    /// </summary>
    /// <param name="baseCurrencyCode">Базовый код валюты, относительно которой считается курс</param>
    /// <param name="currencyCode">Код валюты</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Вызывает метод внешнего API <c>/v3/latest</c> для конкретного <paramref name="currencyCode"/>.
    /// </remarks>
    /// <response code="200">Успешно возвращает курс указанной валюты</response>
    /// <response code="406">Некорректная структура HTTP запроса</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="424">Ошибка при получении курса валюты из базы данных</response>
    /// <response code="429">Исчерпан лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    [HttpGet("{baseCurrencyCode}/to/{currencyCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateDto>> GetCurrentCurrencyRate(
        [FromRoute] CurrencyCode baseCurrencyCode,
        [FromRoute] CurrencyCode currencyCode,
        CancellationToken cancellationToken)
    {
        var response = await _cachedCurrencyApiService.GetCurrentCurrencyAsync(
            baseCurrencyCode, 
            currencyCode, 
            cancellationToken);
    
        return Ok(response);
    }

    /// <summary>
    /// Получить курс заданной валюты на определённую дату
    /// </summary>
    /// <param name="baseCurrencyCode">Базовый код валюты, относительно которой считается курс</param>
    /// <param name="currencyCode">Код валюты</param>
    /// <param name="date">Дата в формате yyyy-MM-dd</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Вызывает метод внешнего API <c>/v3/historical</c> для <paramref name="currencyCode"/> и <paramref name="date"/>.
    /// </remarks>
    /// <response code="200">Успешно возвращает исторический курс валюты</response>
    /// <response code="406">Некорректная структура HTTP запроса</response>
    /// <response code="400">Некорректный формат даты</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="424">Ошибка при получении курса валюты из базы данных</response>
    /// <response code="429">Превышен лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    [HttpGet("{baseCurrencyCode}/to/{currencyCode}/on/{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ObjectResult))]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateDto>> GetCurrencyRateOnDate(
        [FromRoute] CurrencyCode baseCurrencyCode,
        [FromRoute] CurrencyCode currencyCode,
        [FromRoute] DateOnly date,
        CancellationToken cancellationToken)
    {
        var response = await _cachedCurrencyApiService.GetCurrencyOnDateAsync(
            baseCurrencyCode,
            currencyCode,
            date,
            cancellationToken);

        return Ok(response);
    }
}
