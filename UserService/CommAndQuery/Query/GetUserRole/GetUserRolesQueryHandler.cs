using MediatR;

namespace Identity.Service
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, GetUserRolesResponse>
    {
        private readonly IIdmProvider<User> _idmProvider;

        public GetUserRolesQueryHandler(IIdmProvider<User> idmProvider)
        {
            _idmProvider = idmProvider;
        }

        public async Task<GetUserRolesResponse> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _idmProvider.FindByNameAsync(request.Username);
            if (user == null)
            {
                return new GetUserRolesResponse
                {
                    Success = false,
                    Roles = null,
                    Message = "User not found"
                };
            }

            var roles = await _idmProvider.GetUserRoles(user);
            return new GetUserRolesResponse
            {
                Success = true,
                Roles = roles
            };
        }
    }
}