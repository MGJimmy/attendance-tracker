using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Identity.Service
{
    public class RefreshTokenCommandResponse : TokenResponse
    {
    }

    public class RefreshTokenCommand : IRequest<RefreshTokenCommandResponse>
    {
        public string RefreshToken { get; set; }
    }
}
