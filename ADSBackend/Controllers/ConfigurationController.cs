using ADSBackend.Data;
using ADSBackend.Models.ConfigurationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var viewModel = new ConfigurationViewModel
            {
                RSSFeedUrl = Configuration.Get("RSSFeedUrl"),
                PrivacyPolicyUrl = Configuration.Get("PrivacyPolicyUrl"),
                SMTP_HOST = Configuration.Get("SMTP_HOST"),
                SMTP_PORT = Configuration.Get("SMTP_PORT"),
                SMTP_USER = Configuration.Get("SMTP_USER"),
                SMTP_PASSWORD = Configuration.Get("SMTP_PASSWORD")
            };

            return View(viewModel);
        }

        // POST: Configuration/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfigurationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Configuration.Set("RSSFeedUrl", viewModel.RSSFeedUrl);
                Configuration.Set("PrivacyPolicyUrl", viewModel.PrivacyPolicyUrl);
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