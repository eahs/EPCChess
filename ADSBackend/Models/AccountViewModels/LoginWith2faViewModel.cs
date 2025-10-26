
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.AccountViewModels
{
    /// <summary>
    /// View model for logging in with two-factor authentication.
    /// </summary>
    public class LoginWith2faViewModel
    {
        /// <summary>
        /// Gets or sets the two-factor authentication code.
        /// </summary>
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remember this machine.
        /// </summary>
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remember the user.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}