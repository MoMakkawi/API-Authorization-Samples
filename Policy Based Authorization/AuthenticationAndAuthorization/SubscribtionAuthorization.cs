using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Policy_Based_Authorization.AuthenticationAndAuthorization;

public class SubscriptionAuthorizationRequirement : IAuthorizationRequirement;
public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionAuthorizationRequirement requirement)
    {
        if (context.User.FindFirstValue("IsPremium") is string isPremium && bool.Parse(isPremium))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
