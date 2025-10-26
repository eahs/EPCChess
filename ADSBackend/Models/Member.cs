
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a member of the application.
    /// </summary>
    public class Member
    {
        /// <summary>
        /// Gets or sets the ID of the member.
        /// </summary>
        [Key]
        public int MemberId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the member.
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "First name is required")]  // Max 32 characters, min 1 character
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the member.
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "Last name is required")]  // Max 32 characters, min 1 character
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the member.
        /// </summary>
        [Required, DataType(DataType.EmailAddress, ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the member.
        /// </summary>
        [Required]
        [JsonIgnore]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt for the member.
        /// </summary>
        [JsonIgnore]
        public string PasswordSalt { get; set; }
    }
}