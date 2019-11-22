using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Foundatio.Storage;

namespace DigitalPurchasing.Services
{
    public interface IObjectStorageService
    {
        Task<bool> ExistsAsync(string path);
        Task<bool> SaveFileAsync(string path, Stream stream, CancellationToken token = default);
        Task<Stream> GetFileStreamAsync(string path, CancellationToken token = default);
        Task<bool> DeleteFileAsync(string path, CancellationToken token = default);
    }

    public class ObjectStorageService : IObjectStorageService
    {
        private readonly IFileStorage _fileStorage;

        public ObjectStorageService(string bucket, string accessKey, string secretKey)
            => _fileStorage = new S3FileStorage(new S3FileStorageOptions
            {
                Bucket = bucket,
                ServiceUrl = "https://storage.yandexcloud.net",
                Credentials = new BasicAWSCredentials(accessKey, secretKey)
            });

        public Task<bool> ExistsAsync(string path)
            => _fileStorage.ExistsAsync(path);

        public Task<bool> SaveFileAsync(string path, Stream stream, CancellationToken token = default)
            => _fileStorage.SaveFileAsync(path, stream, token);

        public Task<Stream> GetFileStreamAsync(string path, CancellationToken token = default)
            => _fileStorage.GetFileStreamAsync(path, token);

        public Task<bool> DeleteFileAsync(string path, CancellationToken token = default)
            => _fileStorage.DeleteFileAsync(path, token);
    }
}
