using Fuse8.BackendInternship.InternalApi.Contracts.Services;

namespace Fuse8.BackendInternship.InternalApi.Services.CacheWarmupServices;

public class CacheWarmupService : BackgroundService
{
    private readonly ILogger<CacheWarmupService> _logger;

    public CacheWarmupService(ILogger<CacheWarmupService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        ServiceProvider = serviceProvider;
    }
    
    public IServiceProvider ServiceProvider { get; }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cache warmup service started.");
        
        await DoWork(cancellationToken);
        
        _logger.LogInformation("Cache warmup service stopping.");
    }
    
    private async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = ServiceProvider.CreateScope();
        
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

        await scopedProcessingService.DoWork(cancellationToken);
    }
}