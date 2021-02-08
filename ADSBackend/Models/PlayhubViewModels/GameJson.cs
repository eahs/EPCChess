using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.PlayhubViewModels
{
    public class GameJson
    {
        /// <summary>
        /// Id of game on Lichess
        /// </summary>
        public int MatchId { get; set; }
        public string GameId { get; set; }
        public string GameUrl { get; set; }

        /// <summary>
        /// Current board position in Fen notation
        /// </summary>
        public string Fen { get; set; }

        /// <summary>
        /// List of moves in SAN notation
        /// </summary>
        public List<String> Moves { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public DateTime LastMoveAt { get; set; }
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
    }
}
