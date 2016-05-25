using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmsWebSender.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Verður að fylla")]
        [EmailAddress(ErrorMessage = "Ólöglegt netfang")]
        [Display(Name="Netfang")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Verður að fylla")]
        [DataType(DataType.Password)]
        [Display(Name="Lykilorð")]
        public string Password { get; set; }

    }
}
