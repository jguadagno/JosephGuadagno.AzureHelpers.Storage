using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Interfaces
{
    public interface IQueues
    {
        /// <summary>
        /// Provides a reference to the QueueServiceClient for Azure
        /// </summary>
        QueueServiceClient QueueServiceClient { get; }

        /// <summary>
        /// Gets a reference to a Queue
        /// </summary>
        /// <param name="queueName">The name of the Queue</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
        QueueClient GetQueueClient(string queueName);

        /// <summary>
        /// Creates a queue
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>A QueueClient if successful</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
        Task<QueueClient> CreateQueueAsync(string queueName);

        /// <summary>
        /// Deletes the queue
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="queueName"/> is null or empty</exception>
        Task<bool> DeleteQueueAsync(string queueName);

        /// <summary>
        /// Gets a list of all the queues within this storage account
        /// </summary>
        /// <returns>A list of QueueItems</returns>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.queues.models.queueitem?view=azure-dotnet for more info on the QueueItem object </remarks>
        Task<List<QueueItem>> GetQueuesAsync();
    }
}