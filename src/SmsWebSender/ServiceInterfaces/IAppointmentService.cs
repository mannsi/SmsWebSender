using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmsWebSender.Models;

namespace SmsWebSender.ServiceInterfaces
{
    public interface IAppointmentService
    {
        List<Appointment> AppointmentsForDay(DateTime day);
    }
}
