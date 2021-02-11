using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models.Identity;

namespace ADSBackend.Models.PlayViewModels
{
    public class MatchViewModel
    {
        public Match Match { get; set; }
        public ApplicationUser ViewingUser { get; set; }
    }
}
