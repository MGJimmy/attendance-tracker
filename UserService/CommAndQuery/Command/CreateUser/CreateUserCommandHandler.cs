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
            var role = request.Role?.Trim();
            if (role != RoleName.Admin && role != RoleName.User)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "Role must be Admin or User."
                };
            }

            var username = request.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "Username and password are required."
                };
            }

            var existingUser = await _idmProvider.FindByNameAsync(username);
            if (existingUser != null)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "User already exists",
                };
            }

            var email = string.IsNullOrWhiteSpace(request.Email)
                ? $"{username.ToLowerInvariant()}@attendancetracker.local"
                : request.Email.Trim();

            var user = new User
            {
                UserName = username,
                FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? username : request.FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(request.LastName) ? role : request.LastName.Trim(),
                PhoneNumber = request.PhoneNumber,
                Email = email
            };

            var registrationResult = await _idmProvider.CreateUserAsync(user, request.Password);
            if (!registrationResult.Success)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = registrationResult.ErrorMessage
                };
            }

            var roleResult = await _idmProvider.AssignRolesToUser(user, new[] { role });
            if (!roleResult.Success)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = roleResult.ErrorMessage,
                    UserId = user.Id
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