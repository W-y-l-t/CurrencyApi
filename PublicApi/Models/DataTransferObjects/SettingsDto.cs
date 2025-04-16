using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая текущие настройки приложения
/// </summary>
/// <param name="NewRequestsAvailable">Есть ли ещё запросы ко внешнему API</param>
/// <param name="CurrencyRoundCount">Количество знаков после запятой при округлении</param>
public record SettingsDto(
    [property: JsonPropertyName("newRequestsAvailable")] bool NewRequestsAvailable,
    [property: JsonPropertyName("currencyRoundCount")] int CurrencyRoundCount
);