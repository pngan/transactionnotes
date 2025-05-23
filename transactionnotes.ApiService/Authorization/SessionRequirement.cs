using Microsoft.AspNetCore.Authorization;

namespace transactionnotes.ApiService.Authorization
{
    public class SessionRequirement(string? sessionName) : IAuthorizationRequirement
    {
        public string? SessionName { get; } = sessionName;
    }
}
