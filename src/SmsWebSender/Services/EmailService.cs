using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
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

        public Task SendEmailAsync(string emailAddress, string subject, string message, string from, string fromDisplayName)
        {
            // Plug in your email service here to send an email.
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(emailAddress);
            myMessage.From = new System.Net.Mail.MailAddress(from, fromDisplayName);
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            // Create a Web transport for sending email.
            var transportWeb = new SendGrid.Web(_configuration["SendGridApiKey"]);
            // Send the email.
            return transportWeb.DeliverAsync(myMessage);
        }
    }
}
