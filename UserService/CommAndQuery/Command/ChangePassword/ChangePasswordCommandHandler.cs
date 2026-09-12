using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return new ChangePasswordResponse { Success = false, ErrorMessage = "User not found." };

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return new ChangePasswordResponse { Success = false, ErrorMessage = "Password is required." };

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (result.Succeeded)
            return new ChangePasswordResponse { Success = true };

        return new ChangePasswordResponse
        {
            Success = false,
            ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
        };
    }
}
