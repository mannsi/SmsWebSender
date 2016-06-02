using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;

namespace SmsWebSender.Services
{
    public class AppointmentService: IAppointmentService
    {
        private readonly IConfiguration _configuration;

        public AppointmentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Appointment> AppointmentsForDay(DateTime day)
        {
            var startOfDay = new DateTime(day.Year, day.Month, day.Day);
            var endOfDay = new DateTime(day.Year, day.Month, day.Day).AddDays(1);

            return GetAppointmentsForPeriod(startOfDay, endOfDay);
        }

        private List<Appointment> GetAppointmentsForPeriod(DateTime startTime, DateTime endTime)
        {
            List<Appointment> appointments = new List<Appointment>();
            var service = GetCalendarService();
            var calendarList = service.CalendarList.List().Execute().Items;

            foreach (var calendar in calendarList)
            {
                var request = service.Events.List(calendar.Id);
                request.TimeMin = startTime;
                request.TimeMax = endTime;

                var results = request.Execute().Items;
                foreach (var result in results)
                {
                    string clientName = result.Summary;
                    var startTimeOfAppointment = DateTime.MinValue;
                    if (result.Start.DateTime.HasValue)
                    {
                        startTimeOfAppointment = result.Start.DateTime.Value;
                    }

                    int gsmNumber = GetGsmNumberFromSummary(result.Summary);

                    appointments.Add(new Appointment
                    {
                        ClientName = clientName,
                        GsmNumber = gsmNumber,
                        StartTimeOfAppointment = startTimeOfAppointment,
                        EmployeeName = calendar.Summary,
                        CalendarId = calendar.Id
                    });
                }
            }
            return appointments;
        }

        private int GetGsmNumberFromSummary(string summary)
        {
            int gsmNumber = 0;
            string regexPhoneNumberPattern = @"[6-9]\d{2}[ ,-]?\d{4}";
            var regex = new Regex(regexPhoneNumberPattern);
            var regexNumberMatch = regex.Match(summary);
            if (regexNumberMatch.Success)
            {
                var matchString = regexNumberMatch.Value;
                matchString = matchString.Replace(" ", "");
                matchString = matchString.Replace("-", "");
                int.TryParse(matchString, out gsmNumber);
            }
            return gsmNumber;
        }

        private CalendarService GetCalendarService()
        {
            GoogleCredential credential;

            using (var stream = new FileStream(_configuration["GoogleCertPath"], FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(CalendarService.Scope.CalendarReadonly);
            }

            // Create the service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AdgangurAdBokunum",
            });
            return service;
        }
    }
}
