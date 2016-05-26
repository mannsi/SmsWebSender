using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using SmsWebSender.ViewModels.Sms;

namespace SmsWebSender.Controllers
{
    public class SmsController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public JsonResult Recipients(DateTime date)
        {
            var now = date;
            var recipients = new List<SmsRecipient>();
            recipients.Add(new SmsRecipient
            {
                Name = "Jón Jónsson", Number = 8123456, AppointmentStartTime = new DateTime(now.Year, now.Month, now.Day).AddHours(9),
                ShouldBeSentTo = true, SmsText = "Einhver texti"
            });
            recipients.Add(new SmsRecipient
            {
                Name = "Finnur Jónsson", Number = 5649876, AppointmentStartTime = new DateTime(now.Year, now.Month, now.Day).AddHours(10),
                ShouldBeSentTo = true
            });
            recipients.Add(new SmsRecipient
            {
                Name = "Magni Jónsson", Number = 2356897, AppointmentStartTime = new DateTime(now.Year, now.Month, now.Day).AddHours(11),
                ShouldBeSentTo = true
            });

            return new JsonResult(recipients);
        }

        [HttpPost]
        [Authorize]
        public bool Send([FromBody]List<SmsRecipient> smsRecipients)
        {
            // TODO
            // Add the sms sending service here. This function should be called from a JS function
            return true;
        }
    }
}
