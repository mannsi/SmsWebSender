using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class CalendarColorMap
    {
        [Key]
        public string Id { get; set; }
        public string CalendarId { get; set; }
        public string Color { get; set; }
    }
}
