using Aspire.Hosting;

namespace transactionnotes.Tests.Common;

public abstract class HttpTestBase(string resourceName) : IAsyncLifetime
{
    public const string ApiServiceResource = "apiservice";
    public const string WebFrontendResource = "webfrontend";
    protected HttpClient HttpClient { get; private set; } = null!;
    private ResourceNotificationService _resourceNotificationService = null!;
    protected IDistributedApplicationTestingBuilder AppHost { get; private set; } = null!;
    private DistributedApplication _app = null!;

    private string ResourceName { get; } = resourceName;

    public async ValueTask InitializeAsync()
    {
        AppHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.transactionnotes_AppHost>();
        AppHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await AppHost.BuildAsync();
        _resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync();

        // Act
        HttpClient = _app.CreateHttpClient(ResourceName);
        await _resourceNotificationService.WaitForResourceAsync(ResourceName, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(10));
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        await _app.DisposeAsync();
    }
}