using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.Support;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Npgsql;

namespace centraldb_migration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    public static string MigrationuserUsername;
    public static string MigrationuserPassword;
    public static string MigrationDatabaseName;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration for {DatabaseName}", "centralDB");

        string connectionAdmin = configuration.GetValue<string>("ConnectionStrings:centraldb");

        var adminUsername = configuration.GetValue<string>("Database:Central:Admin:Username");
        var adminPassword = configuration.GetValue<string>("Database:Central:Admin:Password");


        Match centralDatabaseName = Regex.Match(connectionAdmin, @"Database=(.*)$");
        if (centralDatabaseName.Success)
        {
            MigrationDatabaseName = centralDatabaseName.Groups[1].Value;
            // Use databaseName as needed  
        }

        MigrationuserUsername = configuration.GetValue<string>("Database:Central:MigrationUser:Username");
        MigrationuserPassword = configuration.GetValue<string>("Database:Central:MigrationUser:Password");



        using var connection = new NpgsqlConnection(connectionAdmin);
        await connection.OpenAsync(stoppingToken);

        const string query = "SELECT 1 FROM pg_roles WHERE rolname = @RoleName";
        var roleExists = await connection.QueryFirstOrDefaultAsync<int?>(query, new { RoleName = "schema_owner" });

        if (roleExists.HasValue)
        {
            logger.LogWarning("Role 'schema_owner' exists in the database. ");
        }
        else
        {
            logger.LogWarning("Role 'schema_owner' does not exist in the database.");

            var databaseName = Worker.MigrationDatabaseName;
            var username = Worker.MigrationuserUsername;
            var password = Worker.MigrationuserPassword;

            var sqlCommands =
                $"CREATE ROLE schema_owner NOLOGIN;" +
                $"GRANT CREATE ON DATABASE {databaseName} TO schema_owner;" +
                $"CREATE SCHEMA central AUTHORIZATION schema_owner;" +
                $"CREATE ROLE {username} LOGIN PASSWORD '{password}';" +
                $"GRANT schema_owner TO {username};" +
                $"CREATE ROLE migration_runner NOLOGIN;" +
                $"GRANT USAGE ON SCHEMA public TO migration_runner;" +
                $"GRANT CREATE ON SCHEMA public TO migration_runner;" +
                $"GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO migration_runner;" +
                $"GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO schema_owner;" +
                $"GRANT migration_runner TO {username};";

            await connection.ExecuteAsync(sqlCommands);
            logger.LogInformation("Roles and schema created successfully.");
        }

        await connection.CloseAsync();


        var tmp = Regex.Replace(connectionAdmin,
            "Username=.*;Password",
            $"Username={MigrationuserUsername};Password");
        var connectionMigration = Regex.Replace(tmp, "Password=.*;Database", $"Password={MigrationuserPassword};Database");

        var schemaUpgrader = DeployChanges.To
            .PostgresqlDatabase(connectionAdmin)
            .WithScriptsAndCodeEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                code => code.StartsWith("centraldb_migration.SqlScripts.SchemaOwner.")
            )
            .LogToConsole()
            .Build();
        var scrs = schemaUpgrader.GetScriptsToExecute();
        var result = schemaUpgrader.PerformUpgrade();
        if (!result.Successful)
        {
            logger.LogError(result.Error, "An error occurred while setting up the PostgreSQL database {DatabaseName}", "centralDB");
        }
        else
        {
            logger.LogInformation("Successfully migrated PostgreSQL database {DatabaseName}", "centralDB");
        }

        Program.RequiredService.StopApplication();
    }
}
