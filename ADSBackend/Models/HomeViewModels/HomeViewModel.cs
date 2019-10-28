using ADSBackend.Models.Identity;
using System.Collections.Generic;

namespace ADSBackend.Models.HomeViewModels
{
    public class HomeViewModel
    {
        public ApplicationUser User { get; set; }

        public School HomeSchool { get; set; }

        public List<Match> Upcoming { get; set; }

        public List<Division> Divisions { get; set; }

        public List<Player> TopSchoolPlayers { get; set; }
    }
}
