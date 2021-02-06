using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int PlayerSchoolId { get; set; }
        public School PlayerSchool { get; set; }

        [DisplayName("First Name")]
        public String FirstName { get; set; }

        [DisplayName("Last Name")]
        public String LastName { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public int Rating { get; set; }
    }
}
