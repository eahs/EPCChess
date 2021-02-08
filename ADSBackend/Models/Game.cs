using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        public int BoardPosition { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public int? HomePlayerId { get; set; }
        public Player HomePlayer { get; set; }

        public int? AwayPlayerId { get; set; }
        public Player AwayPlayer { get; set; }

        public int HomePlayerRatingBefore { get; set; }
        public int AwayPlayerRatingBefore { get; set; }
        public int HomePlayerRatingAfter { get; set; }
        public int AwayPlayerRatingAfter { get; set; }

        public bool Completed { get; set; }
        public DateTime CompletedDate { get; set; }

        public double HomePoints { get; set; }
        public double AwayPoints { get; set; }

        public bool IsStarted { get; set; } = false;
        public string ChallengeJson { get; set; } = "{}";  // Json response for challenge
        public string ChallengeId { get; set; }  // Id of Game
        public string ChallengeUrl { get; set; } // Url of Game on Lichess
        public string GameJson { get; set; } = "{}";
        public string CurrentFen { get; set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public DateTime LastGameExportTime { get; set; } = DateTime.Now.AddDays(-5);

        [NotMapped]
        public string HomePlayerFullName => $"{HomePlayer?.FirstName} {HomePlayer?.LastName}";

        [NotMapped]
        public string AwayPlayerFullName => $"{AwayPlayer?.FirstName} {AwayPlayer?.LastName}";

        [NotMapped]
        public string HomePlayerGameRating
        {
            get
            {
                if (HomePlayer != null)
                {
                    if ((HomePoints+AwayPoints) == 0)
                        return $"({HomePlayerRatingBefore})";
                    else
                        return $"({HomePlayerRatingBefore} -> {HomePlayerRatingAfter})";
                }

                return "";
            }
        }

        [NotMapped]
        public string AwayPlayerGameRating
        {
            get
            {
                if (AwayPlayer != null)
                {
                    if ((HomePoints + AwayPoints) == 0)
                        return $"({AwayPlayerRatingBefore})";
                    else
                        return $"({AwayPlayerRatingBefore} -> {AwayPlayerRatingAfter})";
                }

                return "";
            }
        }


    }
}
