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
    [Authorize(Roles = "Admin")]
    public class DivisionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public DivisionsController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            return View(await _context.Division.Where(d => d.SeasonId == currentSeason).ToListAsync());
        }

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

        // GET: Divisions/Create
        public async Task<IActionResult> Create()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();


            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);


            return View();
        }

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
