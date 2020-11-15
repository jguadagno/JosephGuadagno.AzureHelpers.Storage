# Queue

## Authentication

To interact with a container, you must provide the account information to connect with.  The library supports two different authentication methods for getting a reference and both are done through the constructor of the class.

### Storage Account

This constructor supports any of the Azure Storage connection strings.

```cs
var blobs = new Queue("UseDevelopmentStorage=true", queueName);
```

### Container Client

This overload of the constructor expects a `QueueServiceClient`, which you can get from instantiating an instance of the [Queues](../src/Queues.cs).

```cs
var blobs = new Queue(myQueueClient);
```

### Microsoft Identity

The account name should be the account name for your storage account. Assuming your application has been set up with the Microsoft Identity, you can leave the `tokenCredential` as `null` because the Microsoft Identity library will handle it.

```cs
var containers = new Queue(accountName, tokenCredential, queueName);
```

You can read more about this approach at [https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities](https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities)

## Interacting with the Queue

***Note*** The following methods that are available in the Azure Storage SDK are not yet available in this library.

* DeleteMessages(Async)
* PeekMessages(Async)
* UpdateMessage(Async)

### Add a Message

The message can be of any type, it will be serialized to and from the type type specified.

```cs
var sendReceipt = await queue.AddMessageAsync(testMessage);
```

Returns a [`SendReceipt`](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.queues.models.sendreceipt?view=azure-dotnet&WT.mc_id=AZ-MVP-4024623) if successful, otherwise `null`.

### Get a Message

Retrieves up to 32 messages from the queue.

```cs
var messagesReceived = await queue.GetMessagesAsync<objectType>(messageCount);
```

`objectType` can be any serializable class or object.  The `messageCount` needs to be an integer between 1 and 32.

Returns a List<objectType>.

### Deleting Messages

There are two methods to do this, both work the same: `ClearMessagesAsync` and `DeleteAllMessagesAsync`.

```cs
var wasDeleted = await queue.DeleteAllMessagesAsync();
```

Returns a `true` if all of the messages were deleted or cleared, otherwise, `false`.

## More Examples

You can find more examples in the test cases in [QueueTests](/tests/QueueTests.cs.)