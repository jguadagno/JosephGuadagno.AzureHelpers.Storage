# JosephGuadagno.AzureHelpers.Storage

[![Build Status](https://jguadagno.visualstudio.com/JosephGuadagno.Utilities/_apis/build/status/jguadagno.JosephGuadagno.AzureHelpers.Storage?branchName=main)](https://jguadagno.visualstudio.com/JosephGuadagno.Utilities/_build/latest?definitionId=8&branchName=main) ![nuget](https://img.shields.io/nuget/dt/JosephGuadagno.AzureHelpers.Storage)

A collection of classes to aid in development with Azure Storage [Queues](https://docs.microsoft.com/en-us/azure/storage/queues/?WT.mc_id=AZ-MVP-4024623) and [Blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction?WT.mc_id=AZ-MVP-4024623). This package does not replace the need for the Azure Storage SDK but augment it be providing some wrappers around existing methods.

I tried to document the common use cased of the library.  If you want to see how to us an individual method, check out the [tests](/tests/).

If you are in the need of using Azure Table Storage, check out [JosephGuadagno.AzureHelpers.Cosmos](https://www.github.com/jguadagno/JosephGuadagno.AzureHelpers.Cosmos).

## Blob Storage

The Blob Storage components come in two classes, containers, and blobs.

### Working with Containers

Just as in the Azure Storage SDK, the blobs are inside a container. The [JosephGuadagno.AzureHelpers.Storage.Containers](src/Containers.cs) class contains the methods for interacting with Azure Containers.

[Documentation](docs/containers.md)

### Working with Blobs

Provides methods to interact with Blobs in Azure storage.

[Documentation](docs/blobs.md)

## Queue Storage

The Queue Storage helpers come in two classes, the Queues, and the Queue.

### Working with Queues

[Queues](src/Queues.cs) contains methods around working with the queues in Azure Storage, creating, deleting, listing, etc.

[Documentation](docs/queues.md)

### Working with a Queue

[Queue](src/Queue.cs) contains methods around interacting with an individual queue, like adding messages, peeking messages, etc.

[Documentation](docs/queue.md)

## Running Locally

If you would like to, you can find the instructions on running the solution locally [here](docs/run-locally.md).
