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
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, string message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send string message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, string message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send byte message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ReadOnlyMemory<byte> message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send byte message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ReadOnlyMemory<byte> message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send binary data message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, BinaryData message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send binary data message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, BinaryData message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send <see cref="ServiceBusReceivedMessage"/> message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusReceivedMessage message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send <see cref="ServiceBusReceivedMessage"/> message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusReceivedMessage message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send string message to service bus using token authorization.
        /// </summary>
        /// <param name="serviceBusNamespace">Namespace of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="token">Token with authorization to send messages with the service bus.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusMessage message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Send string message to service bus using connection string authorization.
        /// </summary>
        /// <param name="serviceBusConnectionString">Connection string of service bus.</param>
        /// <param name="serviceBusQueueName">Queue name of service bus.</param>
        /// <param name="message">Message being sent.</param>
        /// <param name="enqueueTime">Sets the time to make the message available to receivers.</param>
        /// <param name="cancellationToken">Token to cancel the request before it completes on its own.</param>
        /// <returns></returns>
        public Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusMessage message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null);
    }

    public class ServiceBusService : IServiceBusService
    {
        private readonly Func<string, TokenCredential?, IServiceBusClient> _clientFactory;

        public ServiceBusService(Func<string, TokenCredential?, IServiceBusClient>? clientFactory = null)
        {
            _clientFactory = clientFactory ?? ((connection, token) =>
            {
                if (token != null)
                {
                    return new ServiceBusClientAdapter(new ServiceBusClient($"{connection}.servicebus.windows.net", token));
                }
                else
                {
                    return new ServiceBusClientAdapter(new ServiceBusClient(connection));
                }
            });
        }

        private static async Task SendToServiceBusBase(IServiceBusClient serviceBusClient, string serviceBusQueueName, ServiceBusMessage serviceBusMessage, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusQueueName);
            if (enqueueTime != null)
            {
                serviceBusMessage.ScheduledEnqueueTime = (DateTimeOffset)enqueueTime;
            }
            await serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, string message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusNamespace, token);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, string message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusConnectionString, null);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ReadOnlyMemory<byte> message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusNamespace, token);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ReadOnlyMemory<byte> message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusConnectionString, null);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, BinaryData message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusNamespace, token);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, BinaryData message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusConnectionString, null);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusReceivedMessage message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusNamespace, token);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusReceivedMessage message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusConnectionString, null);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, new ServiceBusMessage(message), enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusNamespace"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusNamespace, string serviceBusQueueName, ServiceBusMessage message, TokenCredential token, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusNamespace, token);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, message, enqueueTime, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="serviceBusQueueName"></param>
        /// <param name="message"></param>
        /// <param name="enqueueTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToServiceBus(string serviceBusConnectionString, string serviceBusQueueName, ServiceBusMessage message, DateTimeOffset? enqueueTime = null, CancellationToken? cancellationToken = null)
        {
            var serviceBusClient = _clientFactory(serviceBusConnectionString, null);
            await SendToServiceBusBase(serviceBusClient, serviceBusQueueName, message, enqueueTime, cancellationToken);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IServiceBusClient"/>
    /// </summary>
    public class ServiceBusClientAdapter : IServiceBusClient
    {
        private readonly ServiceBusClient _client;

        /// <summary>
        /// Initialize <see cref="ServiceBusClientAdapter"/>
        /// </summary>
        /// <param name="sender"></param>
        public ServiceBusClientAdapter(ServiceBusClient client) => _client = client;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public IServiceBusSender CreateSender(string queueName) => new ServiceBusSenderAdapter(_client.CreateSender(queueName));

        /// <summary>
        /// <inheritdoc cref="ServiceBusClient.DisposeAsync()"/>
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return _client.DisposeAsync();
        }
    }

    /// <summary>
    /// <inheritdoc cref="IServiceBusSender"/>
    /// </summary>
    public class ServiceBusSenderAdapter : IServiceBusSender
    {
        private readonly ServiceBusSender _sender;

        /// <summary>
        /// Initialize <see cref="ServiceBusSenderAdapter"/>
        /// </summary>
        /// <param name="sender"></param>
        public ServiceBusSenderAdapter(ServiceBusSender sender) => _sender = sender;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(ServiceBusMessage message, CancellationToken? token = null)
        {
            if (token == null)
            {
                await _sender.SendMessageAsync(message).ConfigureAwait(false);
            }
            else
            {
                await _sender.SendMessageAsync(message, (CancellationToken)token).ConfigureAwait(false);
            }
            await DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc cref="ServiceBusSender.DisposeAsync()"/>
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return _sender.DisposeAsync();
        }
    }

    /// <summary>
    /// ServiceBusClient wrapper to simplify unit testing and decouple code.
    /// </summary>
    public interface IServiceBusClient : IAsyncDisposable
    {
        /// <summary>
        /// Creates a <see cref="ServiceBusSenderAdapter"/> instance that can be used for sending messages to a specific topic or queue.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        IServiceBusSender CreateSender(string queueName);
    }

    /// <summary>
    /// ServiceBusSender wrapper to simplify unit testing and decouple code.
    /// </summary>
    public interface IServiceBusSender : IAsyncDisposable
    {
        /// <summary>
        /// <inheritdoc cref="ServiceBusSender.SendMessageAsync(ServiceBusMessage, CancellationToken)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendMessageAsync(ServiceBusMessage message, CancellationToken? token = null);
    }
}
