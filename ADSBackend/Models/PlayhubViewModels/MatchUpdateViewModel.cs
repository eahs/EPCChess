
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.PlayhubViewModels
{
    /// <summary>
    /// View model for sending match updates to clients.
    /// </summary>
    public class MatchUpdateViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the match.
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the list of games with their updated states.
        /// </summary>
        public List<GameJson> Games { get; set; }
    }
}