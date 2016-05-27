using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
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
