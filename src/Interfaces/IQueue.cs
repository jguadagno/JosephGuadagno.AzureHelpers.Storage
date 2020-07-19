using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Interfaces
{
    public interface IQueue
    {
        /// <summary>
        /// This property is a reference to the QueueClient being used
        /// </summary>
        /// <remarks>This value of this property can be used for several methods in the <see cref="Queues"/> class</remarks>
        QueueClient QueueClient { get; }

        /// <summary>
        /// Adds a message to the queue
        /// </summary>
        /// <param name="message">The message to ass</param>
        /// <typeparam name="T">A serializable type</typeparam>
        /// <returns>A receipt of the message upon success</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="message"/> is null</exception>
        Task<SendReceipt> AddMessageAsync<T>(T message);

        /// <summary>
        /// Gets a number of messages from the queue
        /// </summary>
        /// <param name="numberOfMessages">The number of messages to retrieve from the queue. The default is 1.</param>
        /// <typeparam name="T">The object to deserialize the message to</typeparam>
        /// <returns>A list of <see cref="T"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the <see cref="numberOfMessages"/>is outside the range</exception>
        /// <remarks>The method can retrieve between 1 and 32 messages. Anything outside that will throw an ArgumentOutOfRangeException</remarks>
        Task<List<T>> GetMessagesAsync<T>(int numberOfMessages = 1);

        /// <summary>
        /// Clears all of the messages in the queue
        /// </summary>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <remarks>This has the same result of <see cref="Queue.DeleteAllMessagesAsync"/></remarks>
        Task<bool> ClearMessagesAsync();

        /// <summary>
        /// Deletes all of the messages in the queue
        /// </summary>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <remarks>This has the same result of <see cref="Queue.ClearMessagesAsync"/></remarks>
        Task<bool> DeleteAllMessagesAsync();
    }
}