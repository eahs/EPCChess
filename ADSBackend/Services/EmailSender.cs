
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Service for sending emails.
    /// </summary>
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713

    public class EmailSender : IEmailSender
    {
        private Services.Configuration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration service.</param>
        public EmailSender(Services.Configuration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Asynchronously sends an email.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient(Configuration.Get("SMTP_HOST"))
            {
                UseDefaultCredentials = false,
                Port = int.Parse(Configuration.Get("SMTP_PORT")),
                Credentials = new NetworkCredential(Configuration.Get("SMTP_USER"), Configuration.Get("SMTP_PASSWORD"))
            };

            MailMessage mailMessage = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(Configuration.Get("SMTP_USER"), "EPC Chess Admin"),
                Body = message,
                Subject = subject,
            };
            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}