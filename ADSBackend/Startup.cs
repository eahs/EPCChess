extern alias signed;

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
using Microsoft.AspNetCore.DataProtection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using StackExchange.Redis;

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
            string conn = Configuration.GetConnectionString("ADSMysqlProductionContext");
#else
            string conn = Configuration.GetConnectionString("ADSMysqlProductionContext");           
#endif

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    conn, 
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 1, 41),
                            Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb); // replace with your Server Version and Type
                    });
                /*
                options.UseSqlServer(conn,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                */
            });

            // https://stackexchange.github.io/StackExchange.Redis/Configuration.html
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"));
            
            if (redis.IsConnected)
            {
                services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }

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
                options.IdleTimeout = TimeSpan.FromMinutes(60*12);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            if (redis.IsConnected)
            {
                services.AddDistributedRedisCache(o =>
                {
                    o.Configuration = Configuration.GetConnectionString("Redis");
                });
            }

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
