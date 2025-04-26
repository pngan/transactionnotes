using centraldb_migration;

public class Program
{
    internal static IHostApplicationLifetime RequiredService = null!;

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();

        RequiredService = host.Services.GetRequiredService<IHostApplicationLifetime>();

        await host.RunAsync();
    }
}
