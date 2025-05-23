using Microsoft.AspNetCore.Authorization;

namespace transactionnotes.ApiService.Authorization
{
    public class SessionHandler : AuthorizationHandler<SessionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionRequirement requirement)
        {
            //// Check if the user has the required role
            //if (context.User.HasClaim(c => c.Type == "session" && c.Value == requirement.SessionName))
            //{
            //    context.Succeed(requirement);
            //}
            context.Succeed(requirement);
            var failure = new AuthorizationFailureReason(this, $"{requirement.SessionName} needs admin");
            //context.Fail(failure);
            return Task.CompletedTask;
        }
    }
}
