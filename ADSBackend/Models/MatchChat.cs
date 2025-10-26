
using ADSBackend.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a chat message within a match.
    /// </summary>
    public class MatchChat
    {
        /// <summary>
        /// Gets or sets the ID of the chat message.
        /// </summary>
        [Key]
        public int MatchChatId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the match this message belongs to.
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the match this message belongs to.
        /// </summary>
        public Match Match { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who sent the message.
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Gets or sets the user who sent the message.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the date and time the message was sent.
        /// </summary>
        public DateTime MessageDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether the message has been deleted.
        /// </summary>
        public bool Deleted { get; set; }
    }
}