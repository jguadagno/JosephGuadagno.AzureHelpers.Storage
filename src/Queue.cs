using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    // TODO: Update XML Documentation

    /// <summary>
    /// Provides functions to interact with Azure Storage queues
    /// </summary>
    /// <remarks>This class mimics some of the QueueClient class</remarks>
    public class Queue
    {
        
        public QueueClient QueueClient { get; }
        
        /// <summary>
        /// Creates an instance of the <see cref="Queue"/> using the supplied <paramref name="storageConnectionString">Storage Account Url</paramref>
        /// </summary>
        /// <param name="storageConnectionString">The connection string of the cloud storage account to use.</param>
        /// <param name="queueName"></param>
        public Queue(string storageConnectionString, string queueName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString), "The storage connection string parameter can not be null or empty.");
            }
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name can not be null or empty.");
            }
            
            // Initialize QueueClient
            QueueClient = new QueueClient(storageConnectionString, queueName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueServiceClient"></param>
        /// <param name="queueName"></param>
        public Queue(QueueServiceClient queueServiceClient, string queueName)
        {
            if (queueServiceClient == null)
            {
                throw new ArgumentNullException(nameof(queueServiceClient), "The queue service client can not be null.");
            }

            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name can not be null or empty.");
            }
            
            QueueClient = queueServiceClient.GetQueueClient(queueName);
        }


        // TODO: Implement the methods in the future
        // DeleteMessageAsync
        // PeekMessagesAsync
        // UpdateMessageAsync
        public async Task<SendReceipt> AddMessageAsync<T>(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "The message cannot be null.");
            }
            
            return await QueueClient.SendMessageAsync(JsonSerializer.Serialize(message));
        }
        
        public async Task<List<T>> GetMessagesAsync<T>(int numberOfMessages = 1)
        {

            if (numberOfMessages <= 0 || numberOfMessages > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfMessages), numberOfMessages,
                    "The number of messages to receive must be greater than 0 and less than 33");
            }
            
            var returnedMessages = new List<T>();
            var apiResponse = await QueueClient.ReceiveMessagesAsync(maxMessages: numberOfMessages);
            var messages = apiResponse.Value;
            
            foreach (var message in messages)
            {
                returnedMessages.Add(JsonSerializer.Deserialize<T>(message.MessageText));
                await QueueClient.DeleteMessageAsync(message.MessageId,message.PopReceipt);
            }

            return returnedMessages;
        }

        public async Task<bool> ClearMessagesAsync()
        {
            var apiResponse = await QueueClient.ClearMessagesAsync();
            return apiResponse.Status == (int) HttpStatusCode.NoContent;
        }
        
        public async Task<bool> DeleteAllMessagesAsync()
        {
            return await ClearMessagesAsync();
        }
    }
}