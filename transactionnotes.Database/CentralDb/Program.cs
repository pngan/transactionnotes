using Microsoft.Extensions.Hosting;

namespace CentralDb;

class Program
{
    static async Task Main(string[] args)
    {
        IHostApplicationBuilder builder = CreateHostBuilder(args)
            .AddServiceDefaults()
            .MigrateDatabase<Program>();

    }

    public static IHostApplicationBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateApplicationBuilder(args);
    }

}