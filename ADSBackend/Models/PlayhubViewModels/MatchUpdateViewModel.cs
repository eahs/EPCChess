using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.PlayhubViewModels
{
    public class MatchUpdateViewModel
    {
        public int MatchId { get; set; }
        public List<GameJson> Games { get; set; }
    }
}
