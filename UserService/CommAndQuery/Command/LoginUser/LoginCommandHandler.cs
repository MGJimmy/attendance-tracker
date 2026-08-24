using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.Service
{

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IIdmProvider<User> _idmProvider;

        public LoginCommandHandler(ITokenProvider tokenProvider, IIdmProvider<User> idmProvider)
        {
            _tokenProvider = tokenProvider;
            _idmProvider = idmProvider;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user credentials using IdmProvider
            var user = await _idmProvider.FindByNameAsync(request.Username);
            var loginStatus = await _idmProvider.LoginUserAsync(request.Username, request.Password);

            if (user is null || loginStatus != LoginStatus.Succeeded)
            {
                return new LoginCommandResponse
                {
                    LoginResponse = new AccessTokenResponse
                    {
                        Success = false,
                        Token = null,
                        Message = loginStatus.ToString()
                    },
                    RefreshToken = string.Empty
                };
            }



            var token = await _tokenProvider.GenerateAccessToken(user);

            string refreshToken = await _tokenProvider.GenerateRefreshToken(user.Id);

            var loginResponse = new AccessTokenResponse
            {
                Success = true,
                Token = token
            };

            return new LoginCommandResponse
            {
                LoginResponse = loginResponse,
                RefreshToken = refreshToken
            };
        }


    }
}