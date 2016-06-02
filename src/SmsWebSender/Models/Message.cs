using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class Message
    {
        public Message()
        {

        }

        public Message(Twilio.Message twilioMessage)
        {
            To = twilioMessage.To;
            From = twilioMessage.From;
            Body = twilioMessage.Body;
        }

        public string Id => $"{To}{Body}";
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}
