using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ServiceBusServiceTests
    {
        //Common
        private readonly string serviceBusNamespace = "serviceBusNamespace";
        private readonly string serviceBusQueueName = "serviceBusQueueName";
        private readonly string serviceBusConnectionString = "serviceBusConnectionString";
        private readonly string messageString = "message";
        private readonly ReadOnlyMemory<byte> messageReadOnlyMemory = new([0]);
        private readonly BinaryData binaryData = new([0]);
        private readonly ServiceBusMessage serviceBusMessage = new("message");
        private readonly DefaultAzureCredential token = new();
        private readonly DateTimeOffset dateTimeOffset = new();
        private readonly CancellationToken cancellationToken = new();

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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), null), Times.Once);
            }
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
            using (Assert.EnterMultipleScope())
            {
                serviceBusClient.Verify(x => x.CreateSender(It.IsAny<string>()), Times.Once);
                serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
        #endregion
    }
}