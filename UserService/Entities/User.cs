using Microsoft.AspNetCore.Identity;

namespace Identity.Service
{
    public class User : IdentityUser, IApplicationUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual List<RefreshToken> RefreshTokens { get; set; }
    }
}
