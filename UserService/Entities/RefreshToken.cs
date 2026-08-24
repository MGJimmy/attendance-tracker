using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
