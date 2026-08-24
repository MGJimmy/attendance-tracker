using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Service;

internal sealed class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {

        if (context.User is null || !context.User.Identity.IsAuthenticated || requirement is null || string.IsNullOrEmpty(requirement.RolesCommaSeprated))
            return;

        var roles = requirement.RolesCommaSeprated.Split(',');

        foreach (var role in roles)
        {
            var canAccess = context.User.Claims.Any(c => (c.Type == AppClaimType.Roles || c.Type == ClaimTypes.Role)
                && c.Value.ToLower() == role.ToLower());

            if (canAccess)
            {
                context.Succeed(requirement);
                return;
            }
        }

        await Task.CompletedTask;

    }
}
