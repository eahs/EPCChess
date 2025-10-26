
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ADSBackend.Models.ManageViewModels
{
    /// <summary>
    /// View model for enabling two-factor authentication.
    /// </summary>
    public class EnableAuthenticatorViewModel
    {
            /// <summary>
            /// Gets or sets the verification code from the authenticator app.
            /// </summary>
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }

            /// <summary>
            /// Gets or sets the shared key for the authenticator app.
            /// </summary>
            [BindNever]
            public string SharedKey { get; set; }

            /// <summary>
            /// Gets or sets the URI for the authenticator app QR code.
            /// </summary>
            [BindNever]
            public string AuthenticatorUri { get; set; }
    }
}