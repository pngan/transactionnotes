using Dapper;

namespace CentralDb;

using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {

        // Build the configuration
        var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory()) // Set the base path for configuration files
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load appsettings.json
            //.AddEnvironmentVariables() // Optionally add environment variables
            .Build();


        var connectionString = configuration.GetConnectionString("centraldbserver");

        // Pass the configuration to the repository
        var repo = new DataRepository(configuration);
        Thread.Sleep(5000);
        Console.WriteLine("Hello, World!");



    }

    public class DataRepository(IConfiguration config)
    {
        public async Task GetAsync(string id)
        {
            var connectionString = config.GetConnectionString("centraldbserver");

            await using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT 1";
            var result = await connection.QueryFirstOrDefaultAsync<int>(query);

            Console.WriteLine($"Query Result: {result}");
        }
    }
}
