
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.MatchSetupViewModels
{
    /// <summary>
    /// View model for managing a match.
    /// </summary>
    public class ManageViewModel
    {
        /// <summary>
        /// Gets or sets the match being managed.
        /// </summary>
        public Match Match { get; set; }
    }
}