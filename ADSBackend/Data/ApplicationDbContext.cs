using ADSBackend.Models;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Models.MessagesModels;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ADSBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationItem> ConfigurationItem { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<UserSchool>()
                .HasOne(us => us.User)
                .WithMany(t => t.Schools)
                .HasForeignKey(t => t.UserId);

        }

        public DbSet<UserSchool> UserSchool { get; set; }

        public DbSet<ADSBackend.Models.MessagesModels.Message> Message { get; set; }

        public DbSet<ADSBackend.Models.Season> Season { get; set; }

        public DbSet<ADSBackend.Models.School> School { get; set; }

        public DbSet<ADSBackend.Models.Match> Match { get; set; }

        public DbSet<ADSBackend.Models.Player> Player { get; set; }

        public DbSet<ADSBackend.Models.Game> Game { get; set; }

        public DbSet<ADSBackend.Models.Division> Division { get; set; }

        public DbSet<ADSBackend.Models.RatingEvent> RatingEvent { get; set; }
        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<MatchChat> MatchChat { get; set; }

    }
}
