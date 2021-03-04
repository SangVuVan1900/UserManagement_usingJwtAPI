﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserManagement_usingJwt1.Models;

namespace UserManagement_usingJwt1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //_context.Entry(profile).State = EntityState.Modified;
        private static int _ID;
        private IConfiguration _configuration;
        private readonly UserDbContext _context;

        public UserController(IConfiguration configuration, UserDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                var _user = await _context.Users.FirstOrDefaultAsync
                    (u => u.Username == user.Username && u.Password == Encrypt(user.Password));

                if (_user != null)
                { 
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["UserSettings:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", _user.Id.ToString()),
                        new Claim("Username", _user.Username), 
                        new Claim("Password", Encrypt(_user.Password)),
                    };

                    var key = new SymmetricSecurityKey
                       (Encoding.UTF8.GetBytes(_configuration["UserSettings:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["UserSettings:Issuer"],
                        _configuration["UserSettings:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signIn);
                    _ID = _user.Id;

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("User account doesn't exist, please try again!");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserParams user)
        {
            if (user != null)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    return BadRequest("Password must be match with ConfirmPassword");
                }
                try
                {
                    User _user = new User();
                    _user.Email = user.Email;
                    _user.Username = user.Username;
                    _user.Password = Encrypt(user.Password); 
                    _context.Users.Add(_user);
                    await _context.SaveChangesAsync();

                    return Ok(_user);
                }
                catch
                {
                    return BadRequest("Invalid data");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }
        [HttpGet("GetProfile")]
        public ActionResult<Profile> GetProfile()
        {
            var user = _context.Users.Where(u => u.Id == _ID).FirstOrDefault();
            var profile = _context.Profiles.Where(p => p.Id == user.Profile_Id).FirstOrDefault();

            if (profile == null)
            {
                return NotFound("Data not found");
            }
            return profile;
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> updateUser_Profile([FromQuery] ProfileModel profile)
        {
            try
            {
                var user = _context.Users.Where(u => u.Id == _ID).FirstOrDefault();
                var _profile = _context.Profiles.Where(p => p.Id == user.Profile_Id).FirstOrDefault();

                if (_profile != null) //3 
                {
                    Update(_profile, profile);
                    await _context.SaveChangesAsync();
                    return Ok(profile);
                }
                return BadRequest("Invalid data");
            }
            catch
            {
                return BadRequest("Invalid data");
            }
        }
        private void Update(Profile _profile, ProfileModel profile)
        {
            _profile.Firstname = profile.Firstname;
            _profile.Lastname = profile.Lastname;
            _profile.Birthdate = profile.Birthdate;
            _profile.Gender = profile.Gender;
            _profile.Address = profile.Address;
        }
        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123qq";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}