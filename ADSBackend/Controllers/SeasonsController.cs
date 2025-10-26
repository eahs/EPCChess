
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

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing seasons.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class SeasonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SeasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all seasons.
        /// </summary>
        /// <returns>The index view with a list of seasons.</returns>
        // GET: Seasons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Season.Where(m => m.SeasonId != 1).ToListAsync());
        }

        /// <summary>
        /// Displays the details of a specific season.
        /// </summary>
        /// <param name="id">The ID of the season.</param>
        /// <returns>The details view for the season.</returns>
        // GET: Seasons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season
                .FirstOrDefaultAsync(m => m.SeasonId == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        /// <summary>
        /// Displays the form for creating a new season.
        /// </summary>
        /// <returns>The create view.</returns>
        // GET: Seasons/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation of a new season.
        /// </summary>
        /// <param name="season">The season to create.</param>
        /// <returns>A redirect to the index page on success, or the create view with errors on failure.</returns>
        // POST: Seasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeasonId,Name,StartDate,EndDate")] Season season)
        {
            if (ModelState.IsValid)
            {
                _context.Add(season);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        /// <summary>
        /// Displays the form for editing an existing season.
        /// </summary>
        /// <param name="id">The ID of the season to edit.</param>
        /// <returns>The edit view for the season.</returns>
        // GET: Seasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season.FindAsync(id);
            if (season == null)
            {
                return NotFound();
            }
            return View(season);
        }

        /// <summary>
        /// Handles the update of an existing season.
        /// </summary>
        /// <param name="id">The ID of the season to edit.</param>
        /// <param name="season">The updated season data.</param>
        /// <returns>A redirect to the index page on success, or the edit view with errors on failure.</returns>
        // POST: Seasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeasonId,Name,StartDate,EndDate")] Season season)
        {
            if (id != season.SeasonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(season);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeasonExists(season.SeasonId))
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
            return View(season);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a season.
        /// </summary>
        /// <param name="id">The ID of the season to delete.</param>
        /// <returns>The delete confirmation view.</returns>
        // GET: Seasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season
                .FirstOrDefaultAsync(m => m.SeasonId == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        /// <summary>
        /// Handles the deletion of a season.
        /// </summary>
        /// <param name="id">The ID of the season to delete.</param>
        /// <returns>A redirect to the index page.</returns>
        // POST: Seasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _context.Season.FindAsync(id);
            _context.Season.Remove(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonExists(int id)
        {
            return _context.Season.Any(e => e.SeasonId == id);
        }
    }
}