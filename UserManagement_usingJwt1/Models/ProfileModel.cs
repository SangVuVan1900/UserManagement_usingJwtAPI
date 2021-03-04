using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Models
{
    public class ProfileModel
    {
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
    }
}
