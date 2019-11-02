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
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public MatchesController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: Matches
        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            var matches = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                                              .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                                              .Where(m => m.HomeSchool.SeasonId == currentSeason)
                                              .OrderBy(m => m.MatchDate)
                                              .ToListAsync();

            return View(matches);
        }

        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Match
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // GET: Matches/Create
        public async Task<IActionResult> Create()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            var schools = await _context.School.Select(x => x)
                                               .Where (s => s.SeasonId == currentSeason)
                                               .OrderBy(x => x.Name)
                                               .ToListAsync();

            ViewBag.Schools = new SelectList(schools, "SchoolId", "Name");

            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchDate,HomeSchoolId,AwaySchoolId")] Match match)
        {
            if (ModelState.IsValid)
            {
                match.HomePoints = 0;
                match.AwayPoints = 0;
                match.Completed = false;

                _context.Add(match);

                await _context.SaveChangesAsync();

                for (int board = 1; board <= 10; board++)
                {
                    Game g = new Game
                    {
                        MatchId = match.MatchId,
                        BoardPosition = board
                    };
                    _context.Add(g);

                }
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Match.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            var currentSeason = await _dataService.GetCurrentSeasonId();




            var schools = await _context.School.Select(x => x)
                                               .Where(s => s.SeasonId == currentSeason)
                                               .OrderBy(x => x.Name)
                                               .ToListAsync();

            ViewBag.Schools = new SelectList(schools, "SchoolId", "Name");

            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchId,MatchDate,HomeSchoolId,AwaySchoolId,Completed,HomePoints,AwayPoints")] Match match)
        {
            if (id != match.MatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _match = _context.Match.Find(id);

                    _match.MatchDate = match.MatchDate;
                    _match.HomeSchoolId = match.HomeSchoolId;
                    _match.AwaySchoolId = match.AwaySchoolId;
                    _match.Completed = match.Completed;
                    _match.HomePoints = match.HomePoints;
                    _match.AwayPoints = match.AwayPoints;

                    _context.Update(_match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.MatchId))
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
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Match
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Match.FindAsync(id);

            if (match == null)
                return RedirectToAction(nameof(Index));

            var ratingEvents = await _context.RatingEvent.Include(re => re.Game)
                                                         .Where(re => re.Game != null && re.Game.MatchId == match.MatchId)
                                                         .ToListAsync();

            foreach (var re in ratingEvents)
            {
                re.GameId = null;
                re.Type = "adjustment";
                re.Message = "Original game removed from history";
                _context.RatingEvent.Update(re);
            }

            _context.Match.Remove(match);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return _context.Match.Any(e => e.MatchId == id);
        }
    }
}
