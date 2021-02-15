using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Rules()
        {
            return View();
        }

        public IActionResult VirtualRules()
        {
            return View();
        }
    }
}
