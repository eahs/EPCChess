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
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ADSBackend.Util
{
    public class RefreshTokenFilter : ActionFilterAttribute
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRefresher _tokenRefresher;

        public RefreshTokenFilter(UserManager<ApplicationUser> userManager, ITokenRefresher tokenRefresher)
        {
            _userManager = userManager;
            _tokenRefresher = tokenRefresher;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user is not null && !String.IsNullOrEmpty(user.RefreshToken))
            {
                if (user.ExpiresAt < DateTime.Now.AddMinutes(5)) //Check if the access token will expire in 5 minutes
                {
                    // Update the authorization tokens and sign the user in again
                    await _tokenRefresher.RefreshTokens(user, signInUser: true);
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
        
    }
}
