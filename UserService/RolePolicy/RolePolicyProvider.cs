using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Identity.Service;
internal sealed class RolePolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public RolePolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        var result = await base.GetPolicyAsync(policyName);
        if (result == null)
        {
            result = new AuthorizationPolicyBuilder()
                  .AddRequirements(new RoleRequirement(policyName))
                  .Build();
        }

        return result;
    }
}
