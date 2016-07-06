using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;

namespace SmsWebSender.Jobs.Sms
{
    public class JobStart
    {
        private static ITrigger TimeTrigger()
        {
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule("0 0 8-22 1/1 * ? *") // Every hour between 8-22
                //.WithCronSchedule("0 0/1 * 1/1 * ? *") // Every 1 minute
                .Build();

            return trigger;
        }

        public static void Start(
            ISmsService service, 
            IAppointmentService appointmentService, 
            IEmailService emailService, 
            ApplicationDbContext dbcontext, 
            IConfiguration configuration)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<SendSmsJob>().Build();
            job.JobDataMap.Add("smsService", service);
            job.JobDataMap.Add("appointmentService", appointmentService);
            job.JobDataMap.Add("emailService", emailService);
            job.JobDataMap.Add("dbcontext", dbcontext);
            job.JobDataMap.Add("configuration", configuration);
            scheduler.ScheduleJob(job, TimeTrigger());
        }
    }
}