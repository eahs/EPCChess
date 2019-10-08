using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        public School PlayerSchool { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int Rating { get; set; }
    }
}
