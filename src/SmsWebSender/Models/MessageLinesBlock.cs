using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmsWebSender.ViewModels.Sms;

namespace SmsWebSender.Models
{
    /// <summary>
    /// A collection of message lines
    /// </summary>
    public class MessageLinesBlock
    {
        public MessageLinesBlock()
        {
            MessageLines = new List<MessageLine>();
        }

        public string EmployeeName { get; set; }
        public List<MessageLine> MessageLines { get; set; }
    }
}
