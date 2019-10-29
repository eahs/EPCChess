using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.ConferenceViewModels
{
    public class MatchResultsViewModel
    {
        public List<Division> Divisions { get; set; };
        public List<Match> Matches { get; set; }
    }
}
