using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace LKcosmetics_CLDV6212_POE.Services
{
    public class FileService
    {
        private readonly ShareClient _shareClient;

        public FileService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UploadFileAsync(string directoryName, string fileName, Stream fileStream)
        {
            var directory = _shareClient.GetDirectoryClient(directoryName);
            await directory.CreateIfNotExistsAsync();

            var file = directory.GetFileClient(fileName);

            
            await file.DeleteIfExistsAsync();

            
            await file.CreateAsync(fileStream.Length);
            await file.UploadRangeAsync(
                new HttpRange(0, fileStream.Length),
                fileStream
            );
        }

        public async Task<Stream> DownloadFileAsync(string directoryName, string fileName)
        {
            var file = _shareClient.GetDirectoryClient(directoryName).GetFileClient(fileName);
            var download = await file.DownloadAsync();
            return download.Value.Content;
        }

        public async Task DeleteFileAsync(string directoryName, string fileName)
        {
            var file = _shareClient.GetDirectoryClient(directoryName).GetFileClient(fileName);
            await file.DeleteIfExistsAsync();
        }
    }
}
