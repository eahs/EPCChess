
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Models.MessagesModels;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ADSBackend.Data
{
    /// <summary>
    /// The main database context for the application.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IDataProtectionKeyContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for configuration items.
        /// </summary>
        public DbSet<ConfigurationItem> ConfigurationItem { get; set; }

        /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
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

        /// <summary>
        /// Gets or sets the DbSet for the join table between users and schools.
        /// </summary>
        public DbSet<UserSchool> UserSchool { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for messages.
        /// </summary>
        public DbSet<ADSBackend.Models.MessagesModels.Message> Message { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for seasons.
        /// </summary>
        public DbSet<ADSBackend.Models.Season> Season { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for schools.
        /// </summary>
        public DbSet<ADSBackend.Models.School> School { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for matches.
        /// </summary>
        public DbSet<ADSBackend.Models.Match> Match { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for players.
        /// </summary>
        public DbSet<ADSBackend.Models.Player> Player { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for games.
        /// </summary>
        public DbSet<ADSBackend.Models.Game> Game { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for divisions.
        /// </summary>
        public DbSet<ADSBackend.Models.Division> Division { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for rating events.
        /// </summary>
        public DbSet<ADSBackend.Models.RatingEvent> RatingEvent { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for data protection keys. This maps to the table that stores keys.
        /// </summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for match chat messages.
        /// </summary>
        public DbSet<MatchChat> MatchChat { get; set; }

    }
}