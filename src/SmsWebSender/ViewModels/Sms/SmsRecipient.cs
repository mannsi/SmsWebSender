using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.ViewModels.Sms
{
    public class SmsRecipient
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public string SmsText { get; set; }
        public bool ShouldBeSentTo { get; set; } 
    }
}
