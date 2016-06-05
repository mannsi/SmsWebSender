﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using Quartz;
using SmsWebSender.Controllers;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using System.Linq;

namespace SmsWebSender.Jobs.Sms
{
    public class SendSmsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            // A single run of this function fetches data for tomorrow and 3 days from now and sends sms reminders to everybody that have appointments there.
            var appointmentService = (IAppointmentService) context.JobDetail.JobDataMap["appointmentService"];
            var smsService = (ISmsService) context.JobDetail.JobDataMap["smsService"];
            var emailService = (IEmailService)context.JobDetail.JobDataMap["emailService"];
            var dbcontext = (ApplicationDbContext) context.JobDetail.JobDataMap["dbcontext"];
            var configuration = (IConfiguration) context.JobDetail.JobDataMap["configuration"];

            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var threeDaysFromNow = today.AddDays(3);

            var allUsers = dbcontext.Users.ToList();
            foreach (var user in allUsers)
            {
                // TODO Commented out for testing. 
                //if (user.ShouldAutoSendSms && DateTime.Now.Hour == user.AutoSendHour)
                {
                    SendForDay(tomorrow, appointmentService, user, smsService, configuration, emailService);
                    SendForDay(threeDaysFromNow, appointmentService, user, smsService, configuration, emailService);
                }
            }
        }

        private static void SendForDay(DateTime date, IAppointmentService appointmentService, ApplicationUser sendingUser, ISmsService smsService, IConfiguration configuration, IEmailService emailService)
        {
            var messageLinesBlocks = SmsController.GetMessageLineBlocks(date, appointmentService, sendingUser, true);

            // Debug code
            // =============================================================
            var numberOfMessage = (from block in messageLinesBlocks
                from messageLine in block.MessageLines
                select messageLine).Count();
            var debugMessage =
                $"Hefði sent sms skeyti á {messageLinesBlocks.Count} calendars. Heildarfjöldi skeyta hefði verið {numberOfMessage}";
            emailService.SendEmailAsync("gudbjorn.einarsson@gmail.com", "Prófun á quartz", debugMessage,
                "hyldypi@hyldypi.is", "Hyldýpi").Wait();
            // =============================================================


            //var sendTomorrowTask = SmsController.SendBatch(messageLinesTomorrow, smsService, userManager, sendingUserId,
            //    configuration);
            //sendTomorrowTask.Wait();
        }
    }
}
