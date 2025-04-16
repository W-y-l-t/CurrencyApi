using System.Text.Json.Serialization;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая исторический курс валюты на определённую дату
/// </summary>
/// <param name="Code">Код валюты</param>
/// <param name="Rate">Значение курса на момент даты</param>
/// <param name="Date">Дата в формате yyyy-MM-dd</param>
public record CurrencyRateOnDateDto(
    [property: JsonPropertyName("code")] CurrencyCode Code,
    [property: JsonPropertyName("rate")] decimal Rate,
    [property: JsonPropertyName("date")] DateOnly Date
);