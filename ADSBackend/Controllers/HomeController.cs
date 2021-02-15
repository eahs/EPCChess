using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADSBackend.Controllers
{
    public class HomeController : Controller
    {

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
