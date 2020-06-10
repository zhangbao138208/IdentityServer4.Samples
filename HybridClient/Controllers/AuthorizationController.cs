using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridClient.Controllers
{
    public class AuthorizationController:Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
