using ADSBackend.Configuration;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using ADSBackend.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ADSBackend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            string conn = Configuration.GetConnectionString("ADSBackendLocalContext");
#else
                string conn = Configuration.GetConnectionString("ADSBackendProductionContext");
#endif

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(conn,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<Services.IEmailSender, Services.EmailSender>();
            services.AddTransient<Services.DataService>();

            // caching
            services.AddMemoryCache();
            services.AddTransient<Services.Cache>();

            services.AddTransient<Services.Configuration>();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var dbSeed = new ApplicationDbSeed(dbContext);
                dbSeed.CreateSeasons();
                dbSeed.CreateDivisions();
                dbSeed.CreateSchools();
                dbSeed.CreateMatches();

                // seed the AspNetRoles table
                var roleSeed = new ApplicationRoleSeed(roleManager);
                roleSeed.CreateRoles();

                // seed the AspNetUsers table
                var userSeed = new ApplicationUserSeed(userManager);
                userSeed.CreateAdminUser();

                
            }

        }
    }
}
