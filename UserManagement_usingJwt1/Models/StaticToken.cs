using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Models
{
    public static class StaticToken  
    {
        public static string TokenKey { get; set; } = "_Keyyy";
        public static DateTime ExpiredDate { get { return DateTime.Now; } } 
    }
}
