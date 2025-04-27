using System.Reflection;
using DbUp;

namespace centraldb_migration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration for {DatabaseName}", "centralDB");

        string connection = configuration.GetValue<string>("ConnectionStrings:centraldb");

        EnsureDatabase.For.PostgresqlDatabase(connection);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connection)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "An error occurred while migrating the PostgreSQL database {DatabaseName}", "centralDB");
        }
        else
        {
            logger.LogInformation("Successfully migrated PostgreSQL database {DatabaseName}", "centralDB");
        }

        Program.RequiredService.StopApplication();
    }
}
