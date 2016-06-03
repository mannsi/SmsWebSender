using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class MessageStatus
    {
        public enum Status
        {
            Delivered,
            Sent,
            Queued,
            Undelivered,
            Accepted,
            Failed
        }

        public string Id { get; set; }
        public Status LatestStatus { get; set; }
        public string MessageId { get; set; } 
    }
}
