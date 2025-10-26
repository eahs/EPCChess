
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models.AuthenticationModels
{
    /// <summary>
    /// Represents an authentication request.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}