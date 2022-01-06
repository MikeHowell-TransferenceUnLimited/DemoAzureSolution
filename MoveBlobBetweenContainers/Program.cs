using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace MoveBlobBetweenContainers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Copy items between containers demo");
            Task.Run(async () => await StartContainersDemo()).Wait();

           // Console.WriteLine("Move items between storage accounts demo");
           // Task.Run(async () => await StartAccountDemo()).Wait();
        }

        public static async Task StartContainersDemo()
        {
            string sourceBlobFileName = "Testing.zip";
            AppSettings appSettings = AppSettings.LoadAppSettings();

            BlobServiceClient sourceClient = Common.CreateBlobClientStorageFromSAS(appSettings.SourceSASConnectionString);

            var sourceContainerReference = sourceClient.GetBlobContainerClient(appSettings.SourceContainerName);
            var destinationContainerReference = sourceClient.GetBlobContainerClient(appSettings.DestinationContainerName);

            var sourceBlobReference = sourceContainerReference.GetBlobClient(sourceBlobFileName);
            var destinationBlobReference = destinationContainerReference.GetBlobClient(sourceBlobFileName);

            await destinationBlobReference.StartCopyFromUriAsync(sourceBlobReference.Uri);
        }

        public static async Task StartAccountDemo()
        {
            string sourceBlobFilename = "Testing.zip";
            AppSettings appSettings = AppSettings.LoadAppSettings();

            BlobServiceClient sourceClient = Common.CreateBlobClientStorageFromSAS(appSettings.SourceContainerName);
            BlobServiceClient destinationClient = Common.CreateBlobClientStorageFromSAS(appSettings.DestinationContainerName);

            var sourceContainerReference = sourceClient.GetBlobContainerClient(appSettings.SourceContainerName);
            var destinationContainersReference = destinationClient.GetBlobContainerClient(appSettings.DestinationContainerName);

            var sourceBlobReference = sourceContainerReference.GetBlobClient(sourceBlobFilename);
            var destinationBlobReference = destinationContainersReference.GetBlobClient(sourceBlobFilename);

            await destinationBlobReference.StartCopyFromUriAsync(sourceBlobReference.Uri);
            await sourceBlobReference.DeleteAsync();
        }
    }
}
