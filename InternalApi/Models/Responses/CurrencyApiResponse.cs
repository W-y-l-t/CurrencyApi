using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Models.Responses;

/// <summary>
/// Модель ответа внешнего API на запросы получения курса валюты
/// </summary>
public class CurrencyApiResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, CurrencyItem>? Data { get; set; }
    
    public class CurrencyItem
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}