using Azure.Core;
using Azure.Messaging.ServiceBus;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Wrapper for Azure Service Bus methods to simplify calls.
    /// </summary>
    public interface IServiceBusService
    {
        /// <summary>
        /// Send string message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, string message, TokenCredential token);

        /// <summary>
        /// Send string message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, string message);

        /// <summary>
        /// Send byte message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ReadOnlyMemory<byte> message, TokenCredential token);

        /// <summary>
        /// Send byte message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ReadOnlyMemory<byte> message);

        /// <summary>
        /// Send binary data message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, BinaryData message, TokenCredential token);

        /// <summary>
        /// Send binary data message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, BinaryData message);

        /// <summary>
        /// Send <see cref="ServiceBusReceivedMessage"/> message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusReceivedMessage message, TokenCredential token);

        /// <summary>
        /// Send <see cref="ServiceBusReceivedMessage"/> message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusReceivedMessage message);
    }

    public class ServiceBusService : IServiceBusService
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, string message, TokenCredential token)
        {
            var serviceBusClient = new ServiceBusClient($"{serviceBusNamespace}.servicebus.windows.net", token);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, string message)
        {
            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ReadOnlyMemory<byte> message, TokenCredential token)
        {
            var serviceBusClient = new ServiceBusClient($"{serviceBusNamespace}.servicebus.windows.net", token);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ReadOnlyMemory<byte> message)
        {
            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, BinaryData message, TokenCredential token)
        {
            var serviceBusClient = new ServiceBusClient($"{serviceBusNamespace}.servicebus.windows.net", token);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, BinaryData message)
        {
            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusReceivedMessage message, TokenCredential token)
        {
            var serviceBusClient = new ServiceBusClient($"{serviceBusNamespace}.servicebus.windows.net", token);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusReceivedMessage message)
        {
            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message)).ConfigureAwait(false);
        }
    }
}
