using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая основные текущие настройки приложения
/// </summary>
/// <param name="NewRequestsAvailable">Есть ли ещё запросы ко внешнему API</param>
public record MinimalSettingsDto(
    [property: JsonPropertyName("newRequestsAvailable")] bool NewRequestsAvailable
);