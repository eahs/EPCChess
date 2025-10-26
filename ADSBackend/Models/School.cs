
using ADSBackend.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Util;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a school.
    /// </summary>
    public class School
    {
        /// <summary>
        /// Gets or sets the ID of the school.
        /// </summary>
        [Key]
        public int SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the division this school belongs to.
        /// </summary>
        public int? DivisionId { get; set; }
        /// <summary>
        /// Gets or sets the division this school belongs to.
        /// </summary>
        public Division Division { get; set; }

        /// <summary>
        /// Gets or sets the name of the school.
        /// </summary>
        [DisplayName("School Name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the short name of the school.
        /// </summary>
        [DisplayName("Short Name")]
        [StringLength(12)]
        public String ShortName { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation of the school.
        /// </summary>
        [DisplayName("Abbreviation")]
        [StringLength(3)]
        public String Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets the name of the advisor.
        /// </summary>
        [DisplayName("Advisor Name")]
        public String AdvisorName { get; set; }

        /// <summary>
        /// Gets or sets the email of the advisor.
        /// </summary>
        [DisplayName("Advisor Email")]
        public String AdvisorEmail { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the advisor.
        /// </summary>
        [DisplayName("Advisor Phone Number")]
        public String AdvisorPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID of the season this school is participating in.
        /// </summary>
        [DisplayName("Season")]
        public int SeasonId { get; set; }
        /// <summary>
        /// Gets or sets the season this school is participating in.
        /// </summary>
        public Season Season { get; set; }

        /// <summary>
        /// Gets or sets the join code for the school.
        /// </summary>
        [DisplayName("Join Code")] public string JoinCode { get; set; } = RandomIdGenerator.Generate(8);

        /// <summary>
        /// Gets or sets the list of users associated with this school.
        /// </summary>
        public List<UserSchool> Users { get; set; }
        /// <summary>
        /// Gets or sets the list of players in this school.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Gets or sets the number of wins for the school.
        /// </summary>
        [NotMapped]
        public int Wins { get; set; } = 0;
        /// <summary>
        /// Gets or sets the number of losses for the school.
        /// </summary>
        [NotMapped] 
        public int Losses { get; set; } = 0;
        /// <summary>
        /// Gets or sets the number of ties for the school.
        /// </summary>
        [NotMapped] 
        public int Ties { get; set; } = 0;
        /// <summary>
        /// Gets the total points for the school.
        /// </summary>
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