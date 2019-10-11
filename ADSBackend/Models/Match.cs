using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Match
    {
        public int MatchId { get; set; }

        public DateTime MatchDate { get; set; }
        public DateTime MatchStartTime { get; set; }

        public int HomeSchoolId { get; set; }
        public School HomeSchool { get; set; }

        public int AwaySchoolId { get; set; }
        public School AwaySchool { get; set; }

        public bool Completed { get; set; }

        public double HomePoints { get; set; }
        public double AwayPoints { get; set; }

        public List<Game> Games { get; set; }
    }
}
