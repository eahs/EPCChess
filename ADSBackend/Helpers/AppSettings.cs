
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models;

namespace ADSBackend.Helpers
{
    /// <summary>
    /// Represents application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the Lichess client credentials.
        /// </summary>
        public ClientCredentials Lichess { get; set; }
    }
}