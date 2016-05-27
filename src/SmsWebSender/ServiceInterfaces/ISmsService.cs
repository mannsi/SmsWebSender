using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;
using SmsWebSender.Models;
using SmsWebSender.ViewModels.Sms;
using Twilio;

namespace SmsWebSender.ServiceInterfaces
{
    public delegate void MessageEventHandler(Message message);

    public interface ISmsService
    {
        event MessageEventHandler MessageEvent;

        void SendSmsMessages(string senderName, int areaCode, List<MessageLinesBlock> messageLines);
        List<Message> GetMessages();
    }
}
