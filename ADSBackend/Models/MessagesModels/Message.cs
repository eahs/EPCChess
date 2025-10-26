
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.MessagesModels
{
    /// <summary>
    /// Represents a message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the ID of the message.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the title of the message.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the link associated with the message.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Gets or sets the publication date of the message.
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Gets or sets the author of the message.
        /// </summary>
        public string Author { get; set; }

        private string description;
        /// <summary>
        /// Gets or sets the description of the message.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value.Replace("<br>", "\n");
            }
        }
    }
}