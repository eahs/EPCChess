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

        [DisplayName("Starting Year")]
        public int StartingYear { get; set; }

        //public List<School> Schools { get; set; }
    }
}
