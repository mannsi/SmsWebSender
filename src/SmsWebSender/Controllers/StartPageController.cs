using System;
using System.Net.Mail;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmsWebSender.ServiceInterfaces;

namespace SmsWebSender.Controllers
{
    [Authorize]
    public class StartPageController : Controller
    {
        private readonly IEmailService _emailService;

        public StartPageController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("LandingPage");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/sms")]
        public IActionResult LandingPage()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("contact")]
        public void Contact(string name, string email, string message)
        {
            try
            {
                MailAddress testEmailAddressObject = new MailAddress(email);
            }
            catch (Exception)
            {
                // Illegal email address
                return;
            }

            // Send email to the given email address telling them we received their request
            _emailService.SendEmailAsync(email,
                "Staðfesting á fyrirspurn",
                "Við höfum fengið fyrirspurn frá þér varðandi Sms áminningar. Við munum hafa samband við þig eins fljótt og við getum.",
                "hyldypi@hyldypi.is",
                "Hyldýpi");

            // Send email to me with the request, email and name
            // Send email to the given email address telling them we received their request
            _emailService.SendEmailAsync("gudbjorn.einarsson@gmail.com",
                "Fyrirspurn varðandi Sms áminningar",
                $"Innihald: <br/> Nafn: {name}, Email: {email} <br/> Skilaboð: {message}",
                "hyldypi@hyldypi.is",
                "Hyldýpi");
        }
        
    }
}
