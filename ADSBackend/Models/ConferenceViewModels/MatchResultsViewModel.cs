
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.ConferenceViewModels
{
    /// <summary>
    /// View model for displaying match results.
    /// </summary>
    public class MatchResultsViewModel
    {
        /// <summary>
        /// Gets or sets the list of divisions.
        /// </summary>
        public List<Division> Divisions { get; set; }
        /// <summary>
        /// Gets or sets the list of matches.
        /// </summary>
        public List<Match> Matches { get; set; }
    }
}