using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserManagement_Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UserManagement_Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; // i51
        }


        public async Task<IActionResult> Login(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content =
                    new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:44308/api/User/Login", content))
                {
                    string token = await response.Content.ReadAsStringAsync();

                    if (token == "User account doesn't exist, please try again" || token == "Invalid data")
                    {
                        ViewBag.CheckValid = "User account doesn't exist";
                        return View(user);
                    }
                    HttpContext.Session.SetString("JWT", token);
                    return RedirectToAction("Privacy", "Home");
                }
            }
        }

        
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        } 


        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    //claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                }); 
            return Json(claims);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Login");
        } 

        public IActionResult Privacy()
        {
            var token = HttpContext.Session.GetString("JWT");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
