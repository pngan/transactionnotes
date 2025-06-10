namespace transactionnotes.ApiService.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder AddUserMiddleware(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<AuthHeaderInspectionMiddleware>()
            .UseMiddleware<ValidateKeycloakJwtMiddleware>();
          //  .UseMiddleware<AssignUserPermissionsMiddleware>();
    }
}