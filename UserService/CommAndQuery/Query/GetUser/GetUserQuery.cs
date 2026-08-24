using MediatR;

namespace Identity.Service
{
    public class GetUserQuery : IRequest<User>
    {
        public string Username { get; set; }

        public GetUserQuery(string username)
        {
            Username = username;
        }
    }
}
