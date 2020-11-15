# Queues

## Authentication

To interact with a container, you must provide the account information to connect with.  The library supports two different authentication methods for getting a reference and both are done through the constructor of the class.

### Storage Account

This constructor supports any of the Azure Storage connection strings.

```cs
var queues = new Blobs("UseDevelopmentStorage=true");
```

### Microsoft Identity

The account name should be the account name for your storage account. Assuming your application has been set up with the Microsoft Identity, you can leave the `tokenCredential` as `null` because the Microsoft Identity library will handle it.

```cs
var containers = new Queues(accountName, tokenCredential);
```

You can read more about this approach at [https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities](https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities)

## Interacting with the Queues

### Get Queue

```cs
var queueClient = queues.GetQueueClient(queueName);
```

Returns a reference to the Queue.

### Create Queue

```cs
var queue = await queues.CreateQueueAsync(queueName);
```

Returns a reference to the Queue.

### Delete Queue

```cs
var result = await queues.DeleteQueueAsync(queueName);
```

Return `true` if the queue was deleted, otherwise, `false`.

### Get All Queues

```cs
var queueList = await queues.GetQueuesAsync();
```

Return a List<> of Queues.

## More Examples

You can find more examples in the test cases in [QueuesTests](/tests/QueuesTests.cs.)