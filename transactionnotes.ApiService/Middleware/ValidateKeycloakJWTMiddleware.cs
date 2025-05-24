using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using transactionnotes.ApiService.Authorization;

namespace transactionnotes.ApiService.Middleware
{
    public class ValidateKeycloakJwtMiddleware(RequestDelegate next, ILogger<AuthHeaderInspectionMiddleware> logger)
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

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);



                        var subject = jwtToken.Subject;
                        if (subject == null)
                        {
                            throw new AuthenticationException("Subject claim is missing from the keycloak JWT token");
                        }

                        context.Items[HttpContextItems.UserJwtSub] = subject;

                        logger.LogInformation("Token expires: {Expiration}", jwtToken.ValidTo);

                        // Log claims if needed
                        foreach (var claim in jwtToken.Claims)
                        {
                            logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
                        }
                    }
                }
                catch (AuthenticationException ex)
                {
                    logger.LogWarning(ex, "Authentication error for request to {Path}", context.Request.Path);
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Validation failed",
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/problem+json";

                    // Return the problem details as JSON

                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
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
}
