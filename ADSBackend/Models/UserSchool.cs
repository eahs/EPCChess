
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents the many-to-many relationship between users and schools.
    /// </summary>
    public class UserSchool
    {
        /// <summary>
        /// Gets or sets the ID of the user-school relationship.
        /// </summary>
        [Key]
        public int UserSchoolId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public ApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the ID of the school.
        /// </summary>
        public int SchoolId { get; set; }
        /// <summary>
        /// Gets or sets the school.
        /// </summary>
        public School School { get; set; }
    }
}