using Microsoft.AspNetCore.Authorization;

namespace Identity.Service;
internal sealed class RoleRequirement : IAuthorizationRequirement
{
    public string RolesCommaSeprated { get; private set; }

    public RoleRequirement(string rolesCommaSeprated)
    {
        RolesCommaSeprated = rolesCommaSeprated;
    }
}
