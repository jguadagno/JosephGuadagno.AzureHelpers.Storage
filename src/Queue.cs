using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    /// <summary>
    /// Provides methods to interact with Azure Storage queues
    /// </summary>
    public class Queue
    {
        /// <summary>
        /// This property is a reference to the QueueClient being used
        /// </summary>
        /// <remarks>This value of this property can be used for several methods in the <see cref="Queues"/> class</remarks>
        public QueueClient QueueClient { get; }

        /// <summary>
        /// Creates an instance of the Queue
        /// </summary>
        /// <param name="storageConnectionString">The connection string of the cloud storage account to use.</param>
        /// <param name="queueName"></param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="storageConnectionString"/> or <see cref="queueName"/> is null or empty</exception>
        public Queue(string storageConnectionString, string queueName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string parameter can not be null or empty.");
            }

            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name can not be null or empty.");
            }

            // Initialize QueueClient
            QueueClient = new QueueClient(storageConnectionString, queueName);
        }

        /// <summary>
        /// Creates an instance of the Queue
        /// </summary>
        /// <param name="queueServiceClient">The QueueServiceClient to use</param>
        /// <param name="queueName">The name of the queue</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueServiceClient"/> is null or the <see cref="queueName"/> is null or empty</exception>
        public Queue(QueueServiceClient queueServiceClient, string queueName)
        {
            if (queueServiceClient == null)
            {
                throw new ArgumentNullException(nameof(queueServiceClient),
                    "The queue service client can not be null.");
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

        /// <summary>
        /// Adds a message to the queue
        /// </summary>
        /// <param name="message">The message to ass</param>
        /// <typeparam name="T">A serializable type</typeparam>
        /// <returns>A receipt of the message upon success</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="message"/> is null</exception>
        public async Task<SendReceipt> AddMessageAsync<T>(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "The message cannot be null.");
            }

            return await QueueClient.SendMessageAsync(JsonSerializer.Serialize(message));
        }

        /// <summary>
        /// Gets a number of messages from the queue
        /// </summary>
        /// <param name="numberOfMessages">The number of messages to retrieve from the queue. The default is 1.</param>
        /// <typeparam name="T">The object to deserialize the message to</typeparam>
        /// <returns>A list of <see cref="T"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the <see cref="numberOfMessages"/>is outside the range</exception>
        /// <remarks>The method can retrieve between 1 and 32 messages. Anything outside that will throw an ArgumentOutOfRangeException</remarks>
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
                await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }

            return returnedMessages;
        }

        /// <summary>
        /// Clears all of the messages in the queue
        /// </summary>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <remarks>This has the same result of <see cref="DeleteAllMessagesAsync"/></remarks>
        public async Task<bool> ClearMessagesAsync()
        {
            var apiResponse = await QueueClient.ClearMessagesAsync();
            return apiResponse.Status == (int) HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Deletes all of the messages in the queue
        /// </summary>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <remarks>This has the same result of <see cref="ClearMessagesAsync"/></remarks>
        public async Task<bool> DeleteAllMessagesAsync()
        {
            return await ClearMessagesAsync();
        }
    }
}