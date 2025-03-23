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

        public async Task<bool> TrySendVerificationEmailAsync(string email)
        {
            try
            {
                var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUsername),
                    Subject = "✅ Your email has been successfully changed!",
                    Body = @"
                            <html>
                                <head>
                                    <style>
                                        body { font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }
                                        .container { max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }
                                        h2 { color: #4CAF50; }
                                        p { font-size: 16px; color: #333; }
                                        .footer { margin-top: 20px; font-size: 14px; color: #777; text-align: center; }
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <h2>Your email has been changed! 🎉</h2>
                                            <p>Congratulations! You have successfully updated your email address in our system.</p>
                                            <p>If this wasn't you, please contact our support team immediately.</p>
                                            <div class='footer'>
                                                <p>Best regards,<br><strong>Support Team</strong></p>
                                            </div>
                                    </div>
                                </body>
                            </html>
                            ",

                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (SmtpFailedRecipientException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
