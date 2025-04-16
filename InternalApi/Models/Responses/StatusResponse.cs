using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Models.Responses;

/// <summary>
/// Модель ответа внешнего API на запросы получения текущих параметров
/// </summary>
public class StatusResponse
{
    [JsonPropertyName("quotas")]
    public QuotasClass? Quotas { get; set; }

    public class QuotasClass
    {
        [JsonPropertyName("month")]
        public Month? Month { get; set; }
    }

    public class Month
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        
        [JsonPropertyName("used")]
        public int Used { get; set; }
    }
}