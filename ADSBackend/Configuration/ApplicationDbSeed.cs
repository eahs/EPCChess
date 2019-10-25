using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public string GetJson (string seedFile)
        {
            var file = System.IO.File.ReadAllText(@"Configuration\SeedData\" + seedFile);
            return file;
        }

        public void SeedDatabase<TEntity> (string jsonFile, DbSet<TEntity> dbset) where TEntity : class
        {
            var records = JsonConvert.DeserializeObject<List<TEntity>>(GetJson(jsonFile));

            if (records?.Count > 0)
            {
                records.ForEach(s => dbset.Add(s));
                _context.SaveChanges();
                
            }
        }

        public void CreateSeasons ()
        {
            var season = _context.Season.FirstOrDefault(m => m.SeasonId == 1);
            if (season == null)
            {
                SeedDatabase<Season>("seasons.json", _context.Season);
            }

        }

        public void CreateSchools ()
        {
            var school = _context.School.FirstOrDefault(m => m.SchoolId == 1);
            if (school == null)
            {
                SeedDatabase<School>("schools.json", _context.School);
            }
        }

        public void CreateMatches()
        {
            var match = _context.Match.FirstOrDefault();
            if (match == null)
            {
                SeedDatabase<Match>("matches.json", _context.Match);
            }
        }

    }
}
