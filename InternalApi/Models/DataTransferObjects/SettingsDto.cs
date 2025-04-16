using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая текущие настройки приложения
/// </summary>
/// <param name="RequestLimit">Лимит запросов в месяц</param>
/// <param name="UsedRequestCount">Сколько запросов уже сделано</param>
/// <param name="CurrencyRoundCount">Количество знаков после запятой при округлении</param>
public record SettingsDto(
    [property: JsonPropertyName("requestLimit")] int RequestLimit,
    [property: JsonPropertyName("usedRequestCount")] int UsedRequestCount,
    [property: JsonPropertyName("currencyRoundCount")] int CurrencyRoundCount
);