using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CentralDb;

public static class DatabaseExtension
{
    public static IHostApplicationBuilder MigrateDatabase<TContext>(this IHostApplicationBuilder host)
    {
        IServiceProvider serviceProvider = host.Services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<TContext>>();

        logger.LogInformation("Starting database migration for {DatabaseName}", "centralDB");

        string connection = configuration.GetValue<string>("ConnectionStrings:centraldb");

        EnsureDatabase.For.PostgresqlDatabase(connection);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connection)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            //.LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "An error occurred while migrating the PostgreSQL database {DatabaseName}", "centralDB");
            return host;
        }

        logger.LogInformation("Successfully migrated PostgreSQL database {DatabaseName}", "centralDB");

        return host;
    }

}