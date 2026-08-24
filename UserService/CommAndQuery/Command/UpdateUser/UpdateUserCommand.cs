using MediatR;

namespace Identity.Service;

public class UpdateUserResponse
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class UpdateUserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public UpdateUserDto Dto { get; set; }
}
