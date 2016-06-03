using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class SmsMessage
    {
        public SmsMessage()
        {

        }

        public SmsMessage(Twilio.Message twilioMessage)
        {
            To = twilioMessage.To;
            From = twilioMessage.From;
            Body = twilioMessage.Body;
        }

        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}
