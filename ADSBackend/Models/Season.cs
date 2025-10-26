
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a season.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Gets or sets the ID of the season.
        /// </summary>
        [Key]
        public int SeasonId { get; set; }

        /// <summary>
        /// Gets or sets the name of the season.
        /// </summary>
        [DisplayName("Season Name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the start date of the season.
        /// </summary>
        [DisplayName("Starting Date")]
        [Required(ErrorMessage = "Enter a starting date for the season")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the season.
        /// </summary>
        [Required(ErrorMessage = "Enter an ending date for the season")]
        [DataType(DataType.Date)]
        [DisplayName("Ending Date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the list of schools participating in this season.
        /// </summary>
        public List<School> Schools { get; set; }
    }
}