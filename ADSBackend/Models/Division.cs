
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a division within a season.
    /// </summary>
    public class Division
    {
        /// <summary>
        /// Gets or sets the ID of the division.
        /// </summary>
        [Key]
        public int DivisionId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the season this division belongs to.
        /// </summary>
        [DisplayName("Season")]
        public int? SeasonId { get; set; }
        /// <summary>
        /// Gets or sets the season this division belongs to.
        /// </summary>
        public Season Season { get; set; }

        /// <summary>
        /// Gets or sets the name of the division.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of schools in this division.
        /// </summary>
        public List<School> Schools { get; set; }
    }
}