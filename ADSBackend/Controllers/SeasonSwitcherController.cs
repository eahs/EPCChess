
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for switching the active season in the user's session.
    /// </summary>
    [Authorize]
    public class SeasonSwitcherController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonSwitcherController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SeasonSwitcherController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the index view for the season switcher.
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Switches the active season in the session.
        /// </summary>
        /// <param name="id">The ID of the season to switch to.</param>
        /// <returns>A string indicating the result of the operation ("OK" or "NOT OK").</returns>
        // GET: SeasonSwitcher/Switch/1
        public async Task<String> Switch(int? id)
        {
            if (id == null) return "NOT OK";

            var season = await _context.Season.FirstOrDefaultAsync(x => x.SeasonId == id);

            if (season == null) return "NOT OK";


            if (season != null)
            {
                HttpContext.Session.SetInt32("SeasonId", season.SeasonId);
            }

            return "OK";
        }
    }
}