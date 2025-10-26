
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents an event that caused a player's rating to change.
    /// </summary>
    public class RatingEvent
    {
        /// <summary>
        /// Gets or sets the ID of the rating event.
        /// </summary>
        [Key]
        public int RatingEventId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the player.
        /// </summary>
        public int PlayerId { get; set; }
        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the new rating after the event.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the ID of the game that caused this event.
        /// </summary>
        public int? GameId { get; set; }  // Nullable to allow non game-related events
        /// <summary>
        /// Gets or sets the game that caused this event.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets the type of the event (e.g., "game", "adjustment", "admin").
        /// </summary>
        public string Type { get; set; }  // "game", "adjustment", "admin" (administrative change), etc.

        /// <summary>
        /// Gets or sets an extra message to be logged with the event.
        /// </summary>
        public string Message { get; set; }  // Any extra message that needs to be logged

        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;
    }
}