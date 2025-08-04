using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Azure.Amqp.Encoding;
using Roo.Azure.Configuration.Common.Utilities.Storage;

namespace Roo.Azure.Configuration.UnitTests
{
    public class BlobServiceTests
    {
        private string storageName = "storageName";
        private string containerName = "containerName";
        private string blobName = "blobName";

        [Test]
        public void GetAzureStorageUrl_Verify()
        {
            //Arrange
            var service = new BlobService();

            //Act
            var result = service.GetAzureStorageUrl(storageName, containerName, blobName);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo($"https://{storageName}.blob.core.windows.net/{containerName}/{blobName}"));
            });
        }

        [Test]
        public void GetContainerClient_Verify()
        {
            //Arrange
            var managedIdentityClientId = "managedIdentityClientId";
            var containerManagedIdentity = "containerManagedIdentity";
            var tokenCredential = new ManagedIdentityCredential();
            var containerToken = "containerToken";
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object);
            var containerClientManagedIdentity = new Mock<BlobContainerClient>();
            containerClientManagedIdentity.Setup(x => x.Name).Returns(containerManagedIdentity);
            var serviceManagedIdentity = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClientManagedIdentity.Object);
            var containerClientToken = new Mock<BlobContainerClient>();
            containerClientToken.Setup(x => x.Name).Returns(containerToken);
            var serviceToken = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClientToken.Object);

            //Act
            var result = service.GetContainerClient(storageName, containerName);
            var resultManagedIdentity = serviceManagedIdentity.GetContainerClient(storageName, containerManagedIdentity, managedIdentityClientId);
            var resultToken = serviceToken.GetContainerClient(storageName, containerToken, tokenCredential);
            var hasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var hasKeyManagedIdentity = serviceManagedIdentity.ContainerClients.TryGetValue(containerManagedIdentity, out _);
            var hasKeyToken = serviceToken.ContainerClients.TryGetValue(containerToken, out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo(containerName));
                Assert.That(resultManagedIdentity.Name, Is.EqualTo(containerManagedIdentity));
                Assert.That(resultToken.Name, Is.EqualTo(containerToken));
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
                Assert.That(hasKeyManagedIdentity, Is.True);
                Assert.That(hasKeyToken, Is.True);
            });
        }

        [Test]
        public void GetBlobClient_Verify()
        {
            //Arrange
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            containerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClient.Object);
            var service = new BlobService(null, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = service.GetBlobClient(containerClient.Object, blobName);
            var hasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo(blobName));
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(hasKey, Is.True);
            });
        }

        [Test]
        public void GetFileByName_Verify()
        {
            //Arrange
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var stream = new MemoryStream();
            blobClient.Setup(x => x.OpenReadAsync(It.IsAny<long>(), It.IsAny<int?>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(stream);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = service.GetFileByName(blobName, containerName, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public void DeleteImage_Verify()
        {
            //Arrange
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            blobClient.Setup(x => x.DeleteIfExists(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).Returns(response.Object);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = service.DeleteImage(blobName, containerName, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public async Task DeleteImageAsync_Verify()
        {
            //Arrange
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            blobClient.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = await service.DeleteImageAsync(blobName, containerName, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public async Task UploadAsync_Verify()
        {
            //Arrange
            var stream = new MemoryStream();
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var response = new Mock<Response<BlobContentInfo>>();
            var blobContentInfo = new Mock<BlobContentInfo>();
            response.Setup(x => x.Value).Returns(blobContentInfo.Object);
            blobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = await service.UploadAsync(blobName, stream, containerName, "contentType", false, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public void Upload_Verify()
        {
            //Arrange
            var stream = new MemoryStream();
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var response = new Mock<Response<BlobContentInfo>>();
            var blobContentInfo = new Mock<BlobContentInfo>();
            response.Setup(x => x.Value).Returns(blobContentInfo.Object);
            blobClient.Setup(x => x.Upload(It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns(response.Object);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = service.Upload(blobName, stream, containerName, "contentType", false, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public async Task CheckFile_Verify()
        {
            //Arrange
            var stream = new MemoryStream();
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x => x.Name).Returns(blobName);
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(true);
            blobClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response.Object);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, (blobContainerClient, blobName) => blobClient.Object);

            //Act
            var result = await service.CheckFile(blobName, containerName, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);
            var blobHasKey = service.BlobClients.TryGetValue((containerName, blobName), out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
                Assert.That(service.BlobClients, Is.Not.Null);
                Assert.That(blobHasKey, Is.True);
            });
        }

        [Test]
        public void GetFromDirectory_Verify()
        {
            //Arrange
            var stream = new MemoryStream();
            var containerClient = new Mock<BlobContainerClient>();
            containerClient.Setup(x => x.Name).Returns(containerName);
            containerClient.Setup(x => x.Name).Returns(containerName);
            var blobItem = new Mock<BlobItem>();
            var response = new Mock<Response>();
            var page = Page<BlobItem>.FromValues(new List<BlobItem>() { blobItem.Object }, null, response.Object);
            var pageable = Pageable<BlobItem>.FromPages(new List<Page<BlobItem>>() { page });
            containerClient.Setup(x => x.GetBlobs(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(pageable);
            var service = new BlobService((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) => containerClient.Object, null);

            //Act
            var result = service.GetFromDirectory("prefix", containerName, storageName);
            var containerHasKey = service.ContainerClients.TryGetValue(containerName, out _);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(service.ContainerClients, Is.Not.Null);
                Assert.That(containerHasKey, Is.True);
            });
        }
    }
}