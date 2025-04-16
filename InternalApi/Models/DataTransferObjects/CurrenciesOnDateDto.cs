using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая исторический курс валют относительно базовой на определённую дату
/// </summary>
/// <param name="Date">Дата в формате yyyy-MM-dd</param>
/// <param name="Rates">Курсы валют относительно базовой</param>
public record CurrenciesOnDateDto(
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("rates")] IEnumerable<CurrencyRateDto> Rates
);