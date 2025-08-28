using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace LKcosmetics_CLDV6212_POE.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

           
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var blobClient = _containerClient.GetBlobClient(fileName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString(); 
        }

        public async Task DeleteFileAsync(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl)) return;

            var blobName = Path.GetFileName(new Uri(blobUrl).AbsolutePath);
            await _containerClient.DeleteBlobIfExistsAsync(blobName);
        }

        
        public string GetBlobSasUri(string blobUrl, int expiryMinutes = 60)
        {
            var blobName = Path.GetFileName(new Uri(blobUrl).AbsolutePath);
            var blobClient = _containerClient.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}
