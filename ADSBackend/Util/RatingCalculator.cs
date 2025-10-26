
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Util
{
    /// <summary>
    /// Represents the result of a game.
    /// </summary>
    public enum GameResult
    {
        /// <summary>
        /// The game was a draw.
        /// </summary>
        Draw = 0,
        /// <summary>
        /// Player 1 won.
        /// </summary>
        Player1Wins = 1,
        /// <summary>
        /// Player 2 won.
        /// </summary>
        Player2Wins = 2,
        /// <summary>
        /// The game result was reset.
        /// </summary>
        Reset = 3
    }

    /// <summary>
    /// Represents an entry in the rating lookup table.
    /// </summary>
    public class RatingLookupEntry
    {
        /// <summary>
        /// Gets or sets the lower bound of the rating difference.
        /// </summary>
        public int Low { get; set; }
        /// <summary>
        /// Gets or sets the upper bound of the rating difference.
        /// </summary>
        public int High { get; set; }
        /// <summary>
        /// Gets or sets the rating points gained by the higher-rated player in a win.
        /// </summary>
        public int HighBoardGains { get; set; }
        /// <summary>
        /// Gets or sets the rating points gained by the lower-rated player in a win.
        /// </summary>
        public int LowBoardGains { get; set; }
        /// <summary>
        /// Gets or sets the rating points exchanged in a draw.
        /// </summary>
        public int DrawHighLosesLowGains { get; set; }
    }

    /// <summary>
    /// Calculates new player ratings based on game results.
    /// </summary>
    public class RatingCalculator
    {
        private static RatingCalculator _ratingCalculator;
        private List<RatingLookupEntry> RatingLookup;

        private RatingCalculator ()
        {
            RatingLookup = new List<RatingLookupEntry>();
            RatingLookup.Add(new RatingLookupEntry { Low = 0, High = 12, HighBoardGains = 16, LowBoardGains = 16, DrawHighLosesLowGains = 0 });

            for (int i = 1; i <= 14; i++)
            {
                RatingLookupEntry entry = RatingLookup[i - 1];

                int low = entry.High + 1;
                int high = low + 24;
                int hbg = entry.HighBoardGains - 1;
                int lbg = entry.LowBoardGains + 1;
                int draw = entry.DrawHighLosesLowGains + 1;
                
                RatingLookup.Add(new RatingLookupEntry { Low = low, High = high, HighBoardGains = hbg, LowBoardGains = lbg, DrawHighLosesLowGains = draw });

                RatingLookup[RatingLookup.Count - 1].High = 2000;
            }
        }

        /// <summary>
        /// Calculates the new ratings for two players after a game.
        /// </summary>
        /// <param name="rating1">The rating of the first player.</param>
        /// <param name="rating2">The rating of the second player.</param>
        /// <param name="winner">The result of the game.</param>
        /// <param name="newRating1">The new rating of the first player.</param>
        /// <param name="newRating2">The new rating of the second player.</param>
        // Winner is either 0 or 1 or 2
        public void CalculateNewRating (int rating1, int rating2, GameResult winner, out int newRating1, out int newRating2)
        {
            int diff = Math.Abs(rating1 - rating2);
            RatingLookupEntry lookup = RatingLookup.Where(entry => entry.Low <= diff && entry.High >= diff).FirstOrDefault();

            newRating1 = rating1;
            newRating2 = rating2;

            if (lookup != null)
            {
                if (winner == GameResult.Player1Wins)
                {
                    if (rating1 > rating2)
                    {
                        newRating1 = rating1 + lookup.HighBoardGains;
                        newRating2 = rating2 - lookup.HighBoardGains;
                    }
                    else
                    {
                        newRating1 = rating1 + lookup.LowBoardGains;
                        newRating2 = rating2 - lookup.LowBoardGains;
                    }
                }
                else if (winner == GameResult.Player2Wins)
                {
                    if (rating2 > rating1)
                    {
                        newRating1 = rating1 - lookup.HighBoardGains;
                        newRating2 = rating2 + lookup.HighBoardGains;
                    }
                    else
                    {
                        newRating1 = rating1 - lookup.LowBoardGains;
                        newRating2 = rating2 + lookup.LowBoardGains;
                    }

                }
                else  // Draw
                {
                    if (rating1 > rating2)
                    {
                        newRating1 = rating1 - lookup.DrawHighLosesLowGains;
                        newRating2 = rating2 + lookup.DrawHighLosesLowGains;
                    }
                    else
                    {
                        newRating1 = rating1 + lookup.DrawHighLosesLowGains;
                        newRating2 = rating2 - lookup.DrawHighLosesLowGains;
                    }
                }
            }

        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="RatingCalculator"/>.
        /// </summary>
        public static RatingCalculator Current 
        {
            get {
                if (_ratingCalculator == null)
                    _ratingCalculator = new RatingCalculator();

                return _ratingCalculator;
            } 
        }
    }
}