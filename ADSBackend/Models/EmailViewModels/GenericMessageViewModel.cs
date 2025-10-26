
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.EmailViewModels
{
    /// <summary>
    /// View model for a generic email message.
    /// </summary>
    public class GenericMessageViewModel
    {
        /// <summary>
        /// Gets or sets the title of the email.
        /// </summary>
        public string Title { get; set; } = "";
        /// <summary>
        /// Gets or sets the heading of the message body.
        /// </summary>
        public string MessageHeading { get; set; } = "";
        /// <summary>
        /// Gets or sets the main body of the message.
        /// </summary>
        public string MessageBody { get; set; } = "";
        /// <summary>
        /// Gets or sets the text for the action link.
        /// </summary>
        public string ActionLinkTitle { get; set; } = "";
        /// <summary>
        /// Gets or sets the URL for the action link.
        /// </summary>
        public string ActionLink { get; set; } = "";
    }
}