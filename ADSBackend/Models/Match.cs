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

        [DisplayName("Is Virtual?")]
        public bool IsVirtual { get; set; } = false;  // Is this a LiChess game?

        [DisplayName("Clock Time Limit (seconds)")]
        public int ClockTimeLimit { get; set; } = 30 * 60;

        [DisplayName("Clock Per Move Increment (seconds)")]
        public int ClockIncrement { get; set; } = 0;

        [DisplayName("Match Date")]
        [DataType(DataType.Date)]
        public DateTime MatchDate { get; set; } = DateTime.Now;

        [DisplayName("Start Time")]
        [DataType(DataType.DateTime)]
        public DateTime MatchStartTime { get; set; }
        public bool MatchStarted { get; set; }
        public bool HomeRosterLocked { get; set; }
        public bool AwayRosterLocked { get; set; }


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
