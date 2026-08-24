using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Identity.Service
{
    public class LogoutCommadHandler : IRequestHandler<LogoutCommand, LogoutCommandResponse>
    {
        private readonly ITokenProvider _tokenProvider;

        public LogoutCommadHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public Task<LogoutCommandResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Task.FromResult(new LogoutCommandResponse
                {
                    Success = false,
                    Message = "Refresh token is missing."
                });
            }

            _tokenProvider.RevokeRefreshTokenAsync(request.RefreshToken);

            return Task.FromResult(new LogoutCommandResponse
            {
                Success = true,
                Message = "Logged out successfully."
            });

        }
    }
}
