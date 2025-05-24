using Microsoft.AspNetCore.Authorization;

namespace transactionnotes.ApiService.Authorization
{
    public class PermissionRequirement(string? permission) : IAuthorizationRequirement
    {
        public string? Permission { get; } = permission;
    }
}
