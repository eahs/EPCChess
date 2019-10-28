using ADSBackend.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }

        public int? DivisionId { get; set; }
        public Division Division { get; set; }

        [DisplayName("School Name")]
        public String Name { get; set; }

        [DisplayName("Short Name")]
        [StringLength(12)]
        public String ShortName { get; set; }

        [DisplayName("Abbreviation")]
        [StringLength(2)]
        public String Abbreviation { get; set; }

        [DisplayName("Advisor Name")]
        public String AdvisorName { get; set; }

        [DisplayName("Advisor Email")]
        public String AdvisorEmail { get; set; }

        [DisplayName("Advisor Phone Number")]
        public String AdvisorPhoneNumber { get; set; }

        [DisplayName("Season")]
        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public List<Player> Players { get; set; }

        [NotMapped]
        public int Wins { get; set; } = 0;
        [NotMapped] 
        public int Losses { get; set; } = 0;
        [NotMapped] 
        public int Ties { get; set; } = 0;
        [NotMapped]
        public double Points
        {
            get
            {
                return Wins + (Ties * 0.5);
            }
        }
    }
}
