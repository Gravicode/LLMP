using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LLMP.Desktop.Services
{
    public class AzureBlobHelper
    {
        public BlobContainerClient containerClient { get; set; }
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
        public AzureBlobHelper()
        {
           
        }
      
        public async Task<bool> Configure(string ConnectionString, string ContainerName)
        {
            try
            {
                this.ConnectionString = ConnectionString;
                this.ContainerName = ContainerName;
                // TODO: Replace <storage-account-name> with your actual storage account name
                var blobServiceClient = new BlobServiceClient(this.ConnectionString);

                // Create the container and return a container client object
                containerClient = blobServiceClient.GetBlobContainerClient(this.ContainerName);

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                return false;
            }
           
        }

        public async Task<bool> DeleteItem(string deleteFilePath)
        {
            try
            {
                BlobClient blobClient = containerClient.GetBlobClient(deleteFilePath);
                var res = await blobClient.DeleteIfExistsAsync();
                return res.Value;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(  ex.ToString());
            }
            return false;
            
        }
            public async Task<byte[]> DownloadItem(string downloadFilePath)
        {
            try
            {
                BlobClient blobClient = containerClient.GetBlobClient(downloadFilePath);
                // Download the blob's contents and save it to a file
                var res =  await blobClient.DownloadToAsync(downloadFilePath);
                return res.Content.ToArray();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
            }

            return default;
        }

        public async Task<List<BlobItem>> GetFiles(string BlobPath)
        {
            var items = new List<BlobItem>();
            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (blobItem.Name.Contains(BlobPath))
                {
                    Console.WriteLine("\t" + blobItem.Name);
                    items.Add(blobItem);
                }
            }
            return items;
            
        }
        public async Task<bool> UploadFiles(byte[] FileBytes,string fileName)
        {
            try
            {
                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

                // Upload data from the local file

                //var ms = new MemoryStream(FileBytes);
                var ext = Path.GetExtension(fileName);
                ext = ext.Replace(".", string.Empty);
                var blobHttpHeader = new BlobHttpHeaders { ContentType = ext == "pdf" ? "application/pdf" : "image/"+ext };
                var content = await blobClient.UploadAsync(BinaryData.FromBytes(FileBytes), new BlobUploadOptions { HttpHeaders = blobHttpHeader });
                
                //Console.WriteLine("Listing blobs...");

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
            }

            return false;

        }
    }
}
