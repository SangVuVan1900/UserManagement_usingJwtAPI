using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UserManagement_usingJwt.Models;
using UserManagement_usingJwt.Services;

namespace UserManagement_usingJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
         
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromQuery] User user)
        { 
            try
            {
                var result = await _userService.Register(user);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromQuery] UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(user);

                if (result.IsSuccess)
                {
                    return Ok(result);
                } 
                return BadRequest(result);
            }
            return BadRequest("Account invalid");
        }
         
        //[HttpGet]
        //[Authorize]
        //public Task<IActionResult> GetUserProfile()
        //{

        //}

        //private RefreshTokenKey GenerateTokenKey()
        //{
        //    RefreshTokenKey tokenKey = new RefreshTokenKey();

        //    var randomNumber = new byte[32];
        //    using (var item = RandomNumberGenerator.Create())
        //    {
        //        item.GetBytes(randomNumber);
        //        tokenKey.Token = Convert.ToBase64String(randomNumber);
        //    }

        //    tokenKey.ExpireDate = DateTime.UtcNow.AddDays(3);
        //    return tokenKey;
        //}



    }
}
