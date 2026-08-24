using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service
{
    public class TokenResponse
    {
        public AccessTokenResponse LoginResponse { get; set; }
        public string RefreshToken { get; set; }
    }

    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    public class AccessTokenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AccessToken Token { get; set; } // Updated to use AccessToken
    }
}
