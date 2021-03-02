using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{
    public class UserLogin
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
