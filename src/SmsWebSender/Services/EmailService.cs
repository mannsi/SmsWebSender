using System;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmsWebSender.ServiceInterfaces;

namespace SmsWebSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmailAsync(string emailAddress, string subject, string message, string from, string fromDisplayName)
        {
            String apiKey = _configuration["SendGridApiKey"];
            dynamic sg = new SendGridAPIClient(apiKey);

            Email fromEmail = new Email(from);
            Email toEmail = new Email(emailAddress);
            Content content = new Content("text/html", message);
            Mail mail = new Mail(fromEmail, subject, toEmail, content);

            dynamic response = sg.client.mail.send.post(requestBody: mail.Get());
        }
    }
}
