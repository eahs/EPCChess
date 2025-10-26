
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for informational pages.
    /// </summary>
    public class InfoController : Controller
    {
        /// <summary>
        /// Displays the main info page.
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the rules page.
        /// </summary>
        /// <returns>The rules view.</returns>
        public IActionResult Rules()
        {
            return View();
        }

        /// <summary>
        /// Displays the virtual rules page.
        /// </summary>
        /// <returns>The virtual rules view.</returns>
        public IActionResult VirtualRules()
        {
            return View();
        }
    }
}