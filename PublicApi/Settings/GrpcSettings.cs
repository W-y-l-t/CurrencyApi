namespace Fuse8.BackendInternship.PublicApi.Settings;

public sealed record GrpcSettings
{
    public string InternalApiUrl { get; init; }
}