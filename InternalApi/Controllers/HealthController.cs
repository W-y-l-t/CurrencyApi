using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.InternalApi.Controllers;

/// <summary>
/// Методы для проверки работоспособности API
/// </summary>
[Route("health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health Check endpoint для проверки работоспособности REST‑API.
    /// </summary>
    /// <response code="200">Сервис работает</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Health() => Ok("Healthy");
}