using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

/// <summary>
/// Методы для получения текущих настроек приложения
/// </summary>
[Route("settings")]
public class SettingsController : ControllerBase
{
    private readonly ICurrencyApiService _currencyApiService;

    public SettingsController(ICurrencyApiService currencyApiService)
    {
        _currencyApiService = currencyApiService;
    }

    /// <summary>
    /// Получить текущие настройки приложения
    /// </summary>
    /// <remarks>
    /// Возвращает количество знаков после запятой, до которого следует округлять значение курса валют
    /// </remarks>
    /// <response code="200">Успешно возвращает настройки</response>
    /// <response code="503">Ошибка при взаимодействии с RPC</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ProblemDetails))]
    [HttpGet]
    public async Task<ActionResult<SettingsDto>> GetSettings(CancellationToken cancellationToken)
    {
        var settings = await _currencyApiService.GetSettingsAsync(cancellationToken);
        
        return Ok(settings);
    }
}