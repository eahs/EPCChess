using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authorization;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SchoolsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public SchoolsController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            var schools = await _context.School.Where(m => m.SchoolId != 1 && m.SeasonId == currentSeason)
                                               .Include(m => m.Season)
                                               .Include(m => m.Division)
                                               .OrderBy(m => m.Season.StartDate)
                                               .ThenBy(m => m.DivisionId)
                                               .ThenBy(m => m.Name)
                                               .ToListAsync();

            return View(schools);
        }

        public async Task<School> GetModel (int ?id)
        {
            if (id == null)
                return null;

            var school = await _context.School.FirstOrDefaultAsync(m => m.SchoolId == id);
                                              
            if (school == null)
            {
                return null;
            }

            return school;

        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            School model = await GetModel(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Schools/Create
        public async Task<IActionResult> Create()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name", currentSeason);

            ViewBag.Divisions = new SelectList(await _context.Division.Select(x => x).Where(d => d.SeasonId == currentSeason)
                .OrderByDescending(x => x.Name)
                .ToListAsync(), "DivisionId", "Name");

            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeasonId,DivisionId,SchoolId,Name,ShortName,Abbreviation,AdivisorName,AdvisorEmail,AdvisorPhoneNumber")] School school)
        {
            if (ModelState.IsValid)
            {
                if (school.Abbreviation != null)
                {
                    school.Abbreviation = school.Abbreviation.ToUpper();
                    if (school.Abbreviation.Length > 3)
                        school.Abbreviation = school.Abbreviation.Substring(0, 3);
                }

                _context.Add(school);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        // GET: Schools/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentSeason = await _dataService.GetCurrentSeasonId();

            var school = await _context.School.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name");

            ViewBag.Divisions = new SelectList(await _context.Division.Select(x => x)
                .Where(d => d.SeasonId == currentSeason)
                .OrderByDescending(x => x.Name)
                .ToListAsync(), "DivisionId", "Name");

            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeasonId,DivisionId,SchoolId,Name,ShortName,Abbreviation,AdvisorName,AdvisorEmail,AdvisorPhoneNumber")] School school)
        {
            if (id != school.SchoolId)
            {
                return NotFound();
            }

            var _school = await _context.School.FirstOrDefaultAsync(s => s.SchoolId == id);

            if (_school != null && ModelState.IsValid)
            {
                _school.SeasonId = school.SeasonId;
                _school.DivisionId = school.DivisionId;
                _school.Name = school.Name;
                _school.ShortName = school.ShortName;
                _school.Abbreviation = school.Abbreviation;
                _school.AdvisorName = school.AdvisorName;
                _school.AdvisorEmail = school.AdvisorEmail;
                _school.AdvisorPhoneNumber = school.AdvisorPhoneNumber;

                if (school.Abbreviation != null)
                {
                    _school.Abbreviation = school.Abbreviation.ToUpper();

                    if (school.Abbreviation.Length > 3)
                        school.Abbreviation = school.Abbreviation.Substring(0, 3);

                }

                try
                {
                    _context.Update(_school);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.SchoolId))
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
            return View(school);
        }

        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.School
                .FirstOrDefaultAsync(m => m.SchoolId == id);
            if (school == null)
            {
                return NotFound();
            }

            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x).ToListAsync(), "SeasonId", "Name");

            return View(school);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var school = await _context.School.FindAsync(id);
            _context.School.Remove(school);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchoolExists(int id)
        {
            return _context.School.Any(e => e.SchoolId == id);
        }
    }
}
