using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    public static class QueueTestHelper
    {
        public const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
		
        public static QueueClient CreateQueue(string queueName)
        {
            // Create a test queue
            var queueServiceClient = new QueueServiceClient(DevelopmentConnectionString);
            var apiResponse = queueServiceClient.CreateQueue(queueName);
            return apiResponse.Value;
        }

        public static bool DeleteQueue(QueueClient queueClient)
        {
            var apiResponse = queueClient.Delete();
            return apiResponse.Status == (int) HttpStatusCode.OK;
        }

        public static bool DoesQueueExist(string queueName)
        {
            var queueClient = new QueueClient(DevelopmentConnectionString, queueName);
            return queueClient.Exists().Value;
        }

        public static string GetTemporaryQueueName()
        {
            var dateString = DateTime.Now.ToString("O")
                .Replace("/", "-")
                .Replace(":", "-")
                .Replace("T", "-")
                .Replace(".", "-");
            var randomNumber = new Random().Next(1, 1000).ToString()
                .PadLeft(4, '0');
            return $"test-{dateString}-{randomNumber}";
        }

        public static List<T> GetMessages<T>(string queueName)
        {
            var queueServiceClient = new QueueServiceClient(DevelopmentConnectionString);
            var queue = queueServiceClient.GetQueueClient(queueName);

            var apiResponse = queue.PeekMessages();
            var messages = apiResponse.Value;

            return messages.Select(message => JsonSerializer.Deserialize<T>(message.MessageText)).ToList();
        }
        
        public static PeekedMessage[] PeekMessages(string queueName, int numberOfMessages = 1)
        {
            var queueServiceClient = new QueueServiceClient(DevelopmentConnectionString);
            var queue = queueServiceClient.GetQueueClient(queueName);

            var apiResponse = queue.PeekMessages(numberOfMessages);
            return apiResponse.Value;
        }

        public static SendReceipt AddMessage<T>(QueueClient queue, T message)
        {
            queue.CreateIfNotExists();

            var serializedMessage = JsonSerializer.Serialize(message);
            var apiResponse = queue.SendMessage(serializedMessage);
            return apiResponse.Value;
        }
    }
}