using Microsoft.AspNetCore.Authorization;

namespace Identity.Service;

public class RoleAttribute : AuthorizeAttribute
{
    public RoleAttribute(params string[] roles)
        : base(ConstructPolicy(roles))
    {
    }

    private static string ConstructPolicy(params string[] roles)
    {
        return string.Join(",", roles);
    }
}
