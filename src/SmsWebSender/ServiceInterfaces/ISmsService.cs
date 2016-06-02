using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;


namespace SmsWebSender.ServiceInterfaces
{
    public delegate void MessagesFinishedEventHandler(List<Twilio.Message> processedMessages);

    public interface ISmsService
    {
        event MessagesFinishedEventHandler BatchProcessingFinished;

        void SendMessage(Models.Message messageToSend);
        void SendBatch(List<SmsWebSender.Models.Message> messagesToSend);
        List<Twilio.Message> GetMessages();
    }
}
