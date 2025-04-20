using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CentralDb;

using Microsoft.Extensions.Configuration;

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