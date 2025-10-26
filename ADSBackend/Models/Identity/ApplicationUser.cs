
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADSBackend.Models.Identity
{
    /// <summary>
    /// Represents a user in the application, extending the base IdentityUser with custom properties.
    /// </summary>
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// DEPRECATED - Remove after running at least once in production
        /// </summary>
        public int SchoolId { get; set; }
        /// <summary>
        /// Gets or sets the user's school.
        /// </summary>
        public School School { get; set; }

        /// <summary>
        /// Gets or sets the list of schools associated with the user.
        /// </summary>
        public List<UserSchool> Schools { get; set; }
        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        public virtual string FullName => FirstName.Trim() + " " + LastName?.Trim();

        /// <summary>
        /// Gets or sets the OAuth access token.
        /// </summary>
        public string AccessToken { get; set; } = "";
        /// <summary>
        /// Gets or sets the OAuth refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = "";
        /// <summary>
        /// Gets or sets the expiration date and time of the access token.
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Gets or sets the user's Lichess ID.
        /// </summary>
        public String LichessId { get; set; } = "";
        /// <summary>
        /// Gets or sets the last time the user was seen online.
        /// </summary>
        public DateTime LastOnline { get; set; }

        /// <summary>
        /// Gets the CSS class for the user's online status.
        /// </summary>
        [NotMapped]
        public string OnlineStatusCss
        {
            get
            {
                bool isOnline = LastOnline >= DateTime.Now.AddMinutes(-15);

                return isOnline ? "status online" : "status offline";
            }
        }

        /// <summary>
        /// Generates the Gravatar hash for the user's email.
        /// </summary>
        /// <returns>The MD5 hash of the email address for Gravatar.</returns>
        public virtual string GravitarHash()
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Email?.ToLower() ?? ""));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}