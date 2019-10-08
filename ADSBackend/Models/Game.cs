using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public Player White { get; set; }
        public Player Black { get; set; }

        public bool Completed { get; set; }

        public int Result { get; set; }  // -1 = White Wins, 0 = draw, 1 = Black Wins
    }
}
