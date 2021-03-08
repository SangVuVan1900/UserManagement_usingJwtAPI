using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_Mvc.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
    }
}
