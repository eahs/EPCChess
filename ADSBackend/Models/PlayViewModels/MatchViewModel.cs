
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models.PlayViewModels
{
    /// <summary>
    /// View model for displaying a match.
    /// </summary>
    public class MatchViewModel
    {
        /// <summary>
        /// Gets or sets the match details.
        /// </summary>
        public Match Match { get; set; }
        /// <summary>
        /// Gets or sets the user who is viewing the match.
        /// </summary>
        public ApplicationUser ViewingUser { get; set; }
        /// <summary>
        /// Gets or sets the list of chat messages for the match.
        /// </summary>
        public List<MatchChat> Chat { get; set; }
    }
}