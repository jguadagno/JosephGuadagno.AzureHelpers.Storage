using System;

namespace JosephGuadagno.AzureHelpers.Storage.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountName = "cwjgContacts";
            var containerName = "contact-images";
            
            var blobs = new Blobs(accountName, null, containerName);

            var fileWasDownload = blobs.DownloadToAsync("headshot1.jpg", "c:\\Downloads\\headshot0825-1.jpg").Result;
            Console.WriteLine($"File was downloaded = {fileWasDownload}");
            Console.ReadKey();
        }
    }
}