using MediatR;

namespace Identity.Service
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IIdmProvider<User> _idmProvider;

        public CreateUserCommandHandler(IIdmProvider<User> idmProvider)
        {
            _idmProvider = idmProvider;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if the user already exists
            var existingUser = await _idmProvider.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "User already exists",
                };
            }

            // Register the user
            var user = new User { UserName = request.Username, FirstName = request.FirstName, LastName = request.LastName, PhoneNumber = request.PhoneNumber, Email = request.Email };
            var registrationResult = await _idmProvider.CreateUserAsync(user, request.Password);
            if (!registrationResult.Success)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = registrationResult.ErrorMessage
                };
            }

            return new CreateUserResponse
            {
                Success = true,
                Message = "User registered successfully",
                UserId = user.Id
            };
        }
    }
}