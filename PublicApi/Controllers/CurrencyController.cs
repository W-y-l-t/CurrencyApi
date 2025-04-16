using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.PublicApi.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CurrencyCode = Fuse8.BackendInternship.PublicApi.Models.Types.CurrencyCode;
using DateOnly = System.DateOnly;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

/// <summary>
/// Методы для получения актуального курса валют и курса валют в произвольный день
/// </summary>
[Route("currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyApiService _currencyApiService;

    public CurrencyController(ICurrencyApiService currencyApiService)
    {
        _currencyApiService = currencyApiService;
    }

    /// <summary>
    /// Получить курс заданной валюты
    /// </summary>
    /// <param name="baseCurrencyCode">Базовый код валюты, относительно которой считается курс</param>
    /// <param name="currencyCode">Код валюты</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Вызывает соответствующий метод InternalApi для <paramref name="currencyCode"/>.
    /// </remarks>
    /// <response code="200">Успешно возвращает курс указанной валюты</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="429">Исчерпан лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    /// <response code="503">Ошибка при взаимодействии с RPC</response>
    [HttpGet("{baseCurrencyCode}/to/{currencyCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateDto>> GetCurrencyRate(
        [FromRoute] CurrencyCode baseCurrencyCode,
        [FromRoute] CurrencyCode currencyCode,
        CancellationToken cancellationToken)
    {
        var response =
            await _currencyApiService.GetCurrencyRateAsync(baseCurrencyCode, currencyCode, cancellationToken);
        
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
    /// Вызывает соответствующий метод InternalApi для <paramref name="currencyCode"/> и <paramref name="date"/>.
    /// </remarks>
    /// <response code="200">Успешно возвращает исторический курс валюты</response>
    /// <response code="400">Некорректный формат даты</response>
    /// <response code="422">Указанная валюта не найдена</response>
    /// <response code="429">Превышен лимит обращений к внешнему API</response>
    /// <response code="500">Ошибка при обращении ко внешнему API</response>
    /// <response code="503">Ошибка при взаимодействии с RPC</response>
    [HttpGet("{baseCurrencyCode}/to/{currencyCode}/on/{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ObjectResult))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<CurrencyRateOnDateDto>> GetCurrencyRateOnDate(
        [FromRoute] CurrencyCode baseCurrencyCode,
        [FromRoute] CurrencyCode currencyCode, 
        [FromRoute] DateOnly date,
        CancellationToken cancellationToken)
    {
        var response = await _currencyApiService.GetCurrencyRateOnDateAsync(
            baseCurrencyCode, 
            currencyCode, 
            date, 
            cancellationToken);

        return Ok(response);
    }
}