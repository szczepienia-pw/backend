using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace backend.Helpers
{
    public class Mailer
    {
        private readonly MailSettings mailSettings;

        public Mailer(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }

        protected Mailer() { } // Only for tests

        [ExcludeFromCodeCoverage]
        public virtual async Task<Task> SendEmailAsync(string to, string subject, string body, List<IFormFile>? attachments = null)
            => Task.Factory.StartNew(async () => this.sendEmailAsync(to, subject, body, attachments));

        [ExcludeFromCodeCoverage]
        protected virtual async Task sendEmailAsync(string to, string subject, string body, List<IFormFile>? attachments = null)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(this.mailSettings.DisplayName, this.mailSettings.DisplayEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            var builder = new BodyBuilder();

            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(this.mailSettings.Host, this.mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(this.mailSettings.Mail, this.mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
