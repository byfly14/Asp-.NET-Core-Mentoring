using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("My Site Administration", _config["Email"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart
            {
                Text = message
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync(_config["Email"], _config["Password"]);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
