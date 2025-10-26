
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ADSBackend.Util
{
    /// <summary>
    /// An action filter that checks for and handles expired access tokens.
    /// </summary>
    public class RefreshTokenFilter : ActionFilterAttribute
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenFilter"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign-in manager.</param>
        public RefreshTokenFilter(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Called before the action executes, to check for an expiring access token.
        /// </summary>
        /// <param name="context">The context for the action execution.</param>
        /// <param name="next">The delegate to execute the next action filter or the action itself.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user is not null)
            {
                if (user.ExpiresAt < DateTime.Now.AddMinutes(120)) //Check if the access token will expire in 120 minutes
                {
                    // Sign the user out
                    await _signInManager.SignOutAsync();
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
        
    }
}