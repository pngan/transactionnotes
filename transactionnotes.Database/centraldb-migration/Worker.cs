namespace centraldb_migration;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Running centraldb-migration");
        await Task.Delay(1000, stoppingToken);
        _logger.LogInformation("Stopping centraldb-migration");

        Program.RequiredService.StopApplication();
    }
}
