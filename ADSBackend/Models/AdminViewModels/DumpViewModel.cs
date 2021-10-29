using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.AdminViewModels
{
    public class DumpViewModel
    {
        public List<object> Matches { get; set; }
        public List<object> Schools { get; set; }
        public List<object> Divisions { get; set; }
        public List<object> Seasons { get; set; }
    }
}
