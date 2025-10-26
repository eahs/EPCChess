
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a player in a school.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the ID of the player.
        /// </summary>
        [Key]
        public int PlayerId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the user associated with this player.
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Gets or sets the user associated with this player.
        /// </summary>
        public ApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the ID of the school this player belongs to.
        /// </summary>
        public int PlayerSchoolId { get; set; }
        /// <summary>
        /// Gets or sets the school this player belongs to.
        /// </summary>
        public School PlayerSchool { get; set; }

        /// <summary>
        /// Gets or sets the first name of the player.
        /// </summary>
        [DisplayName("First Name")]
        public String FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the player.
        /// </summary>
        [DisplayName("Last Name")]
        public String LastName { get; set; }

        /// <summary>
        /// Gets or sets the number of wins for the player.
        /// </summary>
        public int Wins { get; set; }
        /// <summary>
        /// Gets or sets the number of losses for the player.
        /// </summary>
        public int Losses { get; set; }
        /// <summary>
        /// Gets or sets the number of draws for the player.
        /// </summary>
        public int Draws { get; set; }

        /// <summary>
        /// Gets or sets the rating of the player.
        /// </summary>
        public int Rating { get; set; }
    }
}