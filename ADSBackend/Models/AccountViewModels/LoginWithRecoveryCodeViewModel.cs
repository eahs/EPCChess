
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.AccountViewModels
{
    /// <summary>
    /// View model for logging in with a recovery code.
    /// </summary>
    public class LoginWithRecoveryCodeViewModel
    {
            /// <summary>
            /// Gets or sets the recovery code.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
    }
}