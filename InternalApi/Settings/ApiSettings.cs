namespace Fuse8.BackendInternship.InternalApi.Settings;

public sealed record ApiSettings
{
    public string ApiKey { get; init; }
    
    public string BaseUrl { get; init; }
}