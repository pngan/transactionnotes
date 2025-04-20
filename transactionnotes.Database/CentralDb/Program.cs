using Dapper;

namespace CentralDb;

using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__centraldb");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'ConnectionStrings__centraldb' not found in environment variables.");
        }

        // Pass the configuration to the repository
        var repo = new DataRepository(connectionString);
        int? res = await repo.GetAsync<int>("hi");
        Console.WriteLine($"Result = {res}");
    }

    public class DataRepository(string connectionString)
    {
        public async Task<T?> GetAsync<T>(string id)
        {
            await using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = "SELECT 1";
            var result = await connection.QueryFirstOrDefaultAsync<T>(query);

            Console.WriteLine($"Query Result: {result}");
            return result;
        }
    }
}
