using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserManagement_usingJwt1.Models;
using UserManagement_usingJwt1.Services;

namespace UserManagement_usingJwt1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static int _ID;
        private static DateTime ExpiredKey;
        private static User users;
        private IConfiguration _configuration;
        private readonly UserDbContext _context;
        private readonly IMemoryCache _cache;
        private IMailService _mailService;

        public UserController(IConfiguration configuration, UserDbContext context
            , IMemoryCache memoryCache, IMailService mailService)
        {
            _configuration = configuration;
            _context = context;
            _cache = memoryCache;
            _mailService = mailService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [Produces("application/json")] // chuyển response của nó về dạng application/json
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            try
            {

                if (user.Username != null && user.Password != null)
                {
                    users = await _context.Users.FirstOrDefaultAsync
                        (u => u.Username == user.Username && u.Password == Encrypt(user.Password));

                    if (users != null)
                    {
                        Token token = new Token();
                        //await _mailService.SendEmailAsync(users.Email);
                        //object obj;
                        //obj = RefreshTokenKey(users);
                        //return Request.CreateResponse(HttpStatusCode.OK, obj); 

                        return Ok(RefreshTokenKey(users));
                    }
                }
                return BadRequest("User account doesn't exist, please try again");
            }
            catch
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                if (user != null)
                {
                    if (IsValidEmail(user.Email) == false)
                    {
                        return BadRequest("Email address is invalid, try again!!");
                    }

                    var checkUserExist = _context.Users.Where(u => u.Username == user.Username).FirstOrDefault();
                    if (checkUserExist != null)
                    {
                        return BadRequest("User account already existed, please try again");
                    }

                    try
                    {
                        user.Password = Encrypt(user.Password);
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        return Ok(user);
                    }
                    catch
                    {
                        return BadRequest("Invalid data");
                    }
                }
                return BadRequest("Invalid data");
            }
            catch
            {
                return BadRequest("Invalid data");
            }

        }

        [HttpGet("CheckTokenExpire")]
        [AllowAnonymous]
        public ActionResult CheckTokenExpire()
        {
            DateTime date = DateTime.ParseExact
                   ("01/01/0001 12:00:00 AM", "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            if (DateTime.Compare(ExpiredKey, date) == 0)
            {
                return BadRequest("You have to login :D");
            }

            var atTime = DateTime.UtcNow;
            if (DateTime.Compare(atTime, ExpiredKey) > 0)
            {
                return BadRequest("Token key to expire");
            }

            return Ok("Token key still active");
        }

        [HttpGet("GetProfile")] 
        public ActionResult<Profile> GetProfile()
        {
            var user = _context.Users.Where(u => u.Id == _ID).FirstOrDefault();
            Profile profile = _context.Profiles.Where(p => p.Id == user.Profile_Id).FirstOrDefault();

            if (profile == null)
            {
                return NotFound("Data not found");
            }
            return profile;
        }

        [HttpPut("UpdateProfile")] 
        [Produces("application/json")] // chuyển response của nó về dạng application/json 11
        public async Task<IActionResult> updateUser_Profile([FromBody] ProfileModel profile)
        {
            try
            {
                var user = _context.Users.Where(u => u.Id == _ID).FirstOrDefault();
                var _profile = _context.Profiles.Where(p => p.Id == user.Profile_Id).FirstOrDefault();

                if (_profile != null) //i5
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

        [HttpGet("RefreshToken")]
        [AllowAnonymous]
        public ActionResult RefreshToken()
        {
            if (users != null)
            {
                return Ok(RefreshTokenKey(users));
            }
            return BadRequest("You have to login to operate");
        }

        private string RefreshTokenKey(User _user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["UserSettings:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", _user.Id.ToString()),
                    new Claim("Username", _user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials
                        (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _ID = _user.Id;
            ExpiredKey = token.ValidTo;
            return tokenHandler.WriteToken(token);
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
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123qq";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
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
    }
}

//if (!_cache.TryGetValue(StaticToken.TokenKey, out tokenString))
//{
//    var cacheEntryOptions = new MemoryCacheEntryOptions()
//        //Keep in cache for this time, reset the time if accessed.
//        .SetSlidingExpiration(TimeSpan.FromSeconds(20));
//    _cache.Set(StaticToken.TokenKey, tokenString, cacheEntryOptions);
//     var res = _cache.Set(StaticToken.TokenKey, tokenString, cacheEntryOptions);
//    StaticToken.TokenKey = res; 
//} 


//string responseData = string.Empty;
//HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenString);
//request.Method = "POST";
//request.ContentType = "application/json";
//request.CookieContainer = new CookieContainer();

//HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
//using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
//{
//    responseData = responseReader.ReadToEnd();
//}

//var toke = response.Headers.Get("authToken");
//JObject o = JObject.Parse(responseData);
//tokenString = (string)o["response"]["authToken"].ToString();




////////date
//declare @qq datetime
//  set @qq = (select DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0))
//  print(@qq)

//  print(CONVERT(DATE, getdate(), 101))
