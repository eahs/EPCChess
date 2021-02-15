using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ADSBackend.Services
{
    public class TokenRefresher : ITokenRefresher
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TokenRefresher(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Refreshes the Api tokens for the given ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="signInUser"></param>
        /// <returns></returns>
        public async Task<bool> RefreshTokens(ApplicationUser user, bool signInUser = false)
        {
            var lichessAuthNSection = _configuration.GetSection("Authentication:Lichess");

            HttpMessageHandler _handler = new HttpClientHandler();
            HttpClient _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri(AspNet.Security.OAuth.Lichess.LichessAuthenticationDefaults.TokenEndpoint)
            };

            TokenClientOptions options = new TokenClientOptions
            {
                ClientId = lichessAuthNSection["ClientID"],
                ClientSecret = lichessAuthNSection["ClientSecret"]
            };

            var tokenClient = new TokenClient(_client, options);
            var rt = user.RefreshToken;
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);

            if (!tokenResult.IsError)
            {
                var newAccessToken = tokenResult.AccessToken;
                var newRefreshToken = tokenResult.RefreshToken;
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);

                if (signInUser)
                {
                    var tokens = new List<AuthenticationToken>
                    {
                        new AuthenticationToken
                            {Name = OpenIdConnectParameterNames.AccessToken, Value = newAccessToken},
                        new AuthenticationToken
                            {Name = OpenIdConnectParameterNames.RefreshToken, Value = newRefreshToken},
                        new AuthenticationToken
                            { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) }
                    };

                    var props = new AuthenticationProperties();
                    props.StoreTokens(tokens);
                    props.IsPersistent = true;

                    await _signInManager.SignInAsync(user, props);
                }

                user.AccessToken = newAccessToken;
                user.RefreshToken = newRefreshToken;
                user.ExpiresAt = expiresAt;

                await _userManager.UpdateAsync(user);

                return true;
            }

            return false;
        }
    }
}
