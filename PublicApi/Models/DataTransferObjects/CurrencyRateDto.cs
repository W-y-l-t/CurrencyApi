using System.Text.Json.Serialization;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая текущий курс валюты
/// </summary>
/// <param name="Code">Код валюты</param>
/// <param name="Rate">Числовое значение курса</param>
public record CurrencyRateDto(
    [property: JsonPropertyName("code")] CurrencyCode Code,
    [property: JsonPropertyName("rate")] decimal Rate
);