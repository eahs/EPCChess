
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="IUrlHelper"/> interface.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a link for email confirmation.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="code">The confirmation code.</param>
        /// <param name="scheme">The request scheme (e.g., http, https).</param>
        /// <returns>The email confirmation link.</returns>
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        /// <summary>
        /// Generates a callback link for password reset.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="code">The password reset code.</param>
        /// <param name="scheme">The request scheme (e.g., http, https).</param>
        /// <returns>The password reset callback link.</returns>
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}