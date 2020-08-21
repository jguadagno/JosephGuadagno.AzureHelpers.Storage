using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using JosephGuadagno.AzureHelpers.Storage.Interfaces;

namespace JosephGuadagno.AzureHelpers.Storage
{
    /// <summary>
    /// Provides methods for interactive with a Queue in Azure Storage Queues
    /// </summary>
    /// <remarks>Use the <see cref="GetQueueClient"/> to create create an instance of the <see cref="Queue"/> class</remarks>
    public class Queues : IQueues
    {
        /// <summary>
        /// Provides a reference to the QueueServiceClient for Azure
        /// </summary>
        public QueueServiceClient QueueServiceClient { get; }

        /// <summary>
        /// Creates an instance of the Queues class
        /// </summary>
        /// <param name="storageConnectionString">The connection string to use</param>
        /// <exception cref="ArgumentNullException">Can be thrown if either the <see cref="storageConnectionString"/> is null or empty</exception>
        public Queues(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string parameter must not be null or empty.");
            }

            QueueServiceClient = new QueueServiceClient(storageConnectionString);
        }

        // TODO: Add Unit Test around this.
        // TODO: Update code documentation
        public Queues(string accountName, Azure.Core.TokenCredential tokenCredential)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                throw new ArgumentNullException(nameof(accountName), "The Azure Storage Account name cannot be null or empty");
            }

            if (tokenCredential == null)
            {
                tokenCredential = new DefaultAzureCredential();
            }

            var accountEndpoint = $"https://{accountName}.queue.core.windows.net/";
            QueueServiceClient = new QueueServiceClient(new Uri(accountEndpoint), tokenCredential);
        }

        /// <summary>
        /// Gets a reference to a Queue
        /// </summary>
        /// <param name="queueName">The name of the Queue</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
        public QueueClient GetQueueClient(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name cannot be null or empty.");
            }

            return QueueServiceClient.GetQueueClient(queueName);
        }

        /// <summary>
        /// Creates a queue
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>A QueueClient if successful</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
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

        /// <summary>
        /// Deletes the queue
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
        public async Task<bool> DeleteQueueAsync(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName), "The queue name cannot be null or empty.");
            }

            try
            {
                var apiResponse = await QueueServiceClient.DeleteQueueAsync(queueName);
                return apiResponse.Status == (int) HttpStatusCode.NoContent;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueBeingDeleted || ex.ErrorCode == QueueErrorCode.QueueNotFound)
            {
                // Ignore any errors if the queue is being deleted or not found
                return true;
            }
        }

        /// <summary>
        /// Gets a list of all the queues within this storage account
        /// </summary>
        /// <returns>A list of QueueItems</returns>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.queues.models.queueitem?view=azure-dotnet for more info on the QueueItem object </remarks>
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