using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
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
        public string AppToken { get; set; }
    }
}
