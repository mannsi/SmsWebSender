using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;

namespace SmsWebSender.Services
{
    public class AppointmentService: IAppointmentService
    {
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
                    int gsmNumber = 0;
                    int.TryParse(result.Description, out gsmNumber);

                    var startTimeOfAppointment = DateTime.MinValue;
                    if (result.Start.DateTime.HasValue)
                    {
                        startTimeOfAppointment = result.Start.DateTime.Value;
                    }

                    appointments.Add(new Appointment
                    {
                        ClientName = result.Summary,
                        GsmNumber = gsmNumber,
                        StartTimeOfAppointment = startTimeOfAppointment,
                        EmployeeName = calendar.Summary,
                        CalendarId = calendar.Id
                    });
                }
            }
            return appointments;
        }

        private static CalendarService GetCalendarService()
        {
            String serviceAccountEmail = "hyldypi@calendaraccessor-1322.iam.gserviceaccount.com";

            var certificate = new X509Certificate2(@"..\key.p12", "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[]
                   {
                       CalendarService.Scope.Calendar, // Manage your calendars
 	                    CalendarService.Scope.CalendarReadonly // View your CalendarsCalendarService.Scope.CalendarReadonly
                   }
               }.FromCertificate(certificate));

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
