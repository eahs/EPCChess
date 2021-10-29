using ADSBackend.Models.Identity;
using System.Collections.Generic;

namespace ADSBackend.Models.HomeViewModels
{
    public class HomeViewModel
    {
        /// <summary>
        ///  Is this user a player in this season?
        /// </summary>
        public bool IsPlayerThisSeason { get; set; }

        public ApplicationUser User { get; set; }

        public School HomeSchool { get; set; }

        public List<Match> Upcoming { get; set; }

        public List<Division> Divisions { get; set; }

        public List<Player> TopSchoolPlayers { get; set; }

        public bool JoinCodeError { get; set; } = false;
    }
}
