# Blobs

The `Blobs` class provides methods to interact with individuals `Blob` elements.  The easiest way to understand it, is the `Container` class is like a folder on your computer, and the `Blobs` are like your files.

## Authentication

To interact with a container, you must provide the account information to connect with.  The library supports two different authentication methods for getting a reference and both are done through the constructor of the class.

### Storage Account

This constructor supports any of the Azure Storage connection strings.

```cs
var blobs = new Blobs("UseDevelopmentStorage=true", containerName);
```

### Container Client

This overload of the constructor expects a `BlobContainerClient`, which you can get from instantiating an instance of the [Container](../src/Containers.cs).

```cs
var blobs = new Blobs(myContainer);
```

### Microsoft Identity

The account name should be the account name for your storage account. Assuming your application has been set up with the Microsoft Identity, you can leave the `tokenCredential` as `null` because the Microsoft Identity library will handle it.

```cs
var containers = new Blobs(accountName, tokenCredential);
```

You can read more about this approach at [https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities](https://www.josephguadagno.net/2020/08/22/securing-azure-containers-and-blobs-with-managed-identities)

## Interacting with the Blobs

### Deleting a Blob

```cs
var wasDeleted = await blobs.DeleteAsync(blobName);
```

Returns `true` if the blob was deleted (or does not exists), otherwise false.  This method also supports the `DeleteSnapshotsOption` enumeration.  If not specified, it assumes `DeleteSnapshotsOption.IncludeSnapshots`

### Undelete a Blob

```cs
var wasRestored = await blobs.UndeleteAsync(blobName);
```

Returns `true` if the blob was restored, otherwise false.

***NOTE*** The emulators do not support this method and while there are test cases for it, it has not been validated.

### Downloading a Blob

#### Download

Downloads a blob from the service, including its metadata and properties.

```cs
var blobInfo = await blobs.DownloadAsync(blobName);
```

Returns a [`BlobDownloadInfo`](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobdownloadinfo?view=azure-dotnet&WT.mc_id=AZ-MVP-4024623) object.

#### DownloadTo

Downloads the blob to the specified stream.

```cs
var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationStream);
```

Returns a `true` if the blob was successfully downloaded to the stream

Downloads the blob to the specified file. ***Note***: The `destinationFile` should be a fully qualified file.

```cs
var wasDownloaded = await blobs.DownloadToAsync(blobName, destinationFile);
```

Returns a `true` if the blob was successfully downloaded to the File

### Check if the Blob Exists

```cs
var doesExists = await blobs.ExistsAsync(blobName);
```

Returns `true` if the blob exists, otherwise false.

### Uploading a Blob

Uploads the stream to the container.

```cs
var blobContentInfo = await blobs.UploadAsync(blobName, uploadObject, overwriteIfExists);
```

Uploads the files to the container.

```cs
var blobContentInfo = await blobs.UploadAsync(blobName, generatedFile);
```

The `overwriteIfExists` is optional with a default of `false`. If you always want to overwrite the blob, there is a `UploadAndOverwriteIfExistsAsync` method that can be used.

Returns a [`BlobContentInfo`](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.models.blobcontentinfo?view=azure-dotnet&WT.mc_id=AZ-MVP-4024623) object.

## More Examples

You can find more examples in the test cases in [BlobsTests](/tests/CBlobsTests.cs.)