using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SmsWebSender.Models;
using SmsWebSender.ViewModels.Account;

namespace SmsWebSender.ViewModels.Account
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Verður að fylla")]
        [DataType(DataType.Text)]
        public string SendSmsName { get; set; }

        [Required(ErrorMessage = "Verður að fylla")]
        [DataType(DataType.MultilineText)]
        public string SmsTemplate { get; set; }

        [Display(Name = "Senda sjálfkrafa")]
        public bool SendAutomatically { get; set; }

        public bool SendSameDay { get; set; }
        public bool SendDayBefore { get; set; }
        public bool SendTwoDaysBefore { get; set; }
        public bool SendThreeDaysBefore { get; set; }

        public int AutomaticSendHour { get; set; }
    }
}
