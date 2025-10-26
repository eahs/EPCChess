
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.AdminViewModels
{
    /// <summary>
    /// View model for dumping database content.
    /// </summary>
    public class DumpViewModel
    {
        /// <summary>
        /// Gets or sets the list of matches.
        /// </summary>
        public List<object> Matches { get; set; }
        /// <summary>
        /// Gets or sets the list of schools.
        /// </summary>
        public List<object> Schools { get; set; }
        /// <summary>
        /// Gets or sets the list of divisions.
        /// </summary>
        public List<object> Divisions { get; set; }
        /// <summary>
        /// Gets or sets the list of seasons.
        /// </summary>
        public List<object> Seasons { get; set; }
    }
}