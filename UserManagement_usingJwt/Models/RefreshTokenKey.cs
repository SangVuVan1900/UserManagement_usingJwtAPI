using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{
    public class RefreshTokenKey
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public virtual User User { get; set; }
         
    }
}
