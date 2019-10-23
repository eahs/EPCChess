using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.MatchSetupViewModels
{
    public class MatchSetupViewModel
    {
        public String Prefix { get; set; }

        public bool RosterLocked { get; set; }

        public Match Match { get; set; }

        public School School { get; set; }

        public List<Player> AssignedPlayers { get; set; }

        public List<Player> RemainingPlayers { get; set; }
    }
}
