﻿
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SmsWebSender.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string CompanyName { get; set; }
        public string SendSmsName { get; set; }
        public string SmsTemplate { get; set; }
        public string UsersGsmNumber { get; set; }
        public bool SendSmsConfirmationToUser { get; set; }
        public bool SenEmailConfirmationToUser { get; set; }

        public bool ShouldAutoSendSms { get; set; }
        public int AutoSendHour { get; set; }

        public bool SendSameDay { get; set; }
        public bool SendDayBefore { get; set; }
        public bool SendTwoDaysBefore { get; set; }
        public bool SendThreeDaysBefore { get; set; }
    }
}
