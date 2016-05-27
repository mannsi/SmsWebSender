using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SmsWebSender.Models;
using SmsWebSender.ViewModels.Account;

namespace SmsWebSender.ViewModels.Account
{
    public enum AutomaticSendOptionEnum
    {
        SameDay,
        DayBefore
    }

    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Verður að fylla")]
        [DataType(DataType.Text)]
        [Display(Name="Sms berist frá")]
        public string SendSmsName { get; set; }

        [Required(ErrorMessage = "Verður að fylla")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Snið fyrir sms")]
        public string SmsTemplate { get; set; }

        public string DefaultSmsTemplate { get; set; }

        public bool SendAutomatically { get; set; }
        public AutomaticSendOptionEnum AutomaticSendOption { get; set; }
        public int AutomaticSendHour { get; set; }
    }
}
