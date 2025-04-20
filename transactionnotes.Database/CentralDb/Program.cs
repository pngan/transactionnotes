using Dapper;
using Microsoft.Extensions.Hosting;

namespace CentralDb;

using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .MigrateDatabase<Program>();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => {
              //  services.AddSingleton<ISomeService, SomeService>(); // Register a service
                // Add other services and configurations here
            })
            .ConfigureAppConfiguration((hostContext, config) => {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); // Load from appsettings.json
                // Load other configurations here
            });
    }

}