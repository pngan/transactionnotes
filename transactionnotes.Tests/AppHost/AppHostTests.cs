using transactionnotes.Tests.Common;

namespace transactionnotes.Tests.AppHost
{
    public class AppHostTests()
    {
        private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(120);

        [Fact]
        public async Task AppHostRunsCleanly()
        {
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.transactionnotes_AppHost>();
            await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);
            var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
            await app.StartAsync().WaitAsync(StartStopTimeout);

            await resourceNotificationService.WaitForResourceAsync(HttpTestBase.ApiServiceResource, KnownResourceStates.Running).WaitAsync(StartStopTimeout);
            await resourceNotificationService.WaitForResourceAsync(HttpTestBase.WebFrontendResource, KnownResourceStates.Running).WaitAsync(StartStopTimeout);
            await app.StopAsync().WaitAsync(BuildStopTimeout);
        }
    }
}
