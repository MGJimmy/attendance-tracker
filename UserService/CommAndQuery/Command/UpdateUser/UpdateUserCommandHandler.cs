using MediatR;

namespace Identity.Service;

public class UpdateUserCommanddHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IIdmProvider<User> _idmProvider;

    public UpdateUserCommanddHandler(IIdmProvider<User> idmProvider)
    {
        _idmProvider = idmProvider;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var user = await _idmProvider.FindByIdAsync(dto.Id);
        if (user is null)
            return new UpdateUserResponse { Success = false, ErrorMessage = "المستخدم غير موجود" };

        user.Email = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        var (success, errorMessage) = await _idmProvider.UpdateUserAsync(user);
        if (!success)
            return new UpdateUserResponse { Success = false, ErrorMessage = errorMessage };

        var (rolesSuccess, rolesError) = await _idmProvider.ReplaceRolesAsync(user, dto.Roles);
        return new UpdateUserResponse { Success = rolesSuccess, ErrorMessage = rolesError };
    }
}
