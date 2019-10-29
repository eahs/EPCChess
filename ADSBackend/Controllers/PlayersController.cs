using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ADSBackend.Models.Identity;
using ADSBackend.Services;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin,Advisor")]
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataService _dataService;

        public PlayersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataService dataService)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            int schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
            {
                return NotFound();
            }

            ViewBag.School = await _context.School.FirstOrDefaultAsync(x => x.SchoolId == schoolId);


            return View(await _context.Player.Where(x => x.PlayerSchoolId == schoolId).OrderByDescending(x => x.Rating).ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int schoolId = await _dataService.GetSchoolIdAsync(User);

            var player = await _context.Player                
                .FirstOrDefaultAsync(m => m.PlayerId == id && m.PlayerSchoolId == schoolId);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerId,PlayerSchoolId,FirstName,LastName,Rating")] Player player)
        {
            int schoolId = await _dataService.GetSchoolIdAsync(User);

            if (ModelState.IsValid)
            {
                player.PlayerSchoolId = schoolId;

                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlayerId,FirstName,LastName,Rating")] Player player)
        {
            bool logRating = false;
            int schoolId = await _dataService.GetSchoolIdAsync(User);

            var _player = _context.Player.Find(id);

            if (id != player.PlayerId && _player.PlayerSchoolId != schoolId)
            {
                return NotFound();
            }

            if (_player.Rating != player.Rating) logRating = true;

            _player.FirstName = player.FirstName;
            _player.LastName = player.LastName;
            _player.Rating = player.Rating;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(_player);

                    if (logRating)
                    {
                        await _dataService.LogRatingEvent(_player.PlayerId, _player.Rating, "admin", "Updated rating through backend", false);
                    }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.PlayerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int schoolId = await _dataService.GetSchoolIdAsync(User);

            var player = await _context.Player
                .FirstOrDefaultAsync(m => m.PlayerId == id && m.PlayerSchoolId == schoolId);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Player.FindAsync(id);
            _context.Player.Remove(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Player.Any(e => e.PlayerId == id);
        }


    }
}
