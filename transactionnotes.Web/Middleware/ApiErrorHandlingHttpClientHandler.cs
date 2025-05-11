using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using transactionnotes.Web.Services;

namespace transactionnotes.Web.Middleware
{
    /// <summary>
    /// DelegatingHandler that intercepts 401/403 responses and calls ErrorHandlingService.HandleErrorAsync.
    /// </summary>
    public class ApiErrorHandlingHttpClientHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ApiErrorHandlingHttpClientHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Resolve ErrorHandlingService from DI
                var errorService = _serviceProvider.GetRequiredService<ErrorHandlingService>();
                await errorService.HandleErrorAsync(response, request.RequestUri?.ToString() ?? "ApiCall");
            }

            return response;
        }
    }
}
