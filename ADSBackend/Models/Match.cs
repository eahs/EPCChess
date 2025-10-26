
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a match between two schools.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Gets or sets the ID of the match.
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a virtual (online) match.
        /// </summary>
        [DisplayName("Is Virtual?")]
        public bool IsVirtual { get; set; } = false;  // Is this a LiChess game?

        /// <summary>
        /// Gets or sets the clock time limit in seconds.
        /// </summary>
        [DisplayName("Clock Time Limit (seconds)")]
        public int ClockTimeLimit { get; set; } = 30 * 60;

        /// <summary>
        /// Gets or sets the clock increment per move in seconds.
        /// </summary>
        [DisplayName("Clock Per Move Increment (seconds)")]
        public int ClockIncrement { get; set; } = 0;

        /// <summary>
        /// Gets or sets the date of the match.
        /// </summary>
        [DisplayName("Match Date")]
        [DataType(DataType.Date)]
        public DateTime MatchDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the start time of the match.
        /// </summary>
        [DisplayName("Start Time")]
        [DataType(DataType.DateTime)]
        public DateTime MatchStartTime { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the match has started.
        /// </summary>
        public bool MatchStarted { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the home team's roster is locked.
        /// </summary>
        public bool HomeRosterLocked { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the away team's roster is locked.
        /// </summary>
        public bool AwayRosterLocked { get; set; }


        /// <summary>
        /// Gets or sets the ID of the home school.
        /// </summary>
        [DisplayName("Home School")]
        public int HomeSchoolId { get; set; }

        /// <summary>
        /// Gets or sets the home school.
        /// </summary>
        public School HomeSchool { get; set; }

        /// <summary>
        /// Gets or sets the ID of the away school.
        /// </summary>
        [DisplayName("Away School")]
        public int AwaySchoolId { get; set; }
        /// <summary>
        /// Gets or sets the away school.
        /// </summary>
        public School AwaySchool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match is completed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the home team.
        /// </summary>
        [DisplayName("Home Points")]
        public double HomePoints { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the away team.
        /// </summary>
        [DisplayName("Away Points")]
        public double AwayPoints { get; set; }

        /// <summary>
        /// Gets or sets the list of games in this match.
        /// </summary>
        public List<Game> Games { get; set; }
    }
}