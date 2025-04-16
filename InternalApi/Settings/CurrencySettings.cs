using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.InternalApi.Settings;

public sealed record CurrencySettings
{
    [Required]
    public int CurrencyRoundCount { get; init; }
}