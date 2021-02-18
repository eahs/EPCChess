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
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ActivityTracker
    {
        private readonly RequestDelegate _next;

        public ActivityTracker(RequestDelegate next)
        {
            _next = next;
        }

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

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ActivityTrackerExtensions
    {
        public static IApplicationBuilder UseActivityTracker(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActivityTracker>();
        }
    }
}