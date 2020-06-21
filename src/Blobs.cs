using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace JosephGuadagno.AzureHelpers.Storage
{
    /// <summary>
    /// Provides methods to interact with Blobs in an Azure storage <see cref="Containers"/>
    /// </summary>
    public class Blobs
    {
        /// <summary>
        /// A reference to the current Blob Container being used
        /// </summary>
        /// <remarks>You can use this property for a most of the methods in the <see cref="Containers"/> class</remarks>
        public BlobContainerClient BlobContainerClient { get; }

        /// <summary>
        /// Creates an instance of Blobs
        /// </summary>
        /// <param name="storageConnectionString">The connection string to use</param>
        /// <param name="containerName">The name of the container to access</param>
        /// <exception cref="ArgumentNullException">Can be thrown if either the <see cref="storageConnectionString"/> or <see cref="containerName"/> is null or empty</exception>
        public Blobs(string storageConnectionString, string containerName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string cannot be null or empty");
            }

            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "The container name cannot be null or empty");
            }

            BlobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
        }

        /// <summary>
        /// Creates an instance of Blobs
        /// </summary>
        /// <param name="blobContainerClient">The container to initialize Blobs with</param>
        /// <exception cref="ArgumentNullException">Can be thrown if the <see cref="blobContainerClient"/> is null</exception>
        public Blobs(BlobContainerClient blobContainerClient)
        {
            if (blobContainerClient == null)
            {
                throw new ArgumentNullException(nameof(blobContainerClient),
                    "The blob container client cannot be null");
            }

            BlobContainerClient = blobContainerClient;
        }

        // TODO: Implement the methods in the future
        // CreateSnapshotAsync
        // DownloadTo with overloads
        // GetBlobsAsync

        /// <summary>
        /// Deletes a blob
        /// </summary>
        /// <param name="blobName">The name of the blob to delete</param>
        /// <param name="deleteSnapshotOption">The set of options describing delete operation.</param>
        /// <returns>True, if the blob was deleted or did not exist, otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> parameter is null or empty.</exception>
        public async Task<bool> DeleteAsync(string blobName,
            DeleteSnapshotsOption deleteSnapshotOption = DeleteSnapshotsOption.IncludeSnapshots)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            try
            {
                var apiResponse = await BlobContainerClient.DeleteBlobAsync(blobName, deleteSnapshotOption);
                return apiResponse.Status == (int) HttpStatusCode.Accepted;
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                // Ignore any errors if the queue is being deleted or not found
                return true;
            }
        }

        /// <summary>
        /// Restores a previously deleted the blob
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <returns>True, upon successful restoration, otherwise,false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty.</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobdownloadinfo?view=azure-dotnet for info on BlobDownloadInfo</remarks>
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

        /// <summary>
        /// Downloads a blob from the service, including its metadata and properties.
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <returns>A BlobDownloadInfo object</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty.</exception>
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

        /// <summary>
        /// Downloads the blob to the specified stream
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="destinationStream">The stream to send the blob to</param>
        /// <returns>True, if successful,otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="destinationStream"/> is null</exception>
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
                when (ex.Status == (int) HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// Downloads the blob to the specified file
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="path">The full filename to send the blob to</param>
        /// <returns>True, if successful,otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="path"/>is null or empty</exception>
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
                when (ex.Status == (int) HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates if the blob exists
        /// </summary>
        /// <param name="blobName">The blob name</param>
        /// <returns>True if the blob exists, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty</exception>
        public async Task<bool> ExistsAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName), "The blob name cannot be null or empty");
            }

            var blobClient = BlobContainerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }

        /// <summary>
        /// Uploads the stream to the container
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="sourceStream">The source stream</param>
        /// <param name="overwriteIfExists">Indicates if the blob should be overwritten if it exists.</param>
        /// <returns>The detail around the blob</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="sourceStream"/> is null</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        public async Task<BlobContentInfo> UploadAsync(string blobName, Stream sourceStream,
            bool overwriteIfExists = false)
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

        /// <summary>
        /// Uploads the file to the container
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="filename">The fully qualified file name</param>
        /// <param name="overwriteIfExists">Indicates if the blob should be overwritten if it exists.</param>
        /// <returns>The detail around the blob</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="filename"/>is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
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

        /// <summary>
        /// Uploads the stream and will overwrite it if it exists
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="sourceStream">The source stream to upload</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="sourceStream"/> is null</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        /// <seealso cref="UploadAsync(string,System.IO.Stream,bool)"/>
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

        /// <summary>
        /// Uploads the stream and will overwrite it if it exists
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="filename">The file to upload</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="filename"/> is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        /// <seealso cref="UploadAsync(string,string,bool)"/>
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