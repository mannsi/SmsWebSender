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
    [Route("smsApp")]
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
            return RedirectToAction("List");

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
            var messageLinesBlocks = GetMessageLineBlocks(date, _appointmentService, user, false);

            if (messageLinesBlocks == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult("Error getting message data");
            }

            return new JsonResult(messageLinesBlocks);
        }

        internal static List<MessageLinesBlock> GetMessageLineBlocks(DateTime date, IAppointmentService appointmentService, ApplicationUser user, bool onlyIncludeShouldSendLines)
        {
            List<Appointment> appointmentsForDay;
            try
            {
                appointmentsForDay = appointmentService.AppointmentsForDay(date);
            }
            catch (Exception ex)
            {
                return null;
            }
            var messageLinesBlocks = new List<MessageLinesBlock>();

            foreach (var appointment in appointmentsForDay)
            {
                var messageLine = new MessageLine(user.SmsTemplate, appointment.StartTimeOfAppointment,
                    appointment.GsmNumber.ToString())
                {
                    Name = appointment.ClientName,
                    CalendarId = appointment.CalendarId,
                };

                if (onlyIncludeShouldSendLines && !messageLine.ShouldBeSentTo)
                {
                    continue;
                }

                var block = messageLinesBlocks.FirstOrDefault(mlb => mlb.EmployeeName == appointment.EmployeeName);
                if (block == null)
                {
                    block = new MessageLinesBlock { EmployeeName = appointment.EmployeeName };
                    messageLinesBlocks.Add(block);
                }

                block.MessageLines.Add(messageLine);
            }

            return messageLinesBlocks;
        }

        [HttpPost]
        [Authorize]
        [Route("Send")]
        public async Task Send([FromBody]List<MessageLinesBlock> messageLinesBlocks)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            await SendBatch(messageLinesBlocks, _smsService, _emailService, user, _configuration, DateTime.MinValue);
        }

        /// <summary>
        /// Sends a batch of sms messages
        /// </summary>
        /// <param name="messageLinesBlocks">Message to send</param>
        /// <param name="smsService">Sms service</param>
        /// <param name="emailService">Email service</param>
        /// <param name="sendingUser">User we are sending for</param>
        /// <param name="configuration">System configurations</param>
        /// <param name="reminderDay">The day we are reminding clients about</param>
        /// <returns></returns>
        internal static async Task SendBatch(List<MessageLinesBlock> messageLinesBlocks, ISmsService smsService, IEmailService emailService, ApplicationUser sendingUser, IConfiguration configuration, DateTime reminderDay)
        {
            var messageLinesToSend = new List<MessageLine>();
            foreach (var block in messageLinesBlocks)
            {
                messageLinesToSend.AddRange(block.MessageLines.Where(line => line.ShouldBeSentTo));
            }

            var messages = new List<SmsMessage>();
            foreach (var messageLine in messageLinesToSend)
            {
                string to = $"+{IcelandicAreaCode}{messageLine.Number}";
                messages.Add(new SmsMessage { To = to, From = sendingUser.SendSmsName, Body = messageLine.Body });
            }

            if (!messages.Any()) return;

            smsService.SendBatch(messages, configuration["smsWebSenderCallbackUrl"]);

            if (sendingUser.SendSmsConfirmationToUser)
            {
                SendConfirmationSms(sendingUser, messages, smsService, reminderDay);

                // DEBUG
                // Also send to me so I can keep track of things
                await emailService.SendEmailAsync("gudbjorn.einarsson@gmail.com", "Confirmation email", $"Sendi {messages.Count} skilaboð á {sendingUser.UserName}", "hyldypi@hyldypi.is", "Hyldýpi");
            }
        }

        /// <summary>
        /// Send confirmation sms to user that sms messagse have been sent on his behalf
        /// </summary>
        /// <param name="user">User whos sms message where sent</param>
        /// <param name="messagesToSend">The messages that were sent</param>
        /// <param name="smsService">Sms service</param>
        /// <param name="reminderDay">The date that clients where reminded about</param>
        private static void SendConfirmationSms(ApplicationUser user, List<SmsMessage> messagesToSend, ISmsService smsService, DateTime reminderDay)
        {
            var message = new SmsMessage
            {
                To = $"+{IcelandicAreaCode}{user.UsersGsmNumber}",
                From = "Hyldypi",
                Body = $"Vorum ad senda {messagesToSend.Count} sms fyrir thig fyrir {reminderDay.ToString("dd.MM")}. Vid latum thig vita ef einhver theirra komast ekki til skila." 
            };

            smsService.SendMessage(message, "");
            
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
            _smsService.SendMessage(new SmsMessage { From = "Hyldypi", To = $"+{IcelandicAreaCode}{user.UsersGsmNumber}", Body = errorMessage }, "");

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
