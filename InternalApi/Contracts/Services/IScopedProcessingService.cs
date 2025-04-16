namespace Fuse8.BackendInternship.InternalApi.Contracts.Services;

public interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}