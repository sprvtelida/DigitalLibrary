using System;
using System.Threading.Tasks;
using DigitalLibrary.API.Settings;

using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DigitalLibrary.API.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            if (mailRequest.Subject != null)
            {
                email.Subject = mailRequest.Subject;
            }

            var builder = new BodyBuilder();

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

                await client.SendAsync(email);
                client.Disconnect(true);
            }
        }
    }
}
