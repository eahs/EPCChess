
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ADSBackend.Views.Manage
{
    /// <summary>
    /// Provides navigation helpers for the manage account pages.
    /// </summary>
    public static class ManageNavPages
    {
        /// <summary>
        /// The key for the active page in ViewData.
        /// </summary>
        public static string ActivePageKey => "ActivePage";

        /// <summary>
        /// The name of the index page.
        /// </summary>
        public static string Index => "Index";

        /// <summary>
        /// The name of the change password page.
        /// </summary>
        public static string ChangePassword => "ChangePassword";

        /// <summary>
        /// The name of the external logins page.
        /// </summary>
        public static string ExternalLogins => "ExternalLogins";

        /// <summary>
        /// The name of the two-factor authentication page.
        /// </summary>
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        /// <summary>
        /// Gets the CSS class for the index navigation link.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The CSS class "active" if the current page is the index page; otherwise, null.</returns>
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        /// <summary>
        /// Gets the CSS class for the change password navigation link.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The CSS class "active" if the current page is the change password page; otherwise, null.</returns>
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        /// <summary>
        /// Gets the CSS class for the external logins navigation link.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The CSS class "active" if the current page is the external logins page; otherwise, null.</returns>
        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        /// <summary>
        /// Gets the CSS class for the two-factor authentication navigation link.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The CSS class "active" if the current page is the two-factor authentication page; otherwise, null.</returns>
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);
    
        /// <summary>
        /// Gets the CSS class for a navigation link.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="page">The name of the page.</param>
        /// <returns>The CSS class "active" if the current page matches the specified page; otherwise, null.</returns>
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        /// <summary>
        /// Adds the active page to the ViewData dictionary.
        /// </summary>
        /// <param name="viewData">The ViewData dictionary.</param>
        /// <param name="activePage">The name of the active page.</param>
        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}