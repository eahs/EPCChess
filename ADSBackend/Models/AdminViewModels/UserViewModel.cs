
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models.AdminViewModels
{
    /// <summary>
    /// View model for user management in the admin area.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the user's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        [Required]
        public string Role { get; set; }
        
        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the list of school IDs associated with the user.
        /// </summary>
        public List<int> SchoolIds { get; set; }
        /// <summary>
        /// Gets or sets the list of schools associated with the user.
        /// </summary>
        public List<School> Schools { get; set; }

        /// <summary>
        /// Gets or sets the password confirmation.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}