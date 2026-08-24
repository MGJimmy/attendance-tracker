using MediatR;

namespace Identity.Service
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
    {
        private readonly ITokenProvider _tokenProvider;

        public RefreshTokenCommandHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // check if refresh token is valid and not expired
            var refreshTokenEntity = await _tokenProvider.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshTokenEntity is null || refreshTokenEntity.IsExpired)
            {
                return new RefreshTokenCommandResponse
                {
                    LoginResponse = new AccessTokenResponse
                    {
                        Success = false,
                        Token = null,
                        Message = "Invalid or expired refresh token."
                    },
                    RefreshToken = string.Empty
                };
            }

            // revoke the used refresh token

            await _tokenProvider.RevokeAllUserRefreshTokensAsync(refreshTokenEntity.User.Id);

            // Generate new access token
            var token = await _tokenProvider.GenerateAccessToken(refreshTokenEntity.User);

            string refreshToken = await _tokenProvider.GenerateRefreshToken(refreshTokenEntity.User.Id);

            var loginResponse = new AccessTokenResponse
            {
                Success = true,
                Token = token
            };

            return new RefreshTokenCommandResponse
            {
                LoginResponse = loginResponse,
                RefreshToken = refreshToken
            };

        }
    }
}
