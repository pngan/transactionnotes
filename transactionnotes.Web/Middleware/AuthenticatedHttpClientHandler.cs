using System.Net.Http.Headers;
using transactionnotes.Web.Services;

namespace transactionnotes.Web.Middleware
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public AuthenticatedHttpClientHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
