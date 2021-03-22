using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Models
{
    public class UserParams
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; } 

        public string ConfirmPassword { get; set; }
    }
}
