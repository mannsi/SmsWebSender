using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.ServiceInterfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailAddress, string subject, string message, string from, string fromDisplayName);
    }
}
