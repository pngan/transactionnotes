using transactionnotes.Tests.Common;

namespace transactionnotes.Tests.AppHost
{
    public class AppHostTests()
    {
        private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ResourceTimeout = TimeSpan.FromSeconds(30);
        private const int MaxRetries = 3;
        private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(10);

        [Fact]
        public async Task AppHostRunsCleanly()
        {
            await ExecuteWithRetryAsync(async () =>
            {
                var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.transactionnotes_AppHost>();
                appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
                {
                    clientBuilder.AddStandardResilienceHandler();
                });

                await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);
                var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
                await app.StartAsync().WaitAsync(StartStopTimeout);

                await resourceNotificationService.WaitForResourceAsync(HttpTestBase.ApiServiceResource, KnownResourceStates.Running).WaitAsync(ResourceTimeout);
                await resourceNotificationService.WaitForResourceAsync(HttpTestBase.WebFrontendResource, KnownResourceStates.Running).WaitAsync(ResourceTimeout);
                await app.StopAsync().WaitAsync(BuildStopTimeout);
            });
        }

        private static async Task ExecuteWithRetryAsync(Func<Task> operation)
        {
            Exception? lastException = null;
            
            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                try
                {
                    await operation();
                    return; // Success, exit retry loop
                }
                catch (Exception ex) when (attempt < MaxRetries - 1)
                {
                    lastException = ex;
                    var delay = TimeSpan.FromMilliseconds(InitialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                    await Task.Delay(delay);
                }
            }
            
            // If we get here, all retries failed
            throw lastException ?? new InvalidOperationException("All retry attempts failed");
        }
    }
}
