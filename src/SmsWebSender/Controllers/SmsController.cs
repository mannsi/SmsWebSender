using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using SmsWebSender.ViewModels.Sms;
using Twilio;
using Message = Twilio.Message;

namespace SmsWebSender.Controllers
{
    [Route("sms")]
    public class SmsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private const int IcelandicAreaCode = 354;

        public SmsController(IAppointmentService appointmentService,
            ISmsService smsService, 
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _appointmentService = appointmentService;
            _smsService = smsService;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }

        [Authorize]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var userSending = await _userManager.FindByIdAsync(User.GetUserId());

            var vm = new SmsViewModel
            {
                SenderName = userSending.SendSmsName,
                CompanyName = userSending.CompanyName,
                SmsTemplate = userSending.SmsTemplate
            };
            return View(vm);
        }

        [Authorize]
        [Route("List")]
        public IActionResult List()
        {
            var messages = _smsService.GetMessages();
            return View(messages);
        }

        [Authorize]
        [Route("MessageLinesBlocks")]
        public async Task<JsonResult> MessageLinesBlocks(DateTime date)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            List<Appointment> appointmentsForDay;
            try
            {
                 appointmentsForDay = _appointmentService.AppointmentsForDay(date);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return new JsonResult(ex.Message);
            }
            var messageLinesBlocks = new List<MessageLinesBlock>();

            foreach (var appointment in appointmentsForDay)
            {
                var block = messageLinesBlocks.FirstOrDefault(mlb => mlb.EmployeeName == appointment.EmployeeName);
                if (block == null)
                {
                    block = new MessageLinesBlock {EmployeeName = appointment.EmployeeName};
                    messageLinesBlocks.Add(block);
                }

                block.MessageLines.Add(new MessageLine(user.SmsTemplate, appointment.StartTimeOfAppointment, appointment.GsmNumber.ToString())
                {
                    Name = appointment.ClientName,
                    CalendarId = appointment.CalendarId,
                });

            }

            return new JsonResult(messageLinesBlocks);
        }

        [HttpPost]
        [Authorize]
        [Route("Send")]
        public async Task<bool> Send([FromBody]List<MessageLinesBlock> messageLinesBlocks)
        {

            var messageLinesToSend = new List<MessageLine>();
            foreach (var block in messageLinesBlocks)
            {
                messageLinesToSend.AddRange(block.MessageLines.Where(line => line.ShouldBeSentTo));
            }

            var messages = new List<SmsMessage>();
            var sendingUser = await _userManager.FindByIdAsync(User.GetUserId());
            foreach (var messageLine in messageLinesToSend)
            {
                string to = $"+{IcelandicAreaCode}{messageLine.Number}";
                messages.Add(new SmsMessage {To = to, From = sendingUser.SendSmsName , Body = messageLine.Body });
            }

            if (messages.Any() && sendingUser.SendSmsConfirmationToUser)
            {
                //SendConfirmationSms(sendingUser, messages);
                _smsService.SendBatch(messages, _configuration["smsWebSenderCallbackUrl"]
            );
            }

            return true;
        }

        private void SendConfirmationSms(ApplicationUser user, List<SmsMessage> messagesToSend)
        {
            var message = new SmsMessage
            {
                To = $"+{IcelandicAreaCode}{user.UsersGsmNumber}",
                From = "Hyldypi",
                Body = $"Vorum ad senda {messagesToSend.Count} sms fyrir thig. Vid latum thig vita ef einhver theirra komast ekki til skila." 
            };

            _smsService.SendMessage(message, _configuration["smsWebSenderCallbackUrl"]);
        }

        [Route("Test")]
        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }

        [Authorize]
        [Route("Test")]
        [HttpPost]
        public void Test(SmsWebSender.Models.SmsMessage vm)
        {
            _smsService.SendMessage(vm, _configuration["smsWebSenderCallbackUrl"]);
        }

        [Route("SmsCallback")]
        [HttpPost]
        public async Task SmsCallback(string SmsStatus, string From, string To)
        {
            // Only wish to alert the user of an unsuccessful status 
            if (SmsStatus != "undelivered" && SmsStatus != "failed")
            {
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.SendSmsName == From);
            if (user == null)
            {
                return;
            }

            string errorMessage = $"Ekki tokst a senda sms i numerid {To}";
            _smsService.SendMessage(new SmsMessage {From = "Hyldypi", To=$"+{IcelandicAreaCode}{user.UsersGsmNumber}", Body= errorMessage }, "" );

            // Also send to me so I can keep track of things
            await
                _emailService.SendEmailAsync("gudbjorn.einarsson@gmail.com", "Sms sending mistókst", errorMessage, "hyldypi@hyldypi.is",
                        "Hyldýpi");
        }

        [Route("TestEmailSending")]
        public async Task TestEmailSending(string SmsStatus, string From, string To)
        {
            await
                _emailService.SendEmailAsync("gudbjorn.einarsson@gmail.com", "SMS test mail",
                    $"SmsStatus: {SmsStatus}, From: {From}, To: {To}", "hyldypi@hyldypi.is", "Hyldýpi");
        }

    }
}
