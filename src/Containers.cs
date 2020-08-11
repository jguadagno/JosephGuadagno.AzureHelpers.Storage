using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JosephGuadagno.AzureHelpers.Storage.Interfaces;

namespace JosephGuadagno.AzureHelpers.Storage
{
    /// <summary>
    /// Provides methods to interact with Containers in an Azure storage
    /// </summary>
    public class Containers : IContainers
    {
        /// <summary>
        /// The reference to the Blob Service Client
        /// </summary>
        public BlobServiceClient BlobServiceClient { get; }

        /// <summary>
        /// Creates an instance of a Container
        /// </summary>
        /// <param name="storageConnectionString">The connection string to use</param>
        /// <exception cref="ArgumentNullException">Can be thrown if either the <see cref="storageConnectionString"/> is null or empty</exception>
        public Containers(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string cannot be null or empty");
            }

            BlobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        // TODO: Add Unit Test around this.
        // TODO: Update code documentation
        public Containers(string accountName, Azure.Core.TokenCredential tokenCredential, BlobClientOptions blobClientOptions = null)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                throw new ArgumentNullException(nameof(accountName), "The Azure Storage Account name cannot be null or empty");
            }
            if (tokenCredential == null)
            {
                tokenCredential = new DefaultAzureCredential();
            }

            var accountEndpoint = $"https://{accountName}.blob.core.windows.net/";
            BlobServiceClient = new BlobServiceClient(new Uri(accountEndpoint), tokenCredential, blobClientOptions);
        }

        /// <summary>
        /// Creates a container
        /// </summary>
        /// <param name="containerName">The name of the container</param>
        /// <returns>A BlobContainer info upon success</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontainerinfo?view=azure-dotnet for more info on the BlobContainerInfo object</remarks>
        public async Task<BlobContainerClient> CreateContainerAsync(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }

            //TODO: Add the ability to enable soft delete
            // if (retainForNumberOfDays > 0)
            // {
            //     var serviceProperties = new BlobServiceProperties
            //     {
            //         DeleteRetentionPolicy = {Enabled = true, Days = retainForNumberOfDays}
            //     };
            //     BlobServiceClient.SetProperties(serviceProperties);
            // }

            try
            {
                var apiResponse = await BlobServiceClient.CreateBlobContainerAsync(containerName);
                return apiResponse.Value;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
            {
                return BlobServiceClient.GetBlobContainerClient(containerName);
            }
        }

        /// <summary>
        /// Deletes the container
        /// </summary>
        /// <param name="containerName">The name of the container</param>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        public async Task<bool> DeleteContainerAsync(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }

            try
            {
                var apiResponse = await BlobServiceClient.DeleteBlobContainerAsync(containerName);
                return (apiResponse.Status == (int) HttpStatusCode.NoContent ||
                        apiResponse.Status == (int) HttpStatusCode.Accepted);
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted ||
                      ex.ErrorCode == BlobErrorCode.ContainerNotFound)
            {
                // Ignore any errors if the blob is being deleted or not found
                return true;
            }
        }

        /// <summary>
        /// Returns a reference to the container
        /// </summary>
        /// <param name="containerName">The container name</param>
        /// <returns>A reference to the container, if successful</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        public BlobContainerClient GetContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }

            return BlobServiceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        /// Returns a list of all of the containers in the current storage account
        /// </summary>
        /// <returns>A List of BlobContainerItems</returns>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontaineritem?view=azure-dotnet for more info on the BlobContainerItem object</remarks>
        public async Task<List<BlobContainerItem>> GetContainersAsync()
        {
            var apiResponse = BlobServiceClient.GetBlobContainersAsync();
            var enumerator = apiResponse.GetAsyncEnumerator();
            var blobContainerItems = new List<BlobContainerItem>();

            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var blobContainerItem = enumerator.Current;
                    blobContainerItems.Add(blobContainerItem);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return blobContainerItems;
        }
    }
}