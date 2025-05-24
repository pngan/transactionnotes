using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using transactionnotes.ApiService.Authorization;

namespace transactionnotes.ApiService.Middleware
{
    public class AssignUserPermissionsMiddleware(RequestDelegate next, ILogger<AuthHeaderInspectionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // todo - read 
            context.Items[HttpContextItems.UserPermissions] = UserPermission.PermissionWrite;
            await next(context);
        }
    }
}
