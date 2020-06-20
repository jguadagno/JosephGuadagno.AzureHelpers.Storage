using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    // TODO: Update XML Documentation

    public class Containers
    {
        public BlobServiceClient BlobServiceClient { get; }

        public Containers(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new  ArgumentNullException(nameof(storageConnectionString), "The storage connection string cannot be null or empty");
            }
            BlobServiceClient = new BlobServiceClient(storageConnectionString);
        }

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

        public async Task<bool> DeleteContainerAsync(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }
            
            try
            {
                var apiResponse = await BlobServiceClient.DeleteBlobContainerAsync(containerName);
                return (apiResponse.Status == (int)HttpStatusCode.NoContent || apiResponse.Status== (int)HttpStatusCode.Accepted);
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted || ex.ErrorCode == BlobErrorCode.ContainerNotFound)
            {
                // Ignore any errors if the blob is being deleted or not found
                return true;
            }
        }

        public BlobContainerClient GetContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }

            return BlobServiceClient.GetBlobContainerClient(containerName);
        }

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