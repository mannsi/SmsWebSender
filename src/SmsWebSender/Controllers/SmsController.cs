using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using RestSharp;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using SmsWebSender.ViewModels.Sms;
using Twilio;

namespace SmsWebSender.Controllers
{
    [Route("sms")]
    public class SmsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private const int IcelandicAreaCode = 354;

        public SmsController(IAppointmentService appointmentService,
            ISmsService smsService, 
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _appointmentService = appointmentService;
            _smsService = smsService;
            _userManager = userManager;
            _dbContext = dbContext;
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

            foreach (var block in messageLinesBlocks)
            {
                foreach (var messageLine in block.MessageLines.Where(line => line.ShouldBeSentTo))
                {
                    string to = $"+{IcelandicAreaCode}{messageLine.Number}";
                    _smsService.SendMessage(sendingUser.SendSmsName, to, messageLine.SmsText);
                }
            }

            return true;
        }
    }
}
