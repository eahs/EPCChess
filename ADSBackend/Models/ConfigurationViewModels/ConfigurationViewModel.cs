using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        [Display(Name = "SMTP Host")]
        public string SMTP_HOST { get; set; }

        [Required]
        [Display(Name = "SMTP Port")]
        public string SMTP_PORT { get; set; }

        [Display(Name = "SMTP Username")]
        public string SMTP_USER { get; set; }

        [Display(Name = "SMTP Password")]
        public string SMTP_PASSWORD { get; set; }
    }
}
