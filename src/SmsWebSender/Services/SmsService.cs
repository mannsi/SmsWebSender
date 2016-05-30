using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using Twilio;

namespace SmsWebSender.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        public event MessageEventHandler MessageEvent;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public void SendSmsMessages(string senderName, int areaCode, List<MessageLinesBlock> messageLinesBlocks)
        //{
        //    foreach (var block in messageLinesBlocks)
        //    {
        //        foreach (var messageLine in block.MessageLines.Where(line => line.ShouldBeSentTo))
        //        {
        //            string to = $"+{areaCode}{messageLine.Number}";
        //            SendMessage(senderName, to, messageLine.SmsText);
        //        }
        //    }
        //}

        public List<Message> GetMessages()
        {
            var twilio = GetTwilioClient();
            var options = new MessageListRequest
            {
                Count = 100
            };
            return twilio.ListMessages(options).Messages;
        }

        public void SendMessage(string from, string to, string body)
        {
            //var twilio = GetTwilioClient();
            //twilio.SendMessage(from, to, body, Callback);
        }

        private void Callback(Message message)
        {
            MessageEvent?.Invoke(message);
        }

        private TwilioRestClient GetTwilioClient()
        {
            string accountSid = _configuration["TwilioAccountSid"];
            string authToken = _configuration["TwilioAuthToken"];
            var twilio = new TwilioRestClient(accountSid, authToken);
            return twilio;
        }
    }
}
