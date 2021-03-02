using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManagement_usingJwt.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserManagement_usingJwt.Services
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private UserManager<Profile> _profileManager;
        private IConfiguration _configuration;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        } 

        private static bool IsValidEmail(string checkEmail)
        {
            try
            {
                MailAddress m = new MailAddress(checkEmail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
         
        public async Task<UserManagerResponse> Register(User user)
        {
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User account is null, please try again!",
                    IsSuccess = false
                };
            }

            if (IsValidEmail(user.Email) == false)
            {
                return new UserManagerResponse
                {
                    Message = "Email address is invalid, please try again!",
                    IsSuccess = false
                };
            }

            if (user.Password != user.CofirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match Password, please try again!",
                    IsSuccess = false
                };
            }

            var IdentityUser = new IdentityUser
            {
                UserName = user.Username,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(IdentityUser, user.Password);

            if (result.Succeeded)
            {
                var profileUser = new Profile
                {
                    Birthdate = DateTime.Now,
                    UserId = IdentityUser.Id
                };
                //var profile1 = await _profileManager.CreateAsync(profileUser);
                return new UserManagerResponse
                {
                    Message = "Register Successfully",
                    IsSuccess = true
                };
            }
            else
            {
                return new UserManagerResponse
                {
                    Message = "Register fail, something has gone wrong",
                    IsSuccess = false
                };
            }
        }
        public async Task<UserManagerResponse> Login(UserLogin user)
        {
            var _user = await _userManager.FindByNameAsync(user.Username);
            if (_user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Account doesn't exist, try again!",
                    IsSuccess = false
                };
            }
            var result = await _userManager.CheckPasswordAsync(_user, user.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Account doesn't exist, try again!",
                    IsSuccess = false
                };
            }

            //var claims = new[]
            //{
            //    new Claim("Email", user.Username),
            //    new Claim(ClaimTypes.NameIdentifier, _user.Id)
            //};
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["UserSettings:Key"])); 
              
            var token = new JwtSecurityToken(
                issuer: _configuration["UserSettings:Key"],
                audience: _configuration["UserSettings:Audience"], 
                //claims = claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse 
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }
    }
}
