using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Configuration
{
    public class ApplicationDbSeed
    {
        ApplicationDbContext _context;

        public ApplicationDbSeed (ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateSeasons ()
        {
            var season = _context.Season.FirstOrDefault(m => m.SeasonId == 1);
            if (season == null)
            {
                season = new Season
                {
                    Name = "Unassigned",
                    StartDate = new DateTime(2000, 10, 1),
                    EndDate = new DateTime(2001, 3, 31)
                };

                _context.Add(season);
                _context.SaveChanges();
            }
        }

        public void CreateSchools ()
        {
            var school = _context.School.FirstOrDefault(m => m.SchoolId == 1);
            if (school == null)
            {
                school = new School
                {
                    SeasonId = 1,
                    Name = "Unassigned School",
                    ShortName = "Unassigned",
                    Abbreviation = "UN"
                };

                _context.Add(school);
                _context.SaveChanges();
            }
        }
    }
}
