
using ADSBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Util
{
    /*
    public class SeasonSelector
    {
        public static async Task<int> GetCurrentSeasonId(ApplicationDbContext _context, HttpContext _httpcontext)
        {
            var season = await _context.Season.FirstOrDefaultAsync(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);
           
            var defaultSeason = season?.SeasonId ?? 1;

            var seasonId = _httpcontext.Session.GetInt32("SeasonId") ?? defaultSeason;
            
            //var seasonsSelect = new SelectList(seasons, "SeasonId", "Name", seasonId);

            _httpcontext.Session.SetInt32("SeasonId", seasonId);

            return seasonId;
        }

        public static async Task<SelectList> GetSeasonSelectList (ApplicationDbContext _context, int currentSeasonId)
        {
            var seasons = await _context.Season.Select(x => x)
                                         .OrderByDescending(x => x.StartDate)
                                         .ToListAsync();

            return new SelectList(seasons, "SeasonId", "Name", currentSeasonId);
        }
    }
    */
}