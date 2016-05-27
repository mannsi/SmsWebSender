using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class TwilioMessage
    {
        [Key]
        public string Id { get; set; }
        public DateTime UpdateTime { get; set; }
        public ApplicationUser SendingUser { get; set; }
        public string NumberTo { get; set; }
        public string SentWithName { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
    }
}
