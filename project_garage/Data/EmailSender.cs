using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace project_garage.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer = Env.GetString("SERVER");
        private readonly int _smtpPort = Env.GetInt("PORT");
        private readonly string _smtpUsername = Env.GetString("EMAIL");
        private readonly string _smtpPassword = Env.GetString("PASSWORD");

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUsername),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
