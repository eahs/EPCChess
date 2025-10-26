
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.ManageViewModels
{
    /// <summary>
    /// View model for the two-factor authentication management page.
    /// </summary>
    public class TwoFactorAuthenticationViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user has an authenticator app configured.
        /// </summary>
        public bool HasAuthenticator { get; set; }

        /// <summary>
        /// Gets or sets the number of recovery codes remaining.
        /// </summary>
        public int RecoveryCodesLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether two-factor authentication is enabled.
        /// </summary>
        public bool Is2faEnabled { get; set; }
    }
}