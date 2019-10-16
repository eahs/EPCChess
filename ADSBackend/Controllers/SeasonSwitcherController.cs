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
    [Authorize]
    public class SeasonSwitcherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeasonSwitcherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

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