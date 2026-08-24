using MediatR;

namespace Identity.Service;
internal class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, AssignUserRolesResponse>
{
    private readonly IIdmProvider<User> _idmProviderService;

    public AssignUserRolesCommandHandler(IIdmProvider<User> idmProviderService)
    {
        _idmProviderService = idmProviderService;
    }

    public async Task<AssignUserRolesResponse> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _idmProviderService.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new AssignUserRolesResponse { Success = false, Message = "User not found" };
        }

        var roles = await _idmProviderService.GetUserRoles(user);
        if (request.Roles.Any(roles.Contains))
        {
            return new AssignUserRolesResponse { Success = false, Message = "User already has one or more of the specified roles" };
        }

        var assignResult = await _idmProviderService.AssignRolesToUser(user, request.Roles);
        if (!assignResult.Success)
        {
            return new AssignUserRolesResponse { Success = false, Message = assignResult.ErrorMessage };
        }

        return new AssignUserRolesResponse { Success = true, Message = "Roles assigned successfully" };
    }
}

