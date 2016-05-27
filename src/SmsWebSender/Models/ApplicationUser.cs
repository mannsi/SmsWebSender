using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SmsWebSender.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string CompanyName { get; set; }
        public string SendSmsName { get; set; }
        public string SmsTemplate { get; set; }
    }
}
