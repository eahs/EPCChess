using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713

    public class EmailSender : IEmailSender
    {
        private Services.Configuration Configuration { get; set; }

        public EmailSender(Services.Configuration configuration)
        {
            Configuration = configuration;
        }

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
