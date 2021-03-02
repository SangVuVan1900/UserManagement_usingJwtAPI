using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{
    public class User
    {
        //Id identity(1,1), Email, Username, 
        //Password(password là dạng mã hóa 1 chiều), 
        //và Profile_Id(khóa ngoại trỏ đến bảng profile)

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = ("varchar(50)"))]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        public string CofirmPassword { get; set; }
    }
}
