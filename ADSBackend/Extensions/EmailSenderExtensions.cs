
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Provides extension methods for the <see cref="IEmailSender"/> interface.
    /// </summary>
    public static class EmailSenderExtensions
    {
        /// <summary>
        /// Sends an email confirmation link to the specified email address.
        /// </summary>
        /// <param name="emailSender">The email sender instance.</param>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="link">The email confirmation link.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}