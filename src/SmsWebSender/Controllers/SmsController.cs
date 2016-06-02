using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Metadata.Internal;
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
        private const int IcelandicAreaCode = 354;

        public SmsController(IAppointmentService appointmentService,
            ISmsService smsService, 
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _smsService = smsService;
            _userManager = userManager;
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
            var appointmentsForDay = _appointmentService.AppointmentsForDay(date);
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
            var sendingUser = await _userManager.FindByIdAsync(User.GetUserId());

            var messageLinesToSend = new List<MessageLine>();
            foreach (var block in messageLinesBlocks)
            {
                messageLinesToSend.AddRange(block.MessageLines.Where(line => line.ShouldBeSentTo));
            }

            var messages = new List<Models.Message>();
            foreach (var messageLine in messageLinesToSend)
            {
                string to = $"+{IcelandicAreaCode}{messageLine.Number}";
                messages.Add(new Models.Message {To = to, From = sendingUser.SendSmsName , Body = messageLine.Body });
            }
            _smsService.SendBatch(messages);

            return true;
        }

        private void SmsServiceOnBatchProcessingFinished(List<Twilio.Message> processedMessages)
        {
            int unsuccessfulMessage = 0;
            int successfulMessage = 0;

            foreach (var processedMessage in processedMessages)
            {
                if (processedMessage.ErrorCode.HasValue)
                {
                    unsuccessfulMessage++;
                }
                else
                {
                    successfulMessage++;
                }
            }

            string statusSendMessage = $"Vorum ad senda sms fyrir thig. Thad sendust {successfulMessage} sms. ";
            if (unsuccessfulMessage > 0)
            {
                statusSendMessage += $"Hinsvegar tokst ekki ad senda {unsuccessfulMessage} sms. Skodadu vefsiduna okkar til ad sja hvad for urskeidis. ";
            }

            statusSendMessage += $"Kv, Hyldypi";

            //_smsService.SendMessage(new Models.Message {To = "+3546643507", From="Hyldypi", Body = statusSendMessage});
        }
    }
}
