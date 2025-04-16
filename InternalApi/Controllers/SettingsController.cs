using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.InternalApi.Controllers;

/// <summary>
/// Методы для получения текущих настроек приложения
/// </summary>
[Route("settings")]
public class SettingsController : ControllerBase
{
    private readonly ICachedCurrencyApiService _cachedCurrencyApiService;

    public SettingsController(ICachedCurrencyApiService cachedCurrencyApiService)
    {
        _cachedCurrencyApiService = cachedCurrencyApiService;
    }

    /// <summary>
    /// Получить текущие настройки приложения
    /// </summary>
    /// <returns>
    /// Есть ли ещё запросы ко внешнему API
    /// </returns>
    /// <remarks>
    /// <response code="200">Успешно возвращает настройки</response>
    /// <response code="406">Некорректная структура HTTP запроса</response>
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<MinimalSettingsDto>> GetSettings(CancellationToken cancellationToken)
    {
        var settings = await _cachedCurrencyApiService.GetSettingsAsync(cancellationToken);
        
        return Ok(settings);
    }
}