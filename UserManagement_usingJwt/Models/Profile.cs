using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{
    public class Profile
    {
        public int Id { get; set; }

        [Column(TypeName = ("nvarchar(50)"))]
        public string Firstname { get; set; }

        [Column(TypeName = ("nvarchar(50)"))]
        public string Lastname { get; set; }

        [DataType(DataType.Date)] 
        public DateTime Birthdate { get; set; } 

        [Column(TypeName = ("nvarchar(10)"))]
        public string Gender { get; set; }

        [Column(TypeName = ("nvarchar(100)"))]
        public string Address { get; set; }

        [Column(TypeName = ("varchar(450)"))]
        public string UserId { get; set; }
    }
}
