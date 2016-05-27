using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.Models
{
    public class Appointment
    {
        public string CalendarId { get; set; }
        public string EmployeeName { get; set; }
        public string ClientName { get; set; }
        public int GsmNumber { get; set; }
        public DateTime StartTimeOfAppointment { get; set; }
    }
}
