using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Модель ошибки, возвращаемой внешним API при статусе 422
/// </summary>
public class ErrorApiResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("errors")]
    public Dictionary<string, IList<string>>? Errors { get; set; }
    
    [JsonPropertyName("info")]
    public string? Info { get; set; }
}