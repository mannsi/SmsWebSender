using System.Collections.Generic;
using SmsWebSender.Models;


namespace SmsWebSender.ServiceInterfaces
{
    public delegate void MessagesFinishedEventHandler(List<Twilio.Message> processedMessages);

    public interface ISmsService
    {
        void SendMessage(SmsMessage messageToSend, string callbackUrl);
        void SendBatch(List<SmsMessage> messagesToSend, string callbackUrl);
        List<Twilio.Message> GetMessages();
    }
}
