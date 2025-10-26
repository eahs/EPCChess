
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.MatchSetupViewModels
{
    /// <summary>
    /// View model for setting up a match.
    /// </summary>
    public class MatchSetupViewModel
    {
        /// <summary>
        /// Gets or sets the prefix for form elements.
        /// </summary>
        public String Prefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the roster is locked.
        /// </summary>
        public bool RosterLocked { get; set; }

        /// <summary>
        /// Gets or sets the match being set up.
        /// </summary>
        public Match Match { get; set; }

        /// <summary>
        /// Gets or sets the school associated with the setup.
        /// </summary>
        public School School { get; set; }

        /// <summary>
        /// Gets or sets the list of players assigned to the match.
        /// </summary>
        public List<Player> AssignedPlayers { get; set; }

        /// <summary>
        /// Gets or sets the list of remaining players available for the match.
        /// </summary>
        public List<Player> RemainingPlayers { get; set; }

        /// <summary>
        /// Gets or sets the list of players who are skipped for this match.
        /// </summary>
        public List<Player> SkippedPlayers { get; set; }
    }
}