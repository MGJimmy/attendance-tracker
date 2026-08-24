using MediatR;

namespace Identity.Service;

public class ChangePasswordResponse
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
{
    public string UserId { get; set; }
    public string NewPassword { get; set; }
}
