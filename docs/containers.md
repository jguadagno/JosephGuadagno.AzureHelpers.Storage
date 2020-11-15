# Containers

A container organizes a set of blobs, similar to a directory in a file system. A storage account can include an unlimited number of containers, and a container can store an unlimited number of blobs.

Most methods are both synchronous and asynchronous.

## Authentication

To interact with a container, you must provide the account information to connect with.  The library supports two different authentication methods for getting a reference and both are done through the constructor of the class.

### Storage Account

This constructor supports any of the Azure Storage connection strings.

```cs
var containers = new Containers("UseDevelopmentStorage=true");
```

### Microsoft Identity

The account name should be the account name for your storage account. Assuming your application has been set up with the Microsoft Identity, you can leave the `tokenCredential` as `null` because the Microsoft Identity library will handle it.

```cs
var containers = new Containers(accountName, tokenCredential);
```

You can read more about this approach at [https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities](https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities)

## Interacting with the Containers

### Create a Container

This method returns a reference to the container

```cs
var containers = new Containers("UseDevelopmentStorage=true");
var container = await containers.CreateContainerAsync(containerName);
```

### Delete a Container

This method returns a true if the container specified in `containerName` was deleted, otherwise false.

```cs
var containers = new Containers("UseDevelopmentStorage=true");
var wasDeleted = await containers.DeleteContainerAsync(containerName);
```

### Get a container

```cs
var containers = new Containers("UseDevelopmentStorage=true");
var container = await containers.GetContainerAsync(containerName);
```

## More Examples

You can find more examples in the test cases in [ContainersTests](/tests/ContainersTests.cs.)