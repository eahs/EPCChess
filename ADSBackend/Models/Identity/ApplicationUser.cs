using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADSBackend.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // DEPRECATED - Remove after running at least once in production
        public int SchoolId { get; set; }
        public School School { get; set; }

        public List<UserSchool> Schools { get; set; }
        public virtual string FullName => FirstName.Trim() + " " + LastName?.Trim();

        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime ExpiresAt { get; set; } = DateTime.Now;
        public String LichessId { get; set; } = "";
        public DateTime LastOnline { get; set; }

        [NotMapped]
        public string OnlineStatusCss
        {
            get
            {
                bool isOnline = LastOnline >= DateTime.Now.AddMinutes(-15);

                return isOnline ? "status online" : "status offline";
            }
        }

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
