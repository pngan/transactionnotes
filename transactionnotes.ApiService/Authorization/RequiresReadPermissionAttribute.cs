using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using transactionnotes.ApiService.Middleware;

namespace transactionnotes.ApiService.Authorization
{
    // This attribute is no longer used in preference to using policies
    public class RequiresReadPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Items.TryGetValue(HttpContextItems.UserPermissions, out var permission)
                || permission is not UserPermission permissionValue
                || (permissionValue | UserPermission.PermissionRead) != UserPermission.PermissionRead)
            {
                context.Result = new ObjectResult("User requires Read Permission")
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            return Task.CompletedTask;
        }
    }
}
