using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using DbUp;
using DbUp.Engine;
using DbUp.ScriptProviders;
using Npgsql;

namespace centraldb_migration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration for {DatabaseName}", "centralDB");

        string connectionAdmin = configuration.GetValue<string>("ConnectionStrings:centraldb");
        var dbMatch = Regex.Match(connectionAdmin, @"Database=(.*)$");
        string databaseName = dbMatch.Success ? dbMatch.Groups[1].Value: string.Empty;

        using var connection = new NpgsqlConnection(connectionAdmin);
        await connection.OpenAsync(stoppingToken);

        const string query = "SELECT 1 FROM pg_roles WHERE rolname = @RoleName";
        var roleExists = await connection.QueryFirstOrDefaultAsync<int?>(query, new { RoleName = "schema_owner" });

        var migrationuserUsername = configuration.GetValue<string>("Database:Central:MigrationUser:Username");
        var migrationuserPassword = configuration.GetValue<string>("Database:Central:MigrationUser:Password");

        if (roleExists.HasValue)
        {
            logger.LogWarning("Role 'schema_owner' exists in the database. ");
        }
        else
        {
            logger.LogWarning("Creating role 'schema_owner'.");
            var sqlCommands =
                $"CREATE ROLE schema_owner NOLOGIN;" +
                $"GRANT CREATE ON DATABASE {databaseName} TO schema_owner;" +
                $"CREATE SCHEMA central AUTHORIZATION schema_owner;" +
                $"CREATE ROLE {migrationuserUsername} LOGIN PASSWORD '{migrationuserPassword}';" +
                $"GRANT schema_owner TO {migrationuserUsername};" +
                $"CREATE ROLE migration_runner NOLOGIN;" +
                $"GRANT USAGE ON SCHEMA public TO migration_runner;" +
                $"GRANT CREATE ON SCHEMA public TO migration_runner;" +
                $"GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO migration_runner;" +
                $"GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO schema_owner;" +
                $"GRANT migration_runner TO {migrationuserUsername};";

            await connection.ExecuteAsync(sqlCommands);
            logger.LogInformation("Roles and schema created successfully.");
        }
        await connection.CloseAsync();


        var tmp = Regex.Replace(connectionAdmin,
            "Username=.*;Password",
            $"Username={migrationuserUsername};Password");
        var connectionMigration = Regex.Replace(tmp, "Password=.*;Database", $"Password={migrationuserPassword};Database");

        var options = new FileSystemScriptOptions
        {
            // Patterns to search the file system for. Set to "*.sql" by default.
            Extensions = new[] { "*.psql" },
        };

        var schemaUpgrader = DeployChanges.To
            .PostgresqlDatabase(connectionMigration)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                code => code.StartsWith("centraldb_migration.SqlScripts.SchemaOwner.")
            )
            .LogToConsole()
            .Build();

        var result = schemaUpgrader.PerformUpgrade();
        
        if (!result.Successful)
        {
            logger.LogError(result.Error, "An error occurred while setting up the PostgreSQL database {DatabaseName}", databaseName);
        }
        else
        {
            logger.LogInformation("Successfully migrated PostgreSQL database {DatabaseName}", databaseName);
        }

        Program.RequiredService.StopApplication();
    }
}
