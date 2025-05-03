using System.Reflection;
using System.Text.RegularExpressions;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.Support;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;

namespace centraldb_migration;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    public static string MigrationuserUsername;
    public static string MigrationuserPassword;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration for {DatabaseName}", "centralDB");

        string connectionAdmin = configuration.GetValue<string>("ConnectionStrings:centraldb");

        var adminUsername = configuration.GetValue<string>("Database:Central:Admin:Username");
        var adminPassword = configuration.GetValue<string>("Database:Central:Admin:Password");
        MigrationuserUsername = configuration.GetValue<string>("Database:Central:MigrationUser:Username");
        MigrationuserPassword = configuration.GetValue<string>("Database:Central:MigrationUser:Password");

        var adminUpgrader = DeployChanges.To
            .PostgresqlDatabase(connectionAdmin)
            .WithScriptsAndCodeEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    //script => false,
                    code => code.StartsWith("centraldb_migration.SqlScripts.Admin.")
                    //new SqlScriptOptions { ScriptType = ScriptType.RunAlways }
                    )
            .LogToConsole()
            .JournalTo(new NullJournal())
            .Build();
        var scr  = adminUpgrader.GetScriptsToExecute();
        var result = adminUpgrader.PerformUpgrade();
        

        if (!result.Successful)
        {

            logger.LogError(result.Error, "An error occurred while setting up the PostgreSQL database {DatabaseName}", "centralDB");
        }
        else
        {




            var tmp = Regex.Replace(connectionAdmin,
                "Username=.*;Password",
                $"Username={MigrationuserUsername};Password");
            var connectionMigration = Regex.Replace(tmp, "Password=.*;Database", $"Password={MigrationuserPassword};Database");

            var schemaUpgrader = DeployChanges.To
                .PostgresqlDatabase(connectionMigration)
                .WithScriptsAndCodeEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    code => code.StartsWith("centraldb_migration.SqlScripts.SchemaOwner.")
                )
                .LogToConsole()
                .JournalTo(new NullJournal())
                .Build();
            var scrs = schemaUpgrader.GetScriptsToExecute();
            var result2  = schemaUpgrader.PerformUpgrade();
            if (!result.Successful)
            {
                logger.LogError(result.Error, "An error occurred while setting up the PostgreSQL database {DatabaseName}", "centralDB");
            }
            else
            {
                logger.LogInformation("Successfully migrated PostgreSQL database {DatabaseName}", "centralDB");
            }
        }

        Program.RequiredService.StopApplication();
    }
}
