using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using Twilio;
using Message = Twilio.Message;

namespace SmsWebSender.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private List<string> MessagesNotProcessed { get; set; }
        private List<Twilio.Message> ProcessedMessages { get; set; }
        
        public event MessagesFinishedEventHandler BatchProcessingFinished;
        

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            MessagesNotProcessed = new List<string>();
            ProcessedMessages = new List<Message>();
        }

        public List<Twilio.Message> GetMessages()
        {
            var twilio = GetTwilioClient();
            var options = new Twilio.MessageListRequest
            {
                Count = 100
            };
            return twilio.ListMessages(options).Messages;
        }

        public void SendMessage(Models.Message messageToSend)
        {
            string accountSid = _configuration["TwilioAccountSid"];
            string authToken = _configuration["TwilioAuthToken"];
            var twilio = new Twilio.TwilioRestClient(accountSid, authToken);
            twilio.SendMessage(messageToSend.From, messageToSend.To, messageToSend.Body, CallbackFunction);
        }

        private void CallbackFunction(Twilio.Message twilioMessage)
        {
            Debug.WriteLine($"STATUS: {twilioMessage.Status}");
        }

        public void SendBatch(List<Models.Message> messagesToSend)
        {
            foreach (var message in messagesToSend)
            {
                MessagesNotProcessed.Add(message.Id);
                SendMessage(message);
            }
        }

        private TwilioRestClient GetTwilioClient()
        {
            string accountSid = _configuration["TwilioAccountSid"];
            string authToken = _configuration["TwilioAuthToken"];
            var twilio = new TwilioRestClient(accountSid, authToken);
            return twilio;
        }

        private void AllMessagesProcessed()
        {
            BatchProcessingFinished?.Invoke(ProcessedMessages);
        }
    }
}
