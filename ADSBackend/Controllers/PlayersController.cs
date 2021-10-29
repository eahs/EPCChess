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
            int currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.School = await _context.School.FirstOrDefaultAsync(x => x.SchoolId == schoolId);

            var players = await _context.Player.Include(p => p.PlayerSchool)
                                               .Where(x => x.PlayerSchoolId == schoolId && x.PlayerSchool.SeasonId == currentSeason)
                                               .OrderByDescending(x => x.Rating)
                                               .ToListAsync();


            return View(players);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

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
            int currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

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
            int currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

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

            int currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

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

            bool canDelete = User.IsInRole("Admin");

            if (!canDelete)
            {
                var editingUser = await _dataService.GetUserAsync(User);

                // Editing user can delete if that user goes to the same school as they do
                canDelete = editingUser.Schools.FirstOrDefault(s => s.SchoolId == player.PlayerSchoolId) is not null && User.IsInRole("Advisor");
            }

            if (canDelete)
            {
                if (player?.UserId is not null)
                {
                    var user = await _userManager.FindByIdAsync(player.UserId + "");

                    if (user is not null)
                    {
                        await _dataService.RemoveUserFromSchool(user, player.PlayerSchoolId);
                    }
                }

                player.PlayerSchoolId = 1;
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Player.Any(e => e.PlayerId == id);
        }


    }
}
