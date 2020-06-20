using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    // TODO: Update XML Documentation

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>This class mimics functions of the QueueServiceClient</remarks>
    public class Queues
    {
        
        public QueueServiceClient QueueServiceClient { get; }
        
        public Queues(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString), "The storage connection string parameter must not be null or empty.");
            }
            
            QueueServiceClient = new QueueServiceClient(storageConnectionString);
        }

        /// <summary>
        /// Gets a reference to a Queue
        /// </summary>
        public QueueClient GetQueueClient(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name cannot be null or empty.");
            }
            
            return QueueServiceClient.GetQueueClient(queueName);
        }

        /// <summary>
        /// Creates a Queue
        /// </summary>
        /// <param name="queueName">The name of the queue to create</param>
        /// <returns></returns>
        public async Task<QueueClient> CreateQueueAsync(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name cannot be null or empty.");
            }
            
            try
            {
                // Try to create a queue
                var apiResponse = await QueueServiceClient.CreateQueueAsync(queueName);
                return apiResponse.Value;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueAlreadyExists)
            {
                // Ignore any errors if the queue already exists
                return QueueServiceClient.GetQueueClient(queueName);
            }
        }

        public async Task<bool> DeleteQueueAsync(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name cannot be null or empty.");
            }
            
            try
            {
                var apiResponse = await QueueServiceClient.DeleteQueueAsync(queueName);
                return apiResponse.Status == (int)HttpStatusCode.NoContent;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueBeingDeleted || ex.ErrorCode == QueueErrorCode.QueueNotFound)
            {
                // Ignore any errors if the queue is being deleted or not found
                return true;
            }
        }
        
        public async Task<List<QueueItem>> GetQueuesAsync()
        {
            var apiResponse = QueueServiceClient.GetQueuesAsync();
            var enumerator = apiResponse.GetAsyncEnumerator();
            var queueItems = new List<QueueItem>();
            
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var queueItem = enumerator.Current;
                    queueItems.Add(queueItem);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return queueItems;
        }
    }
}