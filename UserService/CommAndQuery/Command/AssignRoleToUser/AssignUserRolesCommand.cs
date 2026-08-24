using MediatR;

namespace Identity.Service
{

    public class AssignUserRolesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AssignUserRolesCommand : IRequest<AssignUserRolesResponse>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }

        public AssignUserRolesCommand(Guid userId, List<string> roles)
        {
            UserId = userId;
            Roles = roles;
        }
    }
}
