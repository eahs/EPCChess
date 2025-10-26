
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers.Api.v1
{
    /// <summary>
    /// Represents an API error.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the key associated with the error.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the list of error messages.
        /// </summary>
        public List<string> Errors { get; set; }
    }
}