using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Identity.Service
{
    public class LogoutCommand : IRequest<LogoutCommandResponse>
    {
       public string RefreshToken { get; set; }
    }

    public class LogoutCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
