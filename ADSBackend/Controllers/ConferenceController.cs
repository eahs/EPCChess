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
    [Authorize(Roles = "Admin,Advisor,Player")]
    public class ConferenceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public ConferenceController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }


        // Show match results by default
        public IActionResult Index()
        {
            return RedirectToAction("MatchResults", "Conference");
        }

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

        public async Task<IActionResult> Profile(int? id)
        {
            await Task.Delay(1);

            return View();
        }

    }
}