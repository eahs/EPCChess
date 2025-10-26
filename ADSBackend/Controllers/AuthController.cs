
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for handling authentication challenges.
    /// </summary>
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        /// <summary>
        /// Initiates a login challenge.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after authentication.</param>
        /// <returns>A challenge result.</returns>
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }
    }
}