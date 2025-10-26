
using ADSBackend.Models.AccountViewModels;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ADSBackend.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ADSBackend.Models;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for handling user account actions like login, logout, and registration.
    /// </summary>
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for user-related operations.</param>
        /// <param name="signInManager">The sign-in manager for handling user authentication.</param>
        /// <param name="emailSender">The service for sending emails.</param>
        /// <param name="logger">The logger for logging messages.</param>
        /// <param name="context">The database context.</param>
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets or sets the error message to be displayed.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>The login view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Handles the login form submission.
        /// </summary>
        /// <param name="model">The login view model containing user credentials.</param>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>A redirect to the return URL on success, or the login view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    var user = await _userManager.FindByNameAsync(model.Username);
                    user.ExpiresAt = DateTime.Now.AddDays(30);
                    await _userManager.UpdateAsync(user);

                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Displays the two-factor authentication login page.
        /// </summary>
        /// <param name="rememberMe">Whether to remember the user.</param>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>The two-factor authentication login view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        /// <summary>
        /// Handles the two-factor authentication form submission.
        /// </summary>
        /// <param name="model">The two-factor login view model.</param>
        /// <param name="rememberMe">Whether to remember the user.</param>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>A redirect to the return URL on success, or the view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        /// <summary>
        /// Displays the login with recovery code page.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>The login with recovery code view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        /// <summary>
        /// Handles the login with recovery code form submission.
        /// </summary>
        /// <param name="model">The login with recovery code view model.</param>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>A redirect to the return URL on success, or the view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        /// <summary>
        /// Displays the account lockout page.
        /// </summary>
        /// <returns>The lockout view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after a successful registration.</param>
        /// <returns>The registration view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Handles the registration form submission.
        /// </summary>
        /// <param name="model">The registration view model.</param>
        /// <param name="returnUrl">The URL to return to after a successful registration.</param>
        /// <returns>A redirect to the return URL on success, or the registration view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Handles user logout.
        /// </summary>
        /// <returns>A redirect to the home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(AdminController.Index), "Home");
        }

        /// <summary>
        /// Initiates an external login process.
        /// </summary>
        /// <param name="provider">The external login provider.</param>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <returns>A challenge result that redirects to the external provider.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Handles the callback from an external login provider.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after a successful login.</param>
        /// <param name="remoteError">Any error message from the external provider.</param>
        /// <returns>A redirect to the appropriate page based on the login result.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var accessToken = info.AuthenticationTokens.FirstOrDefault(inf => inf.Name.Equals("access_token")).Value;
                var expiresAtRaw = info.AuthenticationTokens.FirstOrDefault(inf => inf.Name.Equals("expires_at")).Value;

                var expiresAt = DateTime.Parse(expiresAtRaw);

                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                user.AccessToken = accessToken;
                user.ExpiresAt = expiresAt;

                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                var season =
                    await _context.Season.FirstOrDefaultAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                var seasonId = season is null ? 1 : season.SeasonId;
                
                var schools = await _context.School.Select(x => x)
                    .Where(s => s.SeasonId == seasonId)
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                ViewBag.Schools = new SelectList(schools, "SchoolId", "Name");

                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

                return View("ExternalLogin", new ExternalLoginViewModel { Email = email, FirstName = firstName, LastName = lastName});
            }
        }

        /// <summary>
        /// Handles the confirmation of an external login, creating a new user if necessary.
        /// </summary>
        /// <param name="model">The external login view model.</param>
        /// <param name="returnUrl">The URL to return to after a successful confirmation.</param>
        /// <returns>A redirect to the return URL on success, or the view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                model.SchoolId = 1;  // Default to unassigned school (schoolId of 1)

                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var accessToken = info.AuthenticationTokens.FirstOrDefault(inf => inf.Name.Equals("access_token")).Value;
                var expiresAtRaw = info.AuthenticationTokens.FirstOrDefault(inf => inf.Name.Equals("expires_at")).Value;

                var expiresAt = DateTime.Parse(expiresAtRaw);

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, AccessToken = accessToken, ExpiresAt = expiresAt, SchoolId = 1};
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    user.SchoolId = 1; // Necessary until this is removed to maintain key constraint
                    user.Schools = new List<UserSchool>();
                    user.Schools.Add(new UserSchool
                    {
                        UserId = user.Id,
                        SchoolId = model.SchoolId
                    });
                    await _userManager.UpdateAsync(user);

                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                        {
                            await _userManager.AddClaimAsync(user,
                                info.Principal.FindFirst(ClaimTypes.GivenName));
                        }

                        // assign new role
                        await _userManager.AddToRoleAsync(user, "Guest");

                        // Include the access token in the properties
                        var props = new AuthenticationProperties();
                        props.StoreTokens(info.AuthenticationTokens);
                        props.IsPersistent = true;

                        await _signInManager.SignInAsync(user, props);

                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            var season =
                await _context.Season.FirstOrDefaultAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

            var seasonId = season is null ? 1 : season.SeasonId;

            var schools = await _context.School.Select(x => x)
                .Where(s => s.SeasonId == seasonId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.Schools = new SelectList(schools, "SchoolId", "Name");


            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        /// <summary>
        /// Confirms a user's email address.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="code">The confirmation code.</param>
        /// <returns>The confirmation view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(AdminController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Displays the forgot password page.
        /// </summary>
        /// <returns>The forgot password view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles the forgot password form submission.
        /// </summary>
        /// <param name="model">The forgot password view model.</param>
        /// <returns>A redirect to the forgot password confirmation page.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Displays the forgot password confirmation page.
        /// </summary>
        /// <returns>The forgot password confirmation view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Displays the reset password page.
        /// </summary>
        /// <param name="code">The password reset code.</param>
        /// <returns>The reset password view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        /// <summary>
        /// Handles the reset password form submission.
        /// </summary>
        /// <param name="model">The reset password view model.</param>
        /// <returns>A redirect to the reset password confirmation page on success, or the view with errors on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Displays the reset password confirmation page.
        /// </summary>
        /// <returns>The reset password confirmation view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        /// <summary>
        /// Displays the access denied page.
        /// </summary>
        /// <returns>The access denied view.</returns>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(AdminController.Index), "Home");
            }
        }

        #endregion
    }
}