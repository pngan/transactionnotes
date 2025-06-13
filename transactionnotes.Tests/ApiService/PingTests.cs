using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using IdentityModel;
using IdentityModel.Client;
using transactionnotes.Tests.Common;

namespace transactionnotes.Tests.ApiService
{
    public class PingTests() : HttpTestBase(ApiServiceResource)
    {
        [Fact]
        public async Task SimplePingShouldReturnPong()
        {
            // Act
            var response = await HttpClient.GetAsync("/api/v1/ping");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Pong", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SecurePingShouldReturnSecurePongWithAuth()
        {
            // Arrange
            // Setup DI
            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());

            // Add HttpClient factory
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            try
            {
                // Configure these settings based on your OIDC provider
                var oidcServerUrl = "https://auth.nganfamily.com/realms/transactionnotes";
                var clientId = AppHost.Configuration["TransNotes:ClientId"];
                var clientSecret = AppHost.Configuration["TransNotes:ClientSecret"];
                var scope = "openid email profile"; // Add required scopes

                // For username/password flow (resource owner password)
                var username = "phillip.ngan+tn-regular@gmail.com";
                var password = "pass";

                var userJwtToken = await GetPasswordTokenAsync(
                    httpClientFactory,
                    oidcServerUrl,
                    clientId,
                    clientSecret,
                    username,
                    password,
                    scope,
                    logger);
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userJwtToken);

                logger.LogInformation($"User JWT Token: {userJwtToken}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obtaining JWT token");
            }

            // Act
            var response = await HttpClient.GetAsync("/api/v1/ping/secure");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Secure Pong", await response.Content.ReadAsStringAsync());
        }

        static async Task<string> GetPasswordTokenAsync(
            IHttpClientFactory httpClientFactory,
            string authority,
            string clientId,
            string clientSecret,
            string username,
            string password,
            string scope,
            ILogger logger)
        {
            logger.LogInformation("Retrieving JWT token using password flow...");

            var client = httpClientFactory.CreateClient();

            // Discover endpoints from metadata
            var disco = await client.GetDiscoveryDocumentAsync(authority);
            if (disco.IsError)
            {
                logger.LogError($"Discovery error: {disco.Error}");
                throw new Exception($"Discovery error: {disco.Error}");
            }

            // Request token with username and password
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                UserName = username,
                Password = password,
                Scope = scope,
                GrantType = OidcConstants.GrantTypes.ClientCredentials // For this to work keycloak client must be configured to allow Direct Grant in Keycloak
                                                                       // This is reduced security, but we should be look at using a more secure flow in production

            });

            if (tokenResponse.IsError)
            {
                logger.LogError($"Token error: {tokenResponse.Error}");
                throw new Exception($"Token error: {tokenResponse.Error}");
            }

            return tokenResponse.AccessToken;
        }
    }
}
