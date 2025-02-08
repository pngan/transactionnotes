using System.IdentityModel.Tokens.Jwt;

namespace transactionnotes.ApiService.Middleware
{
    public class AuthHeaderInspectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthHeaderInspectionMiddleware> _logger;

        public AuthHeaderInspectionMiddleware(RequestDelegate next, ILogger<AuthHeaderInspectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                _logger.LogWarning("No Authorization header present for request to {Path}", context.Request.Path);
            }
            else if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogInformation("Bearer token present for request to {Path}", context.Request.Path);

                // Optional: Add token validation or inspection logic here
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        _logger.LogInformation("Token expires: {Expiration}", jwtToken.ValidTo);

                        // Log claims if needed
                        foreach (var claim in jwtToken.Claims)
                        {
                            _logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error inspecting JWT token");
                }
            }
            else
            {
                _logger.LogWarning("Authorization header present but not a Bearer token for request to {Path}",
                    context.Request.Path);
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

    // Extension method to make registration cleaner
    public static class AuthHeaderInspectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthHeaderInspection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthHeaderInspectionMiddleware>();
        }
    }
}
