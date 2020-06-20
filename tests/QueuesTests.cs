using System;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable ObjectCreationAsStatement

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    // TODO: Verify Code Coverage
    public class QueuesTests
    {
        // Constructor Tests
        [Fact]
        public void Constructor_WithValidConnectionString_ShouldReturnQueueServiceClient()
        {
            // Arrange
            
            // Act
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            
            // Assert
            Assert.NotNull(queues);
            Assert.NotNull(queues.QueueServiceClient);
        }

        [Fact]
        public void Constructor_WithInvalidConnectionString_ThrowsException()
        {
            // Arrange;
            
            // Act
            void Action() => new Queues("This should throw an exception");

            // Assert
            var exception = Assert.Throws<FormatException>(Action);
            Assert.Equal("Settings must be of the form \"name=value\".", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullConnectionString_ThrowsArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action() => new Queues(null);
            
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("storageConnectionString", exception.ParamName);
            Assert.StartsWith("The storage connection string parameter must not be null or empty.", exception.Message);

        }

        // GetQueueClient Tests
        [Fact]
        public void GetQueueClient_WithValidQueueName_ShouldReturnQueueClient()
        {
            // Arrange
            var queueName = QueueTestHelper.GetTemporaryQueueName();
            var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            
            // Act
            var queueClient = queues.GetQueueClient(queueName);
            
            // Assert
            Assert.NotNull(queueClient);
            Assert.Equal(queueName, queueClient.Name);
            
            // Cleanup
            QueueTestHelper.DeleteQueue(temporaryQueue);
        }
        
        [Fact]
        public void GetQueueClient_WithNullQueueName_ThrowsArgumentNullException()
        {
            // Arrange
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            
            // Act
            void Action() => queues.GetQueueClient(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("queueName", exception.ParamName);
            Assert.StartsWith("The queue name cannot be null or empty.", exception.Message);
        }

        // CreateQueueAsync Tests
        [Fact]
        public async Task CreateQueueAsync_WithValidNewQueueName_ShouldReturnQueueClient()
        {
            // Arrange
            var queueName = QueueTestHelper.GetTemporaryQueueName();
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);

            // Act
            var queue = await queues.CreateQueueAsync(queueName);

            // Assert
            Assert.NotNull(queue);
            Assert.Equal(queueName, queue.Name);
            
            // Cleanup
            QueueTestHelper.DeleteQueue(queue);
        }

        [Fact]
        public async Task CreateQueueAsync_WithValidExistingQueueName_ShouldReturnQueueClient()
        {
            // Arrange
            var queueName = QueueTestHelper.GetTemporaryQueueName();
            var testQueue = QueueTestHelper.CreateQueue(queueName);
            
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);

            // Act
            var queue = await queues.CreateQueueAsync(queueName);

            // Assert
            Assert.NotNull(queue);
            Assert.Equal(queueName, queue.Name);
            
            // Cleanup
            QueueTestHelper.DeleteQueue(testQueue);
            
        }

        [Fact]
        public async Task CreateQueueAsync_WithNullQueueName_ThrowsArgumentNullException()
        {
            // Arrange
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            
            // Act
            async Task Action() => await queues.CreateQueueAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);

            Assert.Equal("queueName", exception.ParamName);
            Assert.StartsWith("The queue name cannot be null or empty.", exception.Message);
        }

        // DeleteQueueAsync Tests
        [Fact]
        public async Task DeleteQueueAsync_WithValidExistingQueueName_ShouldReturnTrue()
        {
            // Arrange
            var queueName = QueueTestHelper.GetTemporaryQueueName();
            QueueTestHelper.CreateQueue(queueName);
            
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);

            // Act
            var result = await queues.DeleteQueueAsync(queueName);

            // Assert
            Assert.True(result);
            Assert.False(QueueTestHelper.DoesQueueExist(queueName));
        }

        [Fact]
        public async Task DeleteQueueAsync_WithValidNonExistingQueueName_ShouldReturnTrue()
        {
            // Arrange
            var queueName = QueueTestHelper.GetTemporaryQueueName();
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);

            // Act
            var result = await queues.DeleteQueueAsync(queueName);

            // Assert
            Assert.True(result);
            Assert.False(QueueTestHelper.DoesQueueExist(queueName));
        }

        [Fact]
        public async Task DeleteQueueAsync_WithNullQueueName_ThrowsArgumentNullException()
        {
            // Arrange
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            
            // Act
            async Task Action() => await queues.DeleteQueueAsync(null);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);

            Assert.Equal("queueName", exception.ParamName);
            Assert.StartsWith("The queue name cannot be null or empty.", exception.Message);

        }

        // GetQueuesAsync Tests
        [Fact]
        public async Task GetQueuesAsync_ShouldReturnAListOfQueueItems()
        {
            // Arrange
            var queue1 = QueueTestHelper.CreateQueue(QueueTestHelper.GetTemporaryQueueName());
            var queue2 = QueueTestHelper.CreateQueue(QueueTestHelper.GetTemporaryQueueName());

            // Act
            var queues = new Queues(QueueTestHelper.DevelopmentConnectionString);
            var queueList = await queues.GetQueuesAsync();

            // Assert
            Assert.NotNull(queueList);
            Assert.True(queueList.Count >= 2);

            // Cleanup
            QueueTestHelper.DeleteQueue(queue1);
            QueueTestHelper.DeleteQueue(queue2);
        }
    }
}