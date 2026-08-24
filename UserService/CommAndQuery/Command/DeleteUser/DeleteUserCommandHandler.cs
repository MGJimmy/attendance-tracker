using MediatR;

namespace Identity.Service
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly IIdmProvider<User> _idmProvider;

        public DeleteUserCommandHandler(IIdmProvider<User> idmProvider)
        {
            _idmProvider = idmProvider;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var (success, errorMessage) = await _idmProvider.DeleteUserAsync(request.UserId);
            return new DeleteUserResponse
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }
    }
}