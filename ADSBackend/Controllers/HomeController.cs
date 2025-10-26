
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for the home page.
    /// </summary>
    public class HomeController : Controller
    {

        /// <summary>
        /// Displays the home page. Redirects to the admin dashboard if the user is authenticated.
        /// </summary>
        /// <returns>The home view or a redirect to the admin dashboard.</returns>
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

    }
}