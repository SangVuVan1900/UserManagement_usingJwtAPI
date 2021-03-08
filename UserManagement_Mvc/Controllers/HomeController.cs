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

namespace UserManagement_Mvc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Login(User user)
        {
            ViewBag.CheckValid = "";
            using (var httpClient = new HttpClient())
            {
                StringContent content =
                    new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:44308/api/User/Login", content))
                {
                    string token = await response.Content.ReadAsStringAsync();

                    if (token.Length > 50)
                    {
                        HttpContext.Session.SetString("JWT", token);
                        return RedirectToAction("Privacy", "Home");
                    }
                    else
                    {
                        ViewBag.CheckValid = "Usere account doesn't exist";
                    }
                }
            }
            return View(user);
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
