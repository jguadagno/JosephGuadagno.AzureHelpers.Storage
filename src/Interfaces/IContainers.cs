using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Interfaces
{
    public interface IContainers
    {
        /// <summary>
        /// The reference to the Blob Service Client
        /// </summary>
        BlobServiceClient BlobServiceClient { get; }

        /// <summary>
        /// Creates a container
        /// </summary>
        /// <param name="containerName">The name of the container</param>
        /// <returns>A BlobContainer info upon success</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontainerinfo?view=azure-dotnet for more info on the BlobContainerInfo object</remarks>
        Task<BlobContainerClient> CreateContainerAsync(string containerName);

        /// <summary>
        /// Deletes the container
        /// </summary>
        /// <param name="containerName">The name of the container</param>
        /// <returns>True, if successful, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        Task<bool> DeleteContainerAsync(string containerName);

        /// <summary>
        /// Returns a reference to the container
        /// </summary>
        /// <param name="containerName">The container name</param>
        /// <returns>A reference to the container, if successful</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="containerName"/> is null or empty</exception>
        BlobContainerClient GetContainer(string containerName);

        /// <summary>
        /// Returns a list of all of the containers in the current storage account
        /// </summary>
        /// <returns>A List of BlobContainerItems</returns>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontaineritem?view=azure-dotnet for more info on the BlobContainerItem object</remarks>
        Task<List<BlobContainerItem>> GetContainersAsync();
    }
}