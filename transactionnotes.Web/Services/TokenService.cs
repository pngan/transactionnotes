using Microsoft.AspNetCore.Authentication;

namespace transactionnotes.Web.Services
{
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await _httpContextAccessor.HttpContext!
                .GetTokenAsync("access_token");
            return token ?? string.Empty;
        }
    }
}
