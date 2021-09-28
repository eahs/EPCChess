using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models
{
    public class UserSchool
    {
        [Key]
        public int UserSchoolId { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int SchoolId { get; set; }
        public School School { get; set; }
    }
}
