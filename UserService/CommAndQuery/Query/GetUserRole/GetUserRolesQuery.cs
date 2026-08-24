using MediatR;

namespace Identity.Service
{
    public class GetUserRolesResponse
    {
        public bool Success { get; set; }
        public List<string> Roles { get; set; }
        public string Message { get; set; }
    }

    public class GetUserRolesQuery : IRequest<GetUserRolesResponse>
    {
        public string Username { get; set; }
    }
}