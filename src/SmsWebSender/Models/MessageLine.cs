﻿using System;

namespace SmsWebSender.Models
{
    public class MessageLine
    {

        public MessageLine()
        {
            
        }

        public MessageLine(string smsTemplate, DateTime appointmentStartTime, string number)
        {
            AppointmentStartTime = appointmentStartTime;
            Number = number;
            Body = smsTemplate
                .Replace("{dagsetning}", AppointmentStartTime.ToString("dd/MM/yyyy"))
                .Replace("{klukkan}", AppointmentStartTime.ToString("HH:mm"));
            int dummy;
            ShouldBeSentTo = !Number.StartsWith("5") && Number.Length == 7 && int.TryParse(Number, out dummy);
        }

        public string Name { get; set; }
        public string Number { get; set; }
        public DateTime AppointmentStartTime { get; set; }

        public bool ShouldBeSentTo { get; set; }

        public string Body { get; set; }

        public string CalendarId { get; set; }
    }
}
