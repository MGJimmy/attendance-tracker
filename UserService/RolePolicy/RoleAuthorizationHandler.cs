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
            var canAccess = context.User.Claims.Any(c =>
                (c.Type == AppClaimType.Roles || c.Type == ClaimTypes.Role || c.Type == "role")
                && RoleValueContains(c.Value, role));

            if (canAccess)
            {
                context.Succeed(requirement);
                return;
            }
        }

        await Task.CompletedTask;
    }

    private static bool RoleValueContains(string claimValue, string role)
    {
        if (string.IsNullOrWhiteSpace(claimValue) || string.IsNullOrWhiteSpace(role))
            return false;

        var trimmed = claimValue.Trim();
        if (trimmed.StartsWith('['))
        {
            try
            {
                var roles = System.Text.Json.JsonSerializer.Deserialize<string[]>(trimmed);
                return roles?.Any(value =>
                    string.Equals(value, role, StringComparison.OrdinalIgnoreCase)) == true;
            }
            catch (System.Text.Json.JsonException)
            {
            }
        }

        return string.Equals(trimmed, role, StringComparison.OrdinalIgnoreCase);
    }
}
