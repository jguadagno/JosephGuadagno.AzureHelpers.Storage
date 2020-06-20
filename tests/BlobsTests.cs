using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    // TODO: Verify Code Coverage
    public class BlobsTests
    {
        // Constructor1
        //     string, string
        [Fact]
        public void Constructor1_WithNullStorageConnectionString_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Blobs(null, "containerName");
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("storageConnectionString", exception.ParamName);
            Assert.StartsWith("The storage connection string cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Constructor1_WithNullContainerName_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Blobs(BlobTestHelper.DevelopmentConnectionString, null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("containerName", exception.ParamName);
            Assert.StartsWith("The container name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        public void Constructor1_WithValidStorageConnectionStringAndContainerName_ShouldNotThrowException()
        {
            // Arrange
            
            // Act
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Assert
            Assert.True(true);
            Assert.NotNull(blobs);
            Assert.NotNull(blobs.BlobContainerClient);
        }
        
        // Constructor2
        //    blobContainerClient
        [Fact]
        public void Constructor2_WithNullBlobContainerClient_ShouldThrowArgumentNullException()
        {
            // Arrange

            // Act
            void Action()
            {
                var unused = new Blobs(null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("blobContainerClient", exception.ParamName);
            Assert.StartsWith("The blob container client cannot be null", exception.Message);
        }

        [Fact]
        public void Constructor2_WithNotNullBlobContainerClient_ShouldNotThrowException()
        {
            // Arrange
            var blobContainerClient = new BlobContainerClient(BlobTestHelper.DevelopmentConnectionString, "containerName");
            
            // Act
            var blobs = new Blobs(blobContainerClient);

            // Assert
            Assert.True(true);
            Assert.NotNull(blobs);
            Assert.NotNull(blobs.BlobContainerClient);
        }

        // DeleteAsync
        [Fact]
        public async Task DeleteAsync_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.DeleteAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message   );
        }

        [Fact]
        public async Task DeleteAsync_WithValidateBlobNameThatExists_ShouldDeleteBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);
            
            // Act
            var wasDeleted = await blobs.DeleteAsync(blobName);

            // Assert
            Assert.True(wasDeleted);
            
            // Cleanup
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        [Fact]
        public async Task DeleteAsync_WithValidateBlobNameThatDoesNotExists_ShouldReturnTrue()
        {
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            
            // Act
            var wasDeleted = await blobs.DeleteAsync(blobName);

            // Assert
            Assert.True(wasDeleted);
            
            // Cleanup
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // UndeleteAsync
        [Fact]
        public async Task UndeleteAsync_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.UndeleteAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message   );

        }
        [Fact(Skip = "The emulators do not support Undelete or Setting up retention policies")]
        public async Task UndeleteAsync_WithValidBlobName_ShouldUndeleteBlobAndReturnTrue()
        {
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName, 7);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);
            BlobTestHelper.DeleteBlob(blobContainerName, blobName);
            
            // Act
            var wasRestored = await blobs.UndeleteAsync(blobName);

            // Assert
            Assert.True(wasRestored);
            
            // Cleanup
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        [Fact]
        public async Task UndeleteAsync_WithAndInvalidBlobName_ShouldReturnFalse()
        {
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            
            // Act
            var wasRestored = await blobs.UndeleteAsync(blobName);

            // Assert
            Assert.False(wasRestored);
            
            // Cleanup
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // GetBlobAsync
        [Fact]
        public async Task DownloadAsync_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.DownloadAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        public async Task DownloadAsync_WithValidBlobName_ShouldReturnBlobInfo()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);

            // Act
            var blobInfo = await blobs.DownloadAsync(blobName);

            // Assert
            Assert.NotNull(blobInfo);
            Assert.NotNull(blobInfo.Content);
            Assert.NotNull(blobInfo.Details);
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        [Fact]
        public async Task DownloadAsync_WithANonExistingBlobName_ShouldThrowRequestFailedException()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";

            // Act
            async Task Action() => await blobs.DownloadAsync(blobName);
            
            // Assert
            var exception = await Assert.ThrowsAsync<RequestFailedException>(Action);
            Assert.Equal(BlobErrorCode.BlobNotFound, exception.ErrorCode);
            Assert.StartsWith("The specified blob does not exist.", exception.Message);
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // DownloadToAsync1
        //    string, Stream
        [Fact]
        public async Task DownloadToAsync1_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var destinationStream = new MemoryStream();
            
            // Act
            async Task Action() => await blobs.DownloadToAsync(null, destinationStream);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task DownloadToAsync1_WithNullDestinationStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.DownloadToAsync(BlobTestHelper.DevelopmentConnectionString, destinationStream:null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("destinationStream", exception.ParamName);
            Assert.StartsWith("The stream can not be null", exception.Message);
        }

        [Fact]
        public async Task DownloadToAsync1_WithValidBlobNameAndStream_ShouldDownloadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);
            var destinationStream = new MemoryStream();

            // Act
            var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationStream);

            // Assert
            Assert.True(wasDownloaded);
            Assert.True(BlobTestHelper.CompareMemoryStreams(uploadObject, destinationStream));

            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        [Fact]
        public async Task DownloadToAsync1_WithValidBlobNameAndStreamButBlobDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var destinationStream = new MemoryStream();

            // Act
            var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationStream);

            // Assert
            Assert.False(wasDownloaded);
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // TODO: What happens if the blob does not exists, when downloadTo is called
        
        // DownloadToAsync2
        //    string, string
        [Fact]
        public async Task DownloadToAsync2_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var fileName = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.DownloadToAsync(null, fileName);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);

        }

        [Fact]
        public async Task DownloadToAsync2_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var blobName = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.DownloadToAsync(blobName, path:null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("path", exception.ParamName);
            Assert.StartsWith("The path cannot be null or empty", exception.Message);
        }
        
        [Fact]
        public async Task DownloadToAsync2_WithValidBlobNameAndFilenameButBlobDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var destinationFile = BlobTestHelper.GetTemporaryFile(blobName);

            // Act
            var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationFile);

            // Assert
            Assert.False(wasDownloaded);
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }

        [Fact]
        public async Task DownloadToAsync2_WithValidBlobNameAndStream_ShouldDownloadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);
            
            var destinationFile = BlobTestHelper.GetTemporaryFile(blobName);

            // Act
            var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationFile);

            // Assert
            Assert.True(wasDownloaded);
            Assert.True(File.Exists(destinationFile));
            
            // Clean up
            BlobTestHelper.DeleteTemporaryFile(destinationFile);
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // ExistsAsync
        [Fact]
        public async Task ExistsAsync_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.ExistsAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);

        }

        [Fact]
        public async Task ExistsAsync_WithValidBlobNameThatExists_ShouldReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, uploadObject);
            
            // Act
            var doesExists = await blobs.ExistsAsync(blobName);

            // Assert
            Assert.True(doesExists);
            
            // Cleanup 
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }

        [Fact]
        public async Task ExistsAsync_WithValidBlobNameThatDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            
            // Act
            var doesExists = await blobs.ExistsAsync(blobName);

            // Assert
            Assert.False(doesExists);
            
            // Cleanup 
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // UploadAsync1
        //    string, Stream
        [Fact]
        public async Task UploadAsync1_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var destinationStream = new MemoryStream();
            
            // Act
            async Task Action() => await blobs.UploadAsync(null, destinationStream);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);

        }

        [Fact]
        public async Task UploadAsync1_WithNullDestinationStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() => await blobs.UploadAsync(BlobTestHelper.DevelopmentConnectionString, sourceStream:null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("sourceStream", exception.ParamName);
            Assert.StartsWith("The source stream can not be null", exception.Message);

        }

        [Fact]
        public async Task UploadAsync1_WithValidBlobNameAndStream_ShouldUploadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var uploadObject = BlobTestHelper.GetTestObjectAsStream();
            
            // Act
            var blobContentInfo = await blobs.UploadAsync(blobName, uploadObject);
            
            // Assert
            Assert.NotNull(blobContentInfo);
            Assert.True(BlobTestHelper.DoesBlobExists(blobContainerName, blobName));
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }

        // UploadAsync2
        //    string, string
        [Fact]
        public async Task UploadAsync2_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var sourceFile = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(null, sourceFile);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task UploadAsync2_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");

            // Act
            async Task Action() =>
                await blobs.UploadAsync(BlobTestHelper.DevelopmentConnectionString, filename: null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("filename", exception.ParamName);
            Assert.StartsWith("The filename cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task UploadAsync2_WithValidBlobNameAndStreamButFileDoesNotExists_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var temporaryBlobName = BlobTestHelper.GetTemporaryName();
            var fileToUpload = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() =>
                await blobs.UploadAsync(temporaryBlobName, fileToUpload);

            // Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(Action);
            Assert.Equal(fileToUpload, exception.FileName);
            Assert.StartsWith("The file to upload was not found", exception.Message);
        }

        [Fact]
        public async Task UploadAsync2_WithValidBlobNameAndFileName_ShouldUploadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var generatedFile = BlobTestHelper.GenerateTemporaryFile(blobName);
            
            // Act
            var blobContentInfo = await blobs.UploadAsync(blobName, generatedFile);
            
            // Assert
            Assert.NotNull(blobContentInfo);
            Assert.True(BlobTestHelper.DoesBlobExists(blobContainerName, blobName));
            
            // Clean up
            BlobTestHelper.DeleteTemporaryFile(generatedFile);
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // UploadAndOverwriteIfExistsAsync1
        //    string, Stream
        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync1_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var destinationStream = new MemoryStream();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(null, destinationStream);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync1_WithNullDestinationStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var blobName = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(blobName, sourceStream:null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("sourceStream", exception.ParamName);
            Assert.StartsWith("The source stream can not be null", exception.Message   );

        }

        [Fact] public async Task UploadAndOverwriteIfExistsAsync1_WithValidBlobNameAndStream_ShouldUploadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var temporaryObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, temporaryObject);
            var updatedObject = BlobTestHelper.GetTestObjectAsStream();
            
            // Act
            var blobContentInfo = await blobs.UploadAndOverwriteIfExistsAsync(blobName, updatedObject);
            
            // Assert
            Assert.NotNull(blobContentInfo);
            Assert.True(BlobTestHelper.DoesBlobExists(blobContainerName, blobName));
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // UploadAndOverwriteIfExistsAsync2
        //    string, string
        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync2_WithNullBlobName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var sourceFile = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(null, sourceFile);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("blobName", exception.ParamName);
            Assert.StartsWith("The blob name cannot be null or empty", exception.Message   );

        }

        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync2_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var blobName = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(blobName, filename:null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("filename", exception.ParamName);
            Assert.StartsWith("The filename cannot be null or empty", exception.Message   );

        }
        
        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync2_WithValidBlobNameAndStreamButFileDoesNotExists_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, "containerName");
            var blobName = BlobTestHelper.GetTemporaryName();
            var fileToUpload = BlobTestHelper.GetTemporaryName();
            
            // Act
            async Task Action() => await blobs.UploadAndOverwriteIfExistsAsync(blobName, fileToUpload);

            // Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(Action);
            Assert.Equal(fileToUpload, exception.FileName);
            Assert.StartsWith("The file to upload was not found", exception.Message   );

        }

        [Fact]
        public async Task UploadAndOverwriteIfExistsAsync2_WithValidBlobNameAndStreamFile_ShouldUploadTheBlobAndReturnTrue()
        {
            // Arrange
            var blobContainerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(blobContainerName);
            var blobs = new Blobs(BlobTestHelper.DevelopmentConnectionString, blobContainerName);
            var blobName = BlobTestHelper.GetTemporaryName() + ".json";
            var temporaryObject = BlobTestHelper.GetTestObjectAsStream();
            BlobTestHelper.UploadBlob(blobContainerName, blobName, temporaryObject);
            var generatedFile = BlobTestHelper.GenerateTemporaryFile(blobName);
            
            // Act
            var blobContentInfo = await blobs.UploadAndOverwriteIfExistsAsync(blobName, generatedFile);
            
            // Assert
            Assert.NotNull(blobContentInfo);
            Assert.True(BlobTestHelper.DoesBlobExists(blobContainerName, blobName));
            
            // Clean up
            BlobTestHelper.DeleteTemporaryFile(generatedFile);
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
    }
}