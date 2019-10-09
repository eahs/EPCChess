using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Season
    {
        [Key]
        public int SeasonId { get; set; }

        [DisplayName("Season Name")]
        public String Name { get; set; }

        [DisplayName("Starting Date")]
        [Required(ErrorMessage = "Enter a starting date for the season")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Enter an ending date for the season")]
        [DataType(DataType.Date)]
        [DisplayName("Ending Date")]
        public DateTime EndDate { get; set; }

        public List<School> Schools { get; set; }
    }
}
