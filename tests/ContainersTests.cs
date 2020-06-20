using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    // TODO: Verify Code Coverage
    public class ContainersTests
    {
        // Constructor
        [Fact]
        private void Constructor_WithNullStorageConnectionString_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Containers(null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("storageConnectionString", exception.ParamName);
            Assert.StartsWith("The storage connection string cannot be null or empty", exception.Message);
        }
        
        [Fact]
        private void Constructor_WithValidConnectionString_ShouldReturnHaveAValidBlobServiceClient()
        {
            // Arrange
            
            // Act
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            
            // Assert
            Assert.True(true);
            Assert.NotNull(containers);
            Assert.NotNull(containers.BlobServiceClient);
        }
        
        // CreateContainerAsync
        [Fact]
        private async Task CreateContainerAsync_WithNullContainerName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            
            // Act
            async Task<BlobContainerClient> Action() => await containers.CreateContainerAsync(null);
            
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("containerName", exception.ParamName);
            Assert.StartsWith("The container name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        private async Task CreateContainerAsync_WithValidContainerName_ShouldReturnBlobContainerClient()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var containerName = BlobTestHelper.GetTemporaryName();
            
            // Act
            var container = await containers.CreateContainerAsync(containerName);
            
            // Assert
            Assert.NotNull(container);
            Assert.Equal(containerName, container.Name);
            Assert.True(BlobTestHelper.ContainerExists(container));
            
            // Clean up
            BlobTestHelper.DeleteContainer(container);
        }
        
        [Fact]
        private async Task CreateContainerAsync_WithContainerNameThatExists_ShouldReturnBlobContainerClient()
        {
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var containerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(containerName);
            
            // Act
            var container = await containers.CreateContainerAsync(containerName);
            
            // Assert
            Assert.NotNull(container);
            Assert.Equal(containerName, container.Name);
            Assert.True(BlobTestHelper.ContainerExists(container));
            
            // Clean up
            BlobTestHelper.DeleteContainer(temporaryContainer);
        }
        
        // DeleteContainerAsync
        [Fact]
        private async Task DeleteContainerAsync_WithNullContainerName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            
            // Act
            async Task Action() => await containers.DeleteContainerAsync(null);
            
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
            Assert.Equal("containerName", exception.ParamName);
            Assert.StartsWith("The container name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        private async Task DeleteContainerAsync_WithValidContainerNameThatExists_ShouldReturnTrue()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var containerName = BlobTestHelper.GetTemporaryName();
            var temporaryContainer = BlobTestHelper.CreateContainer(containerName);
            
            // Act
            var wasDeleted = await containers.DeleteContainerAsync(containerName);
            
            // Assert
            Assert.True(wasDeleted);
            Assert.False(BlobTestHelper.ContainerExists(temporaryContainer));

        }
        
        [Fact]
        private async Task DeleteContainerAsync_WithValidContainerNameThatDoesNotExists_ShouldReturnTrue()
        {
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var containerName = BlobTestHelper.GetTemporaryName();
            
            // Act
            var wasDeleted = await containers.DeleteContainerAsync(containerName);
            
            // Assert
            Assert.True(wasDeleted);
            Assert.False(BlobTestHelper.ContainerExists(containerName));
        }
        
        // GetContainer
        [Fact]
        private void GetContainer_WithNullContainerName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            
            // Act
            void Action() => containers.GetContainer(null);
            
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("containerName", exception.ParamName);
            Assert.StartsWith("The container name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        private void GetContainer_WithValidContainerName_ShouldReturnBlobContainerClient()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var containerName = BlobTestHelper.GetTemporaryName();
            
            // Act
            var container = containers.GetContainer(containerName);
            
            // Assert
            Assert.NotNull(container);
            Assert.Equal(containerName, container.Name);
            Assert.EndsWith(containerName, container.Uri.AbsoluteUri);
        }
        
        // GetContainersAsync
        [Fact]
        private async Task GetContainers_WithoutParameters_ShouldReturnAListOfBlobContainerItems()
        {
            // Arrange
            var containers = new Containers(BlobTestHelper.DevelopmentConnectionString);
            var numberOfContainersToCreate = 5;
            var temporaryContainers = new List<BlobContainerClient>();
            for (int i = 0; i < numberOfContainersToCreate; i++)
            {
                var temporaryContainerName = BlobTestHelper.GetTemporaryName();
                temporaryContainers.Add(BlobTestHelper.CreateContainer(temporaryContainerName));
            }
            
            // Act
            var containersList = await containers.GetContainersAsync();
            
            // Assert
            Assert.NotNull(containersList);
            // Make sure we have at least the number of containers we created for the test
            Assert.True(containersList.Count >= numberOfContainersToCreate);
            // Make sure those containers are available
            foreach (var temporaryContainer in temporaryContainers)
            {
                Assert.True(containersList.Exists(c => c.Name == temporaryContainer.Name));
            }
            
            // Cleanup
            foreach (var containerClient in temporaryContainers)
            {
                BlobTestHelper.DeleteContainer(containerClient);
            }
        }
    }
}