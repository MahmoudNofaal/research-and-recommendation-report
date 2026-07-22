using System.Net;
using System.Net.Mail;
using Application.Abstractions.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication
{
    /// <summary>
    /// SMTP transport for account-confirmation and password-reset messages. It
    /// never logs message bodies or one-time tokens.
    /// </summary>
    internal sealed class EmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            ValidateConfiguration();

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.FromAddress, _options.FromName),
                Subject = message.Subject,
                Body = message.HtmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(message.To));
            mailMessage.AlternateViews.Add
            (
                AlternateView.CreateAlternateViewFromString
                (
                    message.PlainTextBody,
                    null,
                    "text/plain"
                )
            );

            using var smtpClient = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                UseDefaultCredentials = string.IsNullOrWhiteSpace(_options.Username)
            };

            if (!smtpClient.UseDefaultCredentials)
            {
                smtpClient.Credentials = new NetworkCredential(_options.Username, _options.Password);
            }

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.Host)
                || string.IsNullOrWhiteSpace(_options.FromAddress)
                || _options.Port is <= 0 or > 65535)
            {
                throw new InvalidOperationException
                (
                    "Email is not configured. Set Authentication:Email:Host, Port, and FromAddress through user secrets or environment variables."
                );
            }
        }
    }
}
