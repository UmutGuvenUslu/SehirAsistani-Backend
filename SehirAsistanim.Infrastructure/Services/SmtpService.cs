using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly SmtpSettings _settings;

        #region Constructor
        public SmtpService(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }
        #endregion

        #region Mail Gönder
        public async Task Gonder(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            var message = new MailMessage(_settings.UserName, toEmail, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
        #endregion
    }
}
