using Aspire.Hosting;

namespace transactionnotes.Tests.Common;

public abstract class HttpTestBase(string resourceName) : IAsyncLifetime
{
    protected const string ApiServiceResource = "apiservice";
    protected const string WebFrontendResource = "webfrontend";
    protected HttpClient HttpClient { get; private set; } = null!;
    private ResourceNotificationService _resourceNotificationService = null!;
    private IDistributedApplicationTestingBuilder _appHost = null!;
    private DistributedApplication _app = null!;

    private string ResourceName { get; } = resourceName;

    public async Task InitializeAsync()
    {
        _appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.transactionnotes_AppHost>();
        _appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await _appHost.BuildAsync();
        _resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync();

        // Act
        HttpClient = _app.CreateHttpClient(ResourceName);
        await _resourceNotificationService.WaitForResourceAsync(ResourceName, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await _app.DisposeAsync();
    }
}