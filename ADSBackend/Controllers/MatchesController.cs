
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
    /// Controller for managing matches.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchesController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dataService">The data service.</param>
        public MatchesController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        /// <summary>
        /// Displays a list of matches for the current season.
        /// </summary>
        /// <returns>The index view with a list of matches.</returns>
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

        /// <summary>
        /// Displays the details of a specific match.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <returns>The details view for the match.</returns>
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

        /// <summary>
        /// Displays the form for creating a new match.
        /// </summary>
        /// <returns>The create view.</returns>
        // GET: Matches/Create
        public async Task<IActionResult> Create()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            var schools = await _context.School.Select(x => x)
                                               .Where (s => s.SeasonId == currentSeason)
                                               .OrderBy(x => x.Name)
                                               .ToListAsync();

            ViewBag.Schools = new SelectList(schools, "SchoolId", "Name");

            return View(new Match());
        }

        /// <summary>
        /// Handles the creation of a new match.
        /// </summary>
        /// <param name="match">The match to create.</param>
        /// <returns>A redirect to the index page on success, or the create view with errors on failure.</returns>
        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchDate,HomeSchoolId,AwaySchoolId,IsVirtual,ClockIncrement,ClockTimeLimit")] Match match)
        {
            if (ModelState.IsValid)
            {
                match.HomePoints = 0;
                match.AwayPoints = 0;
                match.Completed = false;

                _context.Add(match);

                await _context.SaveChangesAsync();

                for (int board = 1; board <= 12; board++)
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

        /// <summary>
        /// Displays the form for editing an existing match.
        /// </summary>
        /// <param name="id">The ID of the match to edit.</param>
        /// <returns>The edit view for the match.</returns>
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

        /// <summary>
        /// Handles the update of an existing match.
        /// </summary>
        /// <param name="id">The ID of the match to edit.</param>
        /// <param name="match">The updated match data.</param>
        /// <returns>A redirect to the index page on success, or the edit view with errors on failure.</returns>
        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchId,MatchDate,HomeSchoolId,AwaySchoolId,Completed,HomePoints,AwayPoints,IsVirtual,ClockIncrement,ClockTimeLimit")] Match match)
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
                    _match.IsVirtual = match.IsVirtual;
                    _match.ClockIncrement = match.ClockIncrement;
                    _match.ClockTimeLimit = match.ClockTimeLimit;

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

        /// <summary>
        /// Displays the confirmation page for deleting a match.
        /// </summary>
        /// <param name="id">The ID of the match to delete.</param>
        /// <returns>The delete confirmation view.</returns>
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

        /// <summary>
        /// Handles the deletion of a match.
        /// </summary>
        /// <param name="id">The ID of the match to delete.</param>
        /// <returns>A redirect to the index page.</returns>
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