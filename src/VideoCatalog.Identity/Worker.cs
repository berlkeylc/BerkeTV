namespace VideoCatalog.Identity;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Identity Service is starting.");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Data.IdentityDbContext>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync(stoppingToken);
        
        _logger.LogInformation("Identity Service database initialized.");
    }
}
