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
    [Authorize(Roles = "Admin")]
    public class SchoolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchoolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            var schools = await _context.School.Include(m => m.Season)
                                               .OrderBy(m => m.Season.StartDate)
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
            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name");

            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeasonId,SchoolId,Name,ShortName,Abbreviation,AdivisorName,AdvisorEmail,AdvisorPhoneNumber")] School school)
        {
            if (ModelState.IsValid)
            {
                if (school.Abbreviation != null)
                {
                    school.Abbreviation = school.Abbreviation.ToUpper();
                    if (school.Abbreviation.Length > 2)
                        school.Abbreviation = school.Abbreviation.Substring(0, 2);
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

            var school = await _context.School.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            ViewBag.Seasons = new SelectList(await _context.Season.Select(x => x)
                                                                  .OrderByDescending(x => x.StartDate)
                                                                  .ToListAsync(), "SeasonId", "Name");

            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeasonId,SchoolId,Name,ShortName,Abbreviation,AdivisorName,AdvisorEmail,AdvisorPhoneNumber")] School school)
        {
            if (id != school.SchoolId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (school.Abbreviation != null)
                {
                    school.Abbreviation = school.Abbreviation.ToUpper();
                    if (school.Abbreviation.Length > 2)
                        school.Abbreviation = school.Abbreviation.Substring(0, 2);
                }

                try
                {
                    _context.Update(school);
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
