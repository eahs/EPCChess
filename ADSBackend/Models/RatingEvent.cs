using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class RatingEvent
    {
        [Key]
        public int RatingEventId { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int Rating { get; set; }

        public int? GameId { get; set; }  // Nullable to allow non game-related events
        public Game Game { get; set; }

        public string Type { get; set; }  // "game", "adjustment", "admin" (administrative change), etc.

        public string Message { get; set; }  // Any extra message that needs to be logged

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
