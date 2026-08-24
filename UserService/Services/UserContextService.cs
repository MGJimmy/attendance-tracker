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
        return _httpContextAccessor.HttpContext?.User?.FindFirst(AppClaimType.NameIdentifier)?.Value;
    }

    public string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }

    public IEnumerable<string> GetCurrentUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User?.FindFirst(AppClaimType.Roles);

        if (roles != null)
        {

           return JsonSerializer.Deserialize<IEnumerable<string>>(roles?.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        }
        return Enumerable.Empty<string>();
    }
}