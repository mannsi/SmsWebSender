using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;

namespace SmsWebSender.ViewModels.Sms
{
    public class SmsViewModel
    {
        public string UserId { get; set; }
        public string SenderName { get; set; }
        public string CompanyName { get; set; }
        public string SmsTemplate { get; set; }
    }
}
