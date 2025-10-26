
using System;
using System.Collections.Generic;

namespace ADSBackend.Models.ApiModels
{
    /// <summary>
    /// Represents an item in a news feed.
    /// </summary>
    public class NewsFeedItem
    {
        /// <summary>
        /// Gets or sets the ID of the news item.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the title of the news item.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the link to the full news item.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Gets or sets the publication date of the news item.
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Gets or sets the author of the news item.
        /// </summary>
        public string Author { get; set; }

        private string description;
        /// <summary>
        /// Gets or sets the description of the news item.
        /// </summary>
        public string Description
        {
            get {
                return description;
            }
            set {
                description = value.Replace("<br>", "\n");
            }
        }
    }
}