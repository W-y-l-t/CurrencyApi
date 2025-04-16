using System.Text.Json.Serialization;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;

/// <summary>
/// Модель, описывающая избранные пары для отображения курса 
/// </summary>
/// <param name="Name">Наименование торговой пары</param>
/// <param name="Currency">Валюта, курс которой будет получен</param>
/// <param name="BaseCurrency">Базовая валюта, относительно которой будет считаться курс</param>
public record FavoriteCurrencyDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("currency")] CurrencyCode Currency,
    [property: JsonPropertyName("baseCurrency")] CurrencyCode BaseCurrency
);