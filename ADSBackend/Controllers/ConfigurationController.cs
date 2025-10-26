
using ADSBackend.Data;
using ADSBackend.Models.ConfigurationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing application configuration settings.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="configuration">The configuration service.</param>
        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        /// <summary>
        /// Displays the configuration management page.
        /// </summary>
        /// <returns>The configuration index view.</returns>
        public IActionResult Index()
        {
            var viewModel = new ConfigurationViewModel
            {
                SMTP_HOST = Configuration.Get("SMTP_HOST"),
                SMTP_PORT = Configuration.Get("SMTP_PORT"),
                SMTP_USER = Configuration.Get("SMTP_USER"),
                SMTP_PASSWORD = Configuration.Get("SMTP_PASSWORD")
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles the submission of updated configuration settings.
        /// </summary>
        /// <param name="viewModel">The view model containing the updated configuration.</param>
        /// <returns>A redirect to the index page on success, or the view with errors on failure.</returns>
        // POST: Configuration/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfigurationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Configuration.Set("SMTP_HOST", viewModel.SMTP_HOST);
                Configuration.Set("SMTP_PORT", viewModel.SMTP_PORT);
                Configuration.Set("SMTP_USER", viewModel.SMTP_USER);
                Configuration.Set("SMTP_PASSWORD", viewModel.SMTP_PASSWORD);

                await Configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}