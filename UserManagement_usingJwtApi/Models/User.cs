﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Models
{
    public class User
    {
        [JsonIgnore] 
        public int Id { get; set; } 

        [Column(TypeName = ("varchar(50)"))]
        public string Email { get; set; } 

        [Column(TypeName = ("varchar(50)"))]
        public string Username { get; set; }

        [Column(TypeName = ("varchar(255)"))]
        public string Password { get; set; }

        [JsonIgnore]
        public int Profile_Id { get; set; }

        //public virtual ICollection<RefreshToken> RefreshTokens { get; set; } 
    }
}
