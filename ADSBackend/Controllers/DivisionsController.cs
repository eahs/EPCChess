
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
using ADSBackend.Services;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing divisions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DivisionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dataService">The data service.</param>
        public DivisionsController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        /// <summary>
        /// Displays a list of divisions for the current season.
        /// </summary>
        /// <returns>The index view with a list of divisions.</returns>
        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            return View(await _context.Division.Where(d => d.SeasonId == currentSeason).ToListAsync());
        }

        /// <summary>
        /// Displays the details of a specific division.
        /// </summary>
        /// <param name="id">The ID of the division.</param>
        /// <returns>The details view for the division.</returns>
        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _context.Division
                .FirstOrDefaultAsync(m => m.DivisionId == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        /// <summary>
        /// Displays the form for creating a new division.
        /// </summary>
        /// <returns>The create view.</returns>
        // GET: Divisions/Create
        public async Task<IActionResult> Create()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();


            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);


            return View();
        }

        /// <summary>
        /// Handles the creation of a new division.
        /// </summary>
        /// <param name="division">The division to create.</param>
        /// <returns>A redirect to the index page on success, or the create view with errors on failure.</returns>
        // POST: Divisions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DivisionId,Name,SeasonId")] Division division)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();


            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);


            if (ModelState.IsValid)
            {
                division.SeasonId = currentSeason;

                _context.Add(division);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(division);
        }

        /// <summary>
        /// Displays the form for editing an existing division.
        /// </summary>
        /// <param name="id">The ID of the division to edit.</param>
        /// <returns>The edit view for the division.</returns>
        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentSeason = await _dataService.GetCurrentSeasonId();

            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);

            var division = await _context.Division.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            return View(division);
        }

        /// <summary>
        /// Handles the update of an existing division.
        /// </summary>
        /// <param name="id">The ID of the division to edit.</param>
        /// <param name="division">The updated division data.</param>
        /// <returns>A redirect to the index page on success, or the edit view with errors on failure.</returns>
        // POST: Divisions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DivisionId,Name,SeasonId")] Division division)
        {
            if (id != division.DivisionId)
            {
                return NotFound();
            }

            var currentSeason = await _dataService.GetCurrentSeasonId();


            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.DivisionId))
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
            return View(division);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a division.
        /// </summary>
        /// <param name="id">The ID of the division to delete.</param>
        /// <returns>The delete confirmation view.</returns>
        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _context.Division
                .FirstOrDefaultAsync(m => m.DivisionId == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        /// <summary>
        /// Handles the deletion of a division.
        /// </summary>
        /// <param name="id">The ID of the division to delete.</param>
        /// <returns>A redirect to the index page.</returns>
        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var division = await _context.Division.FindAsync(id);
            _context.Division.Remove(division);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(int id)
        {
            return _context.Division.Any(e => e.DivisionId == id);
        }
    }
}