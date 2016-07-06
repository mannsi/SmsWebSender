using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
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

        public void SendMessage(SmsMessage messageToSend, string callbackUrl)
        {
            string accountSid = _configuration["TwilioAccountSid"];
            string authToken = _configuration["TwilioAuthToken"];
            var twilio = new TwilioRestClient(accountSid, authToken);
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                twilio.SendMessage(messageToSend.From, messageToSend.To, messageToSend.Body, callbackUrl);
            }
            else
            {
                twilio.SendMessage(messageToSend.From, messageToSend.To, messageToSend.Body);
            }
        }

        public void SendBatch(List<SmsMessage> messagesToSend, string callbackUrl)
        {
            foreach (var message in messagesToSend)
            {
                SendMessage(message, callbackUrl);
            }
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
