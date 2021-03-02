using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{ 
    public class UserManagerResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }  

        public DateTime? ExpireDate { get; set; }
    }
}
