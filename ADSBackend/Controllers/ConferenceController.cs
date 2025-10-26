
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.ConferenceViewModels;
using ADSBackend.Models.PlayViewModels;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for conference-related views like match results and player standings.
    /// </summary>
    [Authorize(Roles = "Admin,Advisor,Player")]
    public class ConferenceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dataService">The data service.</param>
        public ConferenceController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }


        /// <summary>
        /// Redirects to the match results page by default.
        /// </summary>
        /// <returns>A redirect to the MatchResults action.</returns>
        // Show match results by default
        public IActionResult Index()
        {
            return RedirectToAction("MatchResults", "Conference");
        }

        /// <summary>
        /// Displays the match results for the current season.
        /// </summary>
        /// <returns>The match results view.</returns>
        // If schoolId is set it shows results just for that school
        public async Task<IActionResult> MatchResults()
        {
            int currentSeason = await _dataService.GetCurrentSeasonId();

            var divisions = await _dataService.GetDivisionStandingsAsync(currentSeason);

            divisions = divisions.Where(d => d.Schools.Count > 0).ToList();

            var matches = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                                              .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                                              .Where(m => m.HomeSchool.SeasonId == currentSeason && m.Completed)
                                              .OrderBy(m => m.MatchDate)
                                              .ToListAsync();

            return View(new MatchResultsViewModel { Matches = matches, Divisions = divisions });
        }

        /// <summary>
        /// Displays a list of all players in the current season, ordered by rating.
        /// </summary>
        /// <returns>The players view.</returns>
        public async Task<IActionResult> Players()
        {
            int currentSeason = await _dataService.GetCurrentSeasonId();

            var players = await _context.Player.Include(p => p.PlayerSchool)
                                               .Include(p => p.User)
                                               .Where(x => x.PlayerSchool.SeasonId == currentSeason)
                                               .OrderByDescending(x => x.Rating)
                                               .ToListAsync();


            return View(players);
        }

        /// <summary>
        /// Displays an overview of a specific completed match.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <returns>The match overview view, or a NotFound result if the match is not found or not completed.</returns>
        public async Task<IActionResult> MatchOverview(int? id)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var user = await _dataService.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var match = await _dataService.GetMatchAsync(id, currentSeason);

            if (match == null || !match.Completed)
            {
                return NotFound();
            }

            var chat = await _context.MatchChat.Where(m => m.MatchId == match.MatchId)
                .Include(m => m.Match)
                .Include(m => m.User)
                .OrderBy(m => m.MessageDate)
                .ToListAsync();

            MatchViewModel viewmodel = new MatchViewModel
            {
                Match = match,
                ViewingUser = user,
                Chat = chat ?? new List<MatchChat>()
            };

            return View(viewmodel);


        }

        /// <summary>
        /// Displays the profile of a player.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        /// <returns>The profile view.</returns>
        public async Task<IActionResult> Profile(int? id)
        {
            await Task.Delay(1);

            return View();
        }

    }
}