using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ServiceBusServiceTests
    {
        //Common
        private string serviceBusNamespace = "serviceBusNamespace";
        private string serviceBusQueueName = "serviceBusQueueName";
        private string serviceBusConnectionString = "serviceBusConnectionString";
        private string messageString = "message";
        private ReadOnlyMemory<byte> messageReadOnlyMemory = new ReadOnlyMemory<byte>([0]);
        private BinaryData binaryData = new BinaryData([0]);
        private ServiceBusMessage serviceBusMessage = new ServiceBusMessage("message");
        private DefaultAzureCredential token = new();
        private DateTimeOffset dateTimeOffset = new();
        private CancellationToken cancellationToken = new();

        #region String
        [Test]
        public async Task SendToServiceBusStringWithToken_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusNamespace, serviceBusQueueName, messageString, token);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            });
        }

        [Test]
        public async Task SendToServiceBusString_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusConnectionString, serviceBusQueueName, messageString, dateTimeOffset, cancellationToken);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            });
        }
        #endregion

        #region ReadOnlyMemory
        [Test]
        public async Task SendToServiceBusReadOnlyMemoryWithToken_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusNamespace, serviceBusQueueName, messageReadOnlyMemory, token);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            });
        }

        [Test]
        public async Task SendToServiceBusReadOnlyMemory_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusConnectionString, serviceBusQueueName, messageReadOnlyMemory, dateTimeOffset, cancellationToken);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            });
        }
        #endregion

        #region BinaryData
        [Test]
        public async Task SendToServiceBusBinaryDataWithToken_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusNamespace, serviceBusQueueName, binaryData, token);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            });
        }

        [Test]
        public async Task SendToServiceBusBinaryData_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusConnectionString, serviceBusQueueName, binaryData, dateTimeOffset, cancellationToken);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            });
        }
        #endregion

        #region ServiceBusMessage
        [Test]
        public async Task SendToServiceBusServiceBusMessageWithToken_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusNamespace, serviceBusQueueName, serviceBusMessage, token);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            });
        }

        [Test]
        public async Task SendToServiceBusServiceBusMessage_Verify()
        {
            //Arrange
            var serviceBusClient = new Mock<IServiceBusClient>();
            var serviceBusSender = new Mock<IServiceBusSender>();
            serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));
            serviceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSender.Object);
            var service = new ServiceBusService((connection, token) => serviceBusClient.Object);

            //Act
            await service.SendToServiceBus(serviceBusConnectionString, serviceBusQueueName, serviceBusMessage, dateTimeOffset, cancellationToken);

            //Assert
            Assert.Multiple(() =>
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            });
        }
        #endregion
    }
}