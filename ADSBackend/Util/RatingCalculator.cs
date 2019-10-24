using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Util
{
    public enum GameResult
    {
        Draw,
        Player1Wins,
        Player2Wins
    }

    public class RatingLookupEntry
    {
        public int Low { get; set; }
        public int High { get; set; }
        public int HighBoardGains { get; set; }
        public int LowBoardGains { get; set; }
        public int DrawHighLosesLowGains { get; set; }
    }

    public class RatingCalculator
    {
        private static RatingCalculator _ratingCalculator;
        private List<RatingLookupEntry> RatingLookup;

        private RatingCalculator ()
        {
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
