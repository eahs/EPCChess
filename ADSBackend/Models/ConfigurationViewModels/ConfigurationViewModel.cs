
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models.ConfigurationViewModels
{
    /// <summary>
    /// View model for application configuration settings.
    /// </summary>
    public class ConfigurationViewModel
    {
        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        [Required]
        [Display(Name = "SMTP Host")]
        public string SMTP_HOST { get; set; }

        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        [Required]
        [Display(Name = "SMTP Port")]
        public string SMTP_PORT { get; set; }

        /// <summary>
        /// Gets or sets the SMTP username.
        /// </summary>
        [Display(Name = "SMTP Username")]
        public string SMTP_USER { get; set; }

        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        [Display(Name = "SMTP Password")]
        public string SMTP_PASSWORD { get; set; }
    }
}