using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Roo.Azure.Configuration.Common.Utilities.Storage;

namespace Roo.Azure.Configuration.UnitTests
{
    public class FilShareServiceTests
    {
        private readonly string storageConnectionString = "storageName";
        private readonly string fileShareName = "fileShareName";
        private readonly string directoryName = "directoryName";
        private readonly string fileName = "fileName";
        private readonly string path = "path/test";

        [Test]
        public void CreateFileShareClient_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            shareClient.Setup(x => x.Name).Returns(fileShareName);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = service.CreateFileShareClient(storageConnectionString, fileShareName);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo(fileShareName));
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public void DirectoryExists_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareDirectoryClient.Setup(x => x.Exists(It.IsAny<CancellationToken>())).Returns(response.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = service.DirectoryExists(shareClient.Object, path);
            var result2 = service.DirectoryExists(fileShareName, path, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task DirectoryExistsAsync_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareDirectoryClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.DirectoryExistsAsync(shareClient.Object, path);
            var result2 = await service.DirectoryExistsAsync(fileShareName, path, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public void CreateDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareDirectoryClient.Setup(x => x.Name).Returns(directoryName);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = service.CreateDirectory(shareClient.Object, path);
            var result2 = service.CreateDirectory(fileShareName, path, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo(directoryName));
                Assert.That(result2, Is.Not.Null);
                Assert.That(result2?.Name, Is.EqualTo(directoryName));
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task CreateDirectoryAsync_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareDirectoryClient.Setup(x => x.Name).Returns(directoryName);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.CreateDirectoryAsync(shareClient.Object, path);
            var result2 = await service.CreateDirectoryAsync(fileShareName, path, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo(directoryName));
                Assert.That(result2, Is.Not.Null);
                Assert.That(result2?.Name, Is.EqualTo(directoryName));
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public void GetAllFilesFromDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            shareFileClient.Setup(x => x.Name).Returns(fileName);
            var shareFileItem = FilesModelFactory.ShareFileItem(false, fileName);
            var response = new Mock<Response>();
            var page = Page<ShareFileItem>.FromValues(new List<ShareFileItem>() { shareFileItem }, null, response.Object);
            var pageable = Pageable<ShareFileItem>.FromPages(new List<Page<ShareFileItem>>() { page });
            shareDirectoryClient.Setup(x => x.GetFilesAndDirectories(It.IsAny<ShareDirectoryGetFilesAndDirectoriesOptions>(), It.IsAny<CancellationToken>())).Returns(pageable);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = service.GetAllFilesFromDirectory(shareClient.Object, path);
            var result2 = service.GetAllFilesFromDirectory(fileShareName, path, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo(fileName));
                Assert.That(result2, Is.Not.Null);
                Assert.That(result2?.Name, Is.EqualTo(fileName));
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public void DeleteFileFromDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareFileClient.Setup(x => x.DeleteIfExists(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>())).Returns(response.Object);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = service.DeleteFileFromDirectory(shareClient.Object, path, directoryName);
            var result2 = service.DeleteFileFromDirectory(fileShareName, path, directoryName, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task DeleteFileFromDirectoryAsync_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareFileClient.Setup(x => x.DeleteIfExistsAsync(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.DeleteFileFromDirectoryAsync(shareClient.Object, path, directoryName);
            var result2 = await service.DeleteFileFromDirectoryAsync(fileShareName, path, directoryName, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task GetFileFromDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            var responseDownload = new Mock<Response<ShareFileDownloadInfo>>();
            var stream = new MemoryStream();
            var shareFileDownloadInfo = FilesModelFactory.StorageFileDownloadInfo(content: stream);
            responseDownload.Setup(x => x.Value).Returns(shareFileDownloadInfo);
            shareFileClient.Setup(x => x.DownloadAsync(It.IsAny<ShareFileDownloadOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseDownload.Object);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareFileClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.GetFileFromDirectory(shareClient.Object, path, fileName);
            var result2 = await service.GetFileFromDirectory(fileShareName, path, fileName, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result2, Is.Not.Null);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task ReadFileFromDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            var responseDownload = new Mock<Response<ShareFileDownloadInfo>>();
            var stream = new MemoryStream();
            shareFileClient.Setup(x => x.OpenReadAsync(It.IsAny<ShareFileOpenReadOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(stream);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareFileClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            var responseStream = new Mock<Response<Stream>>();
            responseStream.Setup(x => x.Value).Returns(stream);
            shareFileClient.Setup(x => x.OpenReadAsync(It.IsAny<long>(), It.IsAny<int?>(), It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseStream.Object);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.ReadFileFromDirectory(shareClient.Object, path, fileName);
            var result2 = await service.ReadFileFromDirectory(fileShareName, path, fileName, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result2, Is.Not.Null);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }

        [Test]
        public async Task UploadFileToDirectory_Verify()
        {
            //Arrange
            var shareClient = new Mock<ShareClient>();
            var shareDirectoryClient = new Mock<ShareDirectoryClient>();
            var shareFileClient = new Mock<ShareFileClient>();
            var responseDownload = new Mock<Response<ShareFileDownloadInfo>>();
            var bytes = new byte[] { 1 };
            var stream = new MemoryStream(bytes);
            shareFileClient.Setup(x => x.OpenReadAsync(It.IsAny<ShareFileOpenReadOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(stream);
            var responseShareFileInfo = new Mock<Response<ShareFileInfo>>();
            var shareFileInfo = FilesModelFactory.StorageFileInfo();
            responseShareFileInfo.Setup(x => x.Value).Returns(shareFileInfo);
            shareFileClient.Setup(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<ShareFileCreateOptions>(), It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseShareFileInfo.Object);
            var responseShareFileUploadInfo = new Mock<Response<ShareFileUploadInfo>>();
            var shareFileUploadInfo = new Mock<ShareFileUploadInfo>();
            responseShareFileUploadInfo.Setup(x => x.Value).Returns(shareFileUploadInfo.Object);
            shareFileClient.Setup(x => x.UploadRangeAsync(It.IsAny<HttpRange>(), It.IsAny<Stream>(), It.IsAny<ShareFileUploadRangeOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseShareFileUploadInfo.Object);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            shareDirectoryClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            shareDirectoryClient.Setup(x => x.GetFileClient(It.IsAny<string>())).Returns(shareFileClient.Object);
            shareClient.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClient.Object);
            var service = new FileShareService((storageConnectionString, fileShareName) => shareClient.Object);

            //Act
            var result = await service.UploadFileToDirectory(shareClient.Object, path, fileName, stream);
            var result2 = await service.UploadFileToDirectory(fileShareName, path, fileName, stream, storageConnectionString);
            var hasKey = service.Clients.TryGetValue(fileShareName, out _);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result2, Is.Not.Null);
                Assert.That(service.Clients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            }
        }
    }
}