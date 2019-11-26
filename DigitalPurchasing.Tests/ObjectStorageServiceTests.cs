using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Services;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class ObjectStorageServiceTests
    {
        private const string Bucket = "dp-tests";
        private const string AccessKey = "LzdY5OR6r8vAYBr5qGud"; // web-tests
        private const string SecretKey = "2qhEea2bAg63td-p4xeORP8GH5sbT7bNGW-yFGSj";

        private readonly ObjectStorageService _objectStorageService;

        public ObjectStorageServiceTests()
            => _objectStorageService = new ObjectStorageService(Bucket, AccessKey, SecretKey);

        [Fact]
        public async Task ExistsAsyncTest()
        {
            var exist = await _objectStorageService.ExistsAsync("abc/abc.txt");
            Assert.False(exist);
        }

        [Fact]
        public async Task SaveGetAndDelete()
        {
            var path = $"{Guid.NewGuid():N}.txt";
            var text = "5631d3c9490244fb9a7453ec848274d7";

            var encoding = Encoding.UTF8;
            var writeStream = new MemoryStream(encoding.GetBytes(text));

            var saveResult = await _objectStorageService.SaveFileAsync(path, writeStream);
            Assert.True(saveResult);

            var readStream = await _objectStorageService.GetFileStreamAsync(path);
            var reader = new StreamReader(readStream);
            var readText = reader.ReadToEnd();

            Assert.Equal(text, readText);

            await _objectStorageService.DeleteFileAsync(path);
        }
    }
}
