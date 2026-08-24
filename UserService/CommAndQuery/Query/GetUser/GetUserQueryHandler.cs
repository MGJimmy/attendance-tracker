using MediatR;

namespace Identity.Service
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IIdmProvider<User> _idmProvider;

        public GetUserQueryHandler(IIdmProvider<User> idmProvider)
        {
            _idmProvider = idmProvider;
        }

        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            return await _idmProvider.FindByNameAsync(request.Username);
        }
    }
}