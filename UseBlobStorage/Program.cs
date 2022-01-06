using Microsoft.Azure.Storage.Blob;
using System;

namespace UseBlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting System properties Demo!");

            AppSettings appSettings = AppSettings.LoadAppSettings();

            CloudBlobClient blobClient = Common.CreateBlobClientStorageFromSAS(appSettings.SASToken, appSettings.AccountName);

            CloudBlobContainer container = blobClient.GetContainerReference(appSettings.ContainerName);

            container.CreateIfNotExists();

            container.FetchAttributes();

            Console.WriteLine($"Properties for container {container.StorageUri.PrimaryUri.ToString()}");
            Console.WriteLine($"ETag: {container.Properties.ETag}");
            Console.WriteLine($"LastModifiedUTC: {container.Properties.LastModified.ToString()}");
            Console.WriteLine($"Lease status: {container.Properties.LeaseStatus.ToString()}");
            Console.WriteLine();

            container.Metadata.Add("department", "Technical");
            container.Metadata["category"] = "Knowledge Base";
            container.Metadata.Add("docType", "pdfDocuments");

            container.SetMetadata();
            container.FetchAttributes();

            Console.WriteLine("Container's metadata:");
            foreach (var item in container.Metadata)
            {
                Console.Write($"\tKey: {item.Key}\t");
                Console.WriteLine($"\tValue: {item.Value}");
            }
        }
    }
}
