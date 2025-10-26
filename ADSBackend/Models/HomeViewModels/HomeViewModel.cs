
using ADSBackend.Models.Identity;
using System.Collections.Generic;

namespace ADSBackend.Models.HomeViewModels
{
    /// <summary>
    /// View model for the home page/dashboard.
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        ///  Gets or sets a value indicating whether this user is a player in this season.
        /// </summary>
        public bool IsPlayerThisSeason { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the user's home school.
        /// </summary>
        public School HomeSchool { get; set; }

        /// <summary>
        /// Gets or sets the list of upcoming matches.
        /// </summary>
        public List<Match> Upcoming { get; set; }

        /// <summary>
        /// Gets or sets the list of divisions with their standings.
        /// </summary>
        public List<Division> Divisions { get; set; }

        /// <summary>
        /// Gets or sets the list of top players for the user's school.
        /// </summary>
        public List<Player> TopSchoolPlayers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there was an error with the join code.
        /// </summary>
        public bool JoinCodeError { get; set; } = false;
    }
}