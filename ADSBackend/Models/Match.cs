using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Match
    {
        public int MatchId { get; set; }

        [DisplayName("Match Date")]
        [DataType(DataType.Date)]
        public DateTime MatchDate { get; set; }

        [DisplayName("Start Time")]
        [DataType(DataType.DateTime)]
        public DateTime MatchStartTime { get; set; }


        [DisplayName("Home School")]
        public int HomeSchoolId { get; set; }

        public School HomeSchool { get; set; }

        [DisplayName("Away School")]
        public int AwaySchoolId { get; set; }
        public School AwaySchool { get; set; }

        public bool Completed { get; set; }

        [DisplayName("Home Points")]
        public double HomePoints { get; set; }

        [DisplayName("Away Points")]
        public double AwayPoints { get; set; }

        public List<Game> Games { get; set; }
    }
}
