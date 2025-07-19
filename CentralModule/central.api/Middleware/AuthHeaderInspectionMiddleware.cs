using System.IdentityModel.Tokens.Jwt;

namespace central.api.Middleware
{
    public class AuthHeaderInspectionMiddleware(RequestDelegate next, ILogger<AuthHeaderInspectionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                logger.LogWarning("No Authorization header present for request to {Path}", context.Request.Path);
            }
            else if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                logger.LogInformation("Bearer token present for request to {Path}", context.Request.Path);

                // Optional: Add token validation or inspection logic here
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        logger.LogInformation("Token expires: {Expiration}", jwtToken.ValidTo);

                        // Log claims if needed
                        foreach (var claim in jwtToken.Claims)
                        {
                            logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error inspecting JWT token");
                }
            }
            else
            {
                logger.LogWarning("Authorization header present but not a Bearer token for request to {Path}",
                    context.Request.Path);
            }

            // Call the next middleware in the pipeline
            await next(context);
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
