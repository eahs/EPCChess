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

        public int HomePlayerId { get; set; }
        public Player HomePlayer { get; set; }

        public int AwayPlayerId { get; set; }
        public Player AwayPlayer { get; set; }

        public int HomePlayerRatingBefore { get; set; }
        public int AwayPlayerRatingBefore { get; set; }
        public int HomePlayerRatingAfter { get; set; }
        public int AwayPlayerRatingAfter { get; set; }

        public bool Completed { get; set; }

        public int Result { get; set; }  // -1 = Away Wins, 0 = draw, 1 = Home Wins
    }
}
