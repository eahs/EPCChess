
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Interface for a service that sends emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Asynchronously sends an email.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        Task SendEmailAsync(string email, string subject, string message);
    }
}