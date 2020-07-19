using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Interfaces
{
    public interface IBlobs
    {
        /// <summary>
        /// A reference to the current Blob Container being used
        /// </summary>
        /// <remarks>You can use this property for a most of the methods in the <see cref="Containers"/> class</remarks>
        BlobContainerClient BlobContainerClient { get; }

        /// <summary>
        /// Deletes a blob
        /// </summary>
        /// <param name="blobName">The name of the blob to delete</param>
        /// <param name="deleteSnapshotOption">The set of options describing delete operation.</param>
        /// <returns>True, if the blob was deleted or did not exist, otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> parameter is null or empty.</exception>
        Task<bool> DeleteAsync(string blobName,
            DeleteSnapshotsOption deleteSnapshotOption = DeleteSnapshotsOption.IncludeSnapshots);

        /// <summary>
        /// Restores a previously deleted the blob
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <returns>True, upon successful restoration, otherwise,false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty.</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobdownloadinfo?view=azure-dotnet for info on BlobDownloadInfo</remarks>
        Task<bool> UndeleteAsync(string blobName);

        /// <summary>
        /// Downloads a blob from the service, including its metadata and properties.
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <returns>A BlobDownloadInfo object</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty.</exception>
        Task<BlobDownloadInfo> DownloadAsync(string blobName);

        /// <summary>
        /// Downloads the blob to the specified stream
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="destinationStream">The stream to send the blob to</param>
        /// <returns>True, if successful,otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="destinationStream"/> is null</exception>
        Task<bool> DownloadToAsync(string blobName, Stream destinationStream);

        /// <summary>
        /// Downloads the blob to the specified file
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="path">The full filename to send the blob to</param>
        /// <returns>True, if successful,otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="path"/>is null or empty</exception>
        Task<bool> DownloadToAsync(string blobName, string path);

        /// <summary>
        /// Indicates if the blob exists
        /// </summary>
        /// <param name="blobName">The blob name</param>
        /// <returns>True if the blob exists, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty</exception>
        Task<bool> ExistsAsync(string blobName);

        /// <summary>
        /// Uploads the stream to the container
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="sourceStream">The source stream</param>
        /// <param name="overwriteIfExists">Indicates if the blob should be overwritten if it exists.</param>
        /// <returns>The detail around the blob</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="sourceStream"/> is null</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        Task<BlobContentInfo> UploadAsync(string blobName, Stream sourceStream,
            bool overwriteIfExists = false);

        /// <summary>
        /// Uploads the file to the container
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="filename">The fully qualified file name</param>
        /// <param name="overwriteIfExists">Indicates if the blob should be overwritten if it exists.</param>
        /// <returns>The detail around the blob</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="filename"/>is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        Task<BlobContentInfo> UploadAsync(string blobName, string filename, bool overwriteIfExists = false);

        /// <summary>
        /// Uploads the stream and will overwrite it if it exists
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="sourceStream">The source stream to upload</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> is null or empty or the <see cref="sourceStream"/> is null</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        /// <seealso cref="Blobs.UploadAsync(string,System.IO.Stream,bool)"/>
        Task<BlobContentInfo> UploadAndOverwriteIfExistsAsync(string blobName, Stream sourceStream);

        /// <summary>
        /// Uploads the stream and will overwrite it if it exists
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="filename">The file to upload</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="blobName"/> or <see cref="filename"/> is null or empty</exception>
        /// <remarks>See https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet for more details around BlobContentInfo</remarks>
        /// <seealso cref="Blobs.UploadAsync(string,string,bool)"/>
        Task<BlobContentInfo> UploadAndOverwriteIfExistsAsync(string blobName, string filename);
    }
}