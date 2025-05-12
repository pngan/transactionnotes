using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace transactionnotes.Web.Middleware
{
    /// <summary>
    /// DelegatingHandler that throws an exception if the response status code is not successful.
    /// </summary>
    public class ApiErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Backend returned error: {(int)response.StatusCode} {response.ReasonPhrase}\n{content}", null, response.StatusCode);
            }
            return response;
        }
    }
}
