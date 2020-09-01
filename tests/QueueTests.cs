using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using JosephGuadagno.AzureHelpers.Storage.Tests.Models;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
	public class QueueTests
	{
		// TODO: Verify Code Coverage

		// Constructor(string, string)

		[Fact]
		public void Constructor_WithNullStorageConnectionString_ShouldThrowArgumentNullException()
		{
			// Arrange
			
			// Act
			void Action()
			{
				var unused = new Queue(storageConnectionString: null, "notnull");
			}

			// Assert
			var exception = Assert.Throws<ArgumentNullException>(Action);
			Assert.Equal("storageConnectionString", exception.ParamName);
			Assert.StartsWith("The storage connection string parameter can not be null or empty.", exception.Message);
		}

		[Fact]
		public void Constructor_WithNullQueueName_ShouldThrowArgumentNullException()
		{
			// Arrange
			
			// Act
			void Action()
			{
				var unused = new Queue(QueueTestHelper.DevelopmentConnectionString, null);
			}

			// Assert
			var exception = Assert.Throws<ArgumentNullException>(Action);
			Assert.Equal("queueName", exception.ParamName);
			Assert.StartsWith("The queue name can not be null or empty.", exception.Message);
		}

		[Fact]
		public void Constructor_WithInvalidStorageConnectionString_ShouldThrowFormatException()
		{
			// Arrange
			
			// Act
			void Action()
			{
				var unused = new Queue("invalid connection string", "notnull");
			}

			// Assert
			var exception = Assert.Throws<FormatException>(Action);
			Assert.Equal("Settings must be of the form \"name=value\".", exception.Message);
		}

		[Fact]
		public void Constructor_WithValidStorageConnectionStringAndQueueName_ShouldHaveValidQueueClient()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();

			// Act
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);

			// Assert
			Assert.NotNull(queue);
			Assert.NotNull(queue.QueueClient);
			Assert.Equal(queueName, queue.QueueClient.Name);
		}

		// Constructor(QueueServiceClient, string)
		[Fact]
		public void Constructor2_WithNullQueueServiceClient_ShouldThrowArgumentNullException()
		{
			// Arrange
			
			// Act
			void Action()
			{
				var unused = new Queue(queueServiceClient: null, "notnull");
			}

			// Assert
			var exception = Assert.Throws<ArgumentNullException>(Action);
			Assert.Equal("queueServiceClient", exception.ParamName);
			Assert.StartsWith("The queue service client can not be null.", exception.Message);

		}
		
		[Fact]
		public void Constructor2_WithNullQueueName_ShouldThrowArgumentNullException()
		{
			// Arrange
			var queueServiceClient = new QueueServiceClient(QueueTestHelper.DevelopmentConnectionString);
			
			// Act
			void Action()
			{
				var unused = new Queue(queueServiceClient, null);
			}

			// Assert
			var exception = Assert.Throws<ArgumentNullException>(Action);
			Assert.Equal("queueName", exception.ParamName);
			Assert.StartsWith("The queue name can not be null or empty.", exception.Message);

		}
		
		[Fact]
		public void Constructor2_WithValidQueueServiceClientAndQueueName_ShouldHaveValidQueueClient()
		{
			// Arrange
			var queueServiceClient = new QueueServiceClient(QueueTestHelper.DevelopmentConnectionString);
			var queueName = QueueTestHelper.GetTemporaryQueueName();

			// Act
			var queue = new Queue(queueServiceClient, queueName);

			// Assert
			Assert.NotNull(queue);
			Assert.NotNull(queue.QueueClient);
			Assert.Equal(queueName, queue.QueueClient.Name);
		}
		
		// AddMessageAsync
		[Fact]
		public async Task AddMessageAsync_WithNullMessage_ShouldThrowArgumentNullException()
		{
			// Arrange
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, "DummyName");
			
			// Act
			async Task<SendReceipt> Action() => await queue.AddMessageAsync<string>(null);
			
			// Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(Action);
			Assert.Equal("message", exception.ParamName);
			Assert.StartsWith("The message cannot be null.", exception.Message);
		}
		
		[Fact]
		public async Task AddMessageAsync_WithMessage_ShouldPostMessageAndReturnSendReceipt()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var testMessage = TestObject.GetSampleObject();
			
			// Act
			var sendReceipt = await queue.AddMessageAsync(testMessage);

			// Assert
			Assert.NotNull(sendReceipt);
			Assert.True(sendReceipt.ExpirationTime >= DateTimeOffset.Now);
			// See if the message is actually there
			var messages = QueueTestHelper.PeekMessages(queueName);
			Assert.NotNull(messages.First(m => m.MessageId == sendReceipt.MessageId));

			// Cleanup
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
		
		[Fact]
		public async Task AddMessageAsync_WithMessage_ShouldPostMessageAndReceivedSameMessage()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var testMessage = TestObject.GetSampleObject();
			
			// Act
			var sendReceipt = await queue.AddMessageAsync(testMessage);

			// Assert
			Assert.NotNull(sendReceipt);
			Assert.True(sendReceipt.ExpirationTime >= DateTimeOffset.Now);
			// See if the message is actually there
			var messages = QueueTestHelper.PeekMessages(queueName);
			var message = messages.First(m => m.MessageId == sendReceipt.MessageId);
			Assert.NotNull(message);
			// Compare the contents of the TestMessage to the Message
			var retrievedMessage = JsonSerializer.Deserialize<TestObject>(message.MessageText);
			Assert.NotNull(retrievedMessage);
			Assert.Equal(testMessage.StringProperty, retrievedMessage.StringProperty);
			Assert.Equal(testMessage.RandomNumber, retrievedMessage.RandomNumber);

			// Cleanup
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
		
		// GetMessagesAsync
		[Fact]
		public async Task GetMessagesAsync_WithNoNumberOfMessages_ShouldReturnAListWithOneMessage()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var testMessage = TestObject.GetSampleObject();
			QueueTestHelper.AddMessage(temporaryQueue, testMessage);

			// Act
			var messages = await queue.GetMessagesAsync<TestObject>();

			// Assert
			Assert.NotNull(messages);
			Assert.Single(messages);
			
			// Clean up
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
		
		[Fact]
		public async Task GetMessagesAsync_WithANegativeNumberOfMessages_ShouldThrowArgumentOutOfRangeException()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var numberOfMessages = -1;
			
			// Act
			async Task<List<TestObject>> Action() => await queue.GetMessagesAsync<TestObject>(numberOfMessages);
			
			// Assert
			var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
			Assert.Equal("numberOfMessages", exception.ParamName);
			Assert.StartsWith("The number of messages to receive must be greater than 0 and less than 33", exception.Message);
			Assert.Equal(numberOfMessages, exception.ActualValue);
		}
		
		[Fact]
		public async Task GetMessagesAsync_WithAnyNumberGreaterThen32_ShouldThrowArgumentOutOfRangeException()
		{
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var numberOfMessages = 33;
			
			// Act
			async Task<List<TestObject>> Action() => await queue.GetMessagesAsync<TestObject>(numberOfMessages);
			
			// Assert
			var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
			Assert.Equal("numberOfMessages", exception.ParamName);
			Assert.StartsWith("The number of messages to receive must be greater than 0 and less than 33", exception.Message);
			Assert.Equal(numberOfMessages, exception.ActualValue);
		}
		
		[Fact]
		public async Task GetMessagesAsync_WithAnyNumberGreaterThanZeroAndLessThan33_ShouldReturnAListOfMessages()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var messageCount = 5;
			for (int i = 0; i < messageCount; i++)
			{
				var testMessage = TestObject.GetSampleObject();
				QueueTestHelper.AddMessage(temporaryQueue, testMessage);	
			}
			
			// Act
			var messagesReceived = await queue.GetMessagesAsync<TestObject>(messageCount);
			
			// Assert
			Assert.NotNull(messagesReceived);
			Assert.True(messagesReceived.Count == messageCount);
			
			// Cleanup
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
		
		// ClearMessagesAsync && DeleteAllMessagesAsync
		// DeleteAllMessagesAsync is just another method for ClearMessages
		[Fact]
		public async Task ClearMessagesAsync_WithNoParameters_ShouldRemoveAllMessagesAndReturnTrue()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var messageCount = 5;
			for (int i = 0; i < messageCount; i++)
			{
				QueueTestHelper.AddMessage(temporaryQueue, TestObject.GetSampleObject());	
			}
			
			// Act
			var wasCleared = await queue.ClearMessagesAsync();
			
			// Assert
			Assert.True(wasCleared);
			// Make sure there are no messages
			var messages = QueueTestHelper.PeekMessages(queueName, messageCount);
			Assert.True(messages.Length == 0);
			
			// Clean up
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
		
		[Fact]
		public async Task DeleteAllMessagesAsync_WithNoParameters_ShouldRemoveAllMessagesAndReturnTrue()
		{
			// Arrange
			var queueName = QueueTestHelper.GetTemporaryQueueName();
			var temporaryQueue = QueueTestHelper.CreateQueue(queueName);
			var queue = new Queue(QueueTestHelper.DevelopmentConnectionString, queueName);
			var messageCount = 5;
			for (int i = 0; i < messageCount; i++)
			{
				QueueTestHelper.AddMessage(temporaryQueue, TestObject.GetSampleObject());	
			}
			
			// Act
			var wasCleared = await queue.DeleteAllMessagesAsync();
			
			// Assert
			Assert.True(wasCleared);
			// Make sure there are no messages
			var messages = QueueTestHelper.PeekMessages(queueName, messageCount);
			Assert.True(messages.Length == 0);
			
			// Clean up
			QueueTestHelper.DeleteQueue(temporaryQueue);
		}
	}
}