
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.ManageViewModels
{
    /// <summary>
    /// View model for displaying two-factor authentication recovery codes.
    /// </summary>
    public class ShowRecoveryCodesViewModel
    {
        /// <summary>
        /// Gets or sets the array of recovery codes.
        /// </summary>
        public string[] RecoveryCodes { get; set; }
    }
}