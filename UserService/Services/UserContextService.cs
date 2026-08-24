using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Identity.Service;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return null;

        return user.FindFirst(AppClaimType.NameIdentifier)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("nameid")?.Value
            ?? user.FindFirst("sub")?.Value;
    }

    public string GetCurrentUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(AppClaimType.Username)?.Value
            ?? user?.Identity?.Name;
    }

    public IEnumerable<string> GetCurrentUserRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Enumerable.Empty<string>();

        var roles = new List<string>();

        foreach (var claim in user.Claims)
        {
            if (!IsRoleClaim(claim.Type))
                continue;

            foreach (var role in ParseRoles(claim.Value))
            {
                if (!string.IsNullOrWhiteSpace(role)
                    && !roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    roles.Add(role);
                }
            }
        }

        return roles;
    }

    public bool IsAdmin()
    {
        return GetCurrentUserRoles().Any(role =>
            string.Equals(role, RoleName.Admin, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsRoleClaim(string type)
    {
        return type == AppClaimType.Roles
            || type == ClaimTypes.Role
            || string.Equals(type, "role", StringComparison.OrdinalIgnoreCase)
            || type.EndsWith("/role", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> ParseRoles(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        var trimmed = value.Trim();
        if (trimmed.StartsWith('['))
        {
            List<string> parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<List<string>>(trimmed);
            }
            catch (JsonException)
            {
            }

            if (parsed != null)
            {
                foreach (var role in parsed)
                    yield return role;
                yield break;
            }
        }

        yield return trimmed;
    }
}
