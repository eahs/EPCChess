
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents client credentials for OAuth authentication.
    /// </summary>
    public class ClientCredentials
    {
        /// <summary>
        /// OBSOLETE: No longer used by LiChess
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// OBSOLETE: No longer used by LiChess
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Gets or sets the application token.
        /// </summary>
        public string AppToken { get; set; }
    }
}