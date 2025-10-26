
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Routing;
using ADSBackend.Models;
using Serilog;

namespace ADSBackend.Middlewares
{
    /// <summary>
    /// Middleware for tracking user activity.
    /// </summary>
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ActivityTracker
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityTracker"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public ActivityTracker(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware to track user activity.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="context">The application database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            try
            {

                if (httpContext.User is not null)
                {
                    if (httpContext.User.Identity is not null)
                    {
                        if (httpContext.User.Identity.IsAuthenticated)
                        {
                            var user = await userManager.GetUserAsync(httpContext.User);

                            if (user != null)
                            {
                                user.LastOnline = DateTime.Now;
                                await userManager.UpdateAsync(user);
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error tracking user activity");
            }

            await _next(httpContext);
        }
    }

    /// <summary>
    /// Extension methods for adding the <see cref="ActivityTracker"/> middleware.
    /// </summary>
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ActivityTrackerExtensions
    {
        /// <summary>
        /// Adds the <see cref="ActivityTracker"/> middleware to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseActivityTracker(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActivityTracker>();
        }
    }
}