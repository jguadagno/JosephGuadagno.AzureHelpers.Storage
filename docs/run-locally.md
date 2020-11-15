# Running the Solution Locally

To run the unit tests locally you will need an Azure Storage emulator.  The library works with both the [Azure Storage emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?WT.mc_id=AZ-MVP-4024623) and the open-source [Azurite Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?WT.mc_id=AZ-MVP-4024623).

If you are needing to choose between either, go with Azurite.  The Azure Storage Emulator is being deprecated in favor of Azurite.  If you are using a non-Windows device for development, then Azurite is your only option.

I'm currently using Azurite in a [Docker container](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&WT.mc_id=AZ-MVP-4024623#install-and-run-the-azurite-docker-image) and hope to eventually use a container to run the unit tests :smile:.
