
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a single configuration item in the database.
    /// </summary>
    public class ConfigurationItem
    {
        /// <summary>
        /// Gets or sets the key of the configuration item.
        /// </summary>
        [Key]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the configuration item.
        /// </summary>
        public string Value { get; set; }
    }
}