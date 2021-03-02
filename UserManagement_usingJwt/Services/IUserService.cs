using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement_usingJwt.Models;

namespace UserManagement_usingJwt.Services 
{ 
    public interface IUserService 
    {      
        Task<UserManagerResponse> Register(User user);
        Task<UserManagerResponse> Login(UserLogin user); 
    }
}
