using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JosephGuadagno.AzureHelpers.Storage.Tests.Models;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    public static class BlobTestHelper
    {
        public const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
        
        public static string GetTemporaryName()
        {
            var dateString = DateTime.Now.ToString("s")
                .Replace("/", "-")
                .Replace(":", "-")
                .Replace("T", "-");
            var randomNumber = Guid.NewGuid();
            return $"test-{dateString}-{randomNumber}";
        }

        public static bool ContainerExists(BlobContainerClient blobContainerClient)
        {
            var apiResponse = blobContainerClient.Exists();
            return apiResponse.Value;
        }
        
        public static bool ContainerExists(string containerName)
        {
            var blobContainerClient = new BlobContainerClient(DevelopmentConnectionString, containerName);
            return ContainerExists(blobContainerClient);
        }

        public static bool DeleteContainer(BlobContainerClient blobContainerClient)
        {
            var apiResponse = blobContainerClient.Delete();
            return apiResponse.Status == (int) HttpStatusCode.NoContent;
        }

        public static BlobContainerClient CreateContainer(string containerName, int retainForNumberOfDays = 0)
        {
            var containers = new BlobServiceClient(DevelopmentConnectionString);

            if (retainForNumberOfDays > 0)
            {
                var serviceProperties = new BlobServiceProperties
                {
                    DeleteRetentionPolicy = {Enabled = true, Days = retainForNumberOfDays}
                };
                containers.SetProperties(serviceProperties);
            }
            
            return containers.CreateBlobContainer(containerName);
        }

        public static BlobContentInfo UploadBlob(string containerName, string blobName, Stream sourceStream)
        {
            var blobClient = new BlobClient(DevelopmentConnectionString, containerName, blobName);
            var apiResponse = blobClient.Upload(sourceStream);
            return apiResponse.Value;
        }

        public static bool DeleteBlob(string containerName, string blobName)
        {
            var blobClient = new BlobClient(DevelopmentConnectionString, containerName, blobName);
            var apiResponse = blobClient.Delete();
            return apiResponse.Status == (int)HttpStatusCode.NoContent;
        }

        public static bool DoesBlobExists(string containerName, string blobName)
        {
            var blobClient = new BlobClient(DevelopmentConnectionString, containerName, blobName);
            var apiResponse = blobClient.Exists();
            return apiResponse.Value;
        }

        public static Stream GetTestObjectAsStream()
        {
            var temporaryObject = TestObject.GetSampleObject();
            var temporaryObjectAsJson = JsonSerializer.Serialize(temporaryObject);

            return GenerateStreamFromString(temporaryObjectAsJson);
        }
        
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string GenerateTemporaryFile(string filename)
        {
            var fullyQualifiedFilename = GetTemporaryFile(filename);
            var temporaryObject = TestObject.GetSampleObject();
            var temporaryObjectAsJson = JsonSerializer.Serialize(temporaryObject);
            
            File.WriteAllText(fullyQualifiedFilename, temporaryObjectAsJson);

            return fullyQualifiedFilename;
        }

        public static string GetTemporaryFile(string filename)
        {
            var path = Path.GetTempPath();
            var fullyQualifiedFilename = Path.Combine(path, filename);
            return fullyQualifiedFilename;
        }

        public static void DeleteTemporaryFile(string filename)
        {
            
            File.Delete(filename);
        }
        
        public static bool CompareMemoryStreams(Stream stream, MemoryStream memoryStream)
        {
            if (stream.Length != memoryStream.Length)
                return false;
            stream.Position = 0;
            memoryStream.Position = 0;

            var memoryStreamFromStream = new MemoryStream();
            stream.CopyTo(memoryStreamFromStream);
            var msArray1 = memoryStreamFromStream.ToArray();
            var msArray2 = memoryStream.ToArray();

            return msArray1.SequenceEqual(msArray2);
        }
    }
}