using System.Collections.Generic;


namespace SmsWebSender.ServiceInterfaces
{
    public delegate void MessagesFinishedEventHandler(List<Twilio.Message> processedMessages);

    public interface ISmsService
    {
        void SendMessage(Models.Message messageToSend);
        void SendBatch(List<SmsWebSender.Models.Message> messagesToSend);
        List<Twilio.Message> GetMessages();
    }
}
