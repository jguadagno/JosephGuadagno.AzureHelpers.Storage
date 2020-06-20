using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    // TODO: Update XML Documentation
    /// <summary>
    /// Provides functions to interact with Blob Storage
    /// </summary>
    public class Blobs
    {
        public BlobContainerClient BlobContainerClient { get; }
        
        public Blobs(string storageConnectionString, string containerName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new  ArgumentNullException(nameof(storageConnectionString), "The storage connection string cannot be null or empty");
            }
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }
            
            BlobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
        }

        public Blobs(BlobContainerClient blobContainerClient)
        {
            if (blobContainerClient == null)
            {
                throw new ArgumentNullException(nameof(blobContainerClient), "The blob container client cannot be null");
            }
            
            BlobContainerClient = blobContainerClient;
        }
        
        // TODO: Implement the methods in the future
        // CreateSnapshotAsync
        // DownloadTo with overloads
        // GetBlobsAsync

        public async Task<bool> DeleteAsync(string blobName, DeleteSnapshotsOption deleteSnapshotOption = DeleteSnapshotsOption.IncludeSnapshots)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            
            try
            {
                var apiResponse = await BlobContainerClient.DeleteBlobAsync(blobName, deleteSnapshotOption);
                return apiResponse.Status == (int)HttpStatusCode.Accepted;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                // Ignore any errors if the queue is being deleted or not found
                return true;
            }
        }

        public async Task<bool> UndeleteAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            try
            {
                var blobClient = BlobContainerClient.GetBlobClient(blobName);
                var apiResponse = await blobClient.UndeleteAsync();
                return apiResponse.Status == (int) HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BlobDownloadInfo> DownloadAsync(string blobName)
        {
            // Download
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            var blobClient = BlobContainerClient.GetBlobClient(blobName);
            return await blobClient.DownloadAsync();
        }

        public async Task<bool> DownloadToAsync(string blobName, Stream destinationStream)
        {
            // DownloadTo
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            if (destinationStream == null)
            {
                throw new ArgumentNullException(nameof(destinationStream), "The stream can not be null");
            }
            
            try
            {
                var blobClient = BlobContainerClient.GetBlobClient(blobName);
                var apiResponse = await blobClient.DownloadToAsync(destinationStream);
                // The status does not matter for DownloadTo and it returns a PartialContent
                // At least with a small file and Azurite
                return apiResponse.Status == (int) HttpStatusCode.PartialContent;
            }
            catch (RequestFailedException ex) 
                when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                return false;
            }
        }
        
        public async Task<bool> DownloadToAsync(string blobName, string path)
        {
            // DownloadTo
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "The path cannot be null or empty");
            }

            try
            {
                var blobClient = BlobContainerClient.GetBlobClient(blobName);
                var apiResponse = await blobClient.DownloadToAsync(path);
                // The status does not matter for DownloadTo and it returns a PartialContent
                // At least with a small file and Azurite
                return apiResponse.Status == (int) HttpStatusCode.PartialContent;
            }
            catch (RequestFailedException ex) 
                when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            
            var blobClient = BlobContainerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }

        public async Task<BlobContentInfo> UploadAsync(string blobName, Stream sourceStream, bool overwriteIfExists = false)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            if (sourceStream == null)
            {
                throw new ArgumentNullException(nameof(sourceStream), "The source stream can not be null");
            }

            if (overwriteIfExists)
            {
                return await UploadAndOverwriteIfExistsAsync(blobName, sourceStream);
            }

            return await BlobContainerClient.UploadBlobAsync(blobName, sourceStream);
        }

        public async Task<BlobContentInfo> UploadAsync(string blobName, string filename, bool overwriteIfExists = false)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename), "The filename cannot be null or empty");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("The file to upload was not found", filename);
            }

            using (var streamReader = new StreamReader(filename))
            {
                return await UploadAsync(blobName, streamReader.BaseStream, overwriteIfExists);
            }
        }
        
        public async Task<BlobContentInfo> UploadAndOverwriteIfExistsAsync(string blobName, Stream sourceStream)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            if (sourceStream == null)
            {
                throw new ArgumentNullException(nameof(sourceStream), "The source stream can not be null");
            }

            var blobClient = BlobContainerClient.GetBlobClient(blobName);
            return await blobClient.UploadAsync(sourceStream, true);
        }
        public async Task<BlobContentInfo> UploadAndOverwriteIfExistsAsync(string blobName, string filename)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename), "The filename cannot be null or empty");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("The file to upload was not found", filename);
            }

            using (var streamReader = new StreamReader(filename))
            {
                return await UploadAndOverwriteIfExistsAsync(blobName, streamReader.BaseStream);
            }
        }
    }
}
