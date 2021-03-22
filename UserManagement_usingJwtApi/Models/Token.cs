using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Models
{
    public class Token
    {
        public string TokenKey { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
