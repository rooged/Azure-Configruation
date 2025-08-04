using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace Roo.Azure.Configuration.Common.Utilities.Storage
{
    /// <summary>
    /// Service for Azure Blob to retrieve files, upload files, and manage blobs.
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Container clients cache. Any client created using the service is cached here and can be retrieved using the container name.
        /// </summary>
        public Dictionary<string, BlobContainerClient> ContainerClients { get; }

        /// <summary>
        /// Blob clients cache. Any client created using the service is cached here and can be retrieved using the container name and the blob name.
        /// </summary>
        public Dictionary<(string ContainerName, string BlobName), BlobClient> BlobClients { get; }

        /// <summary>
        /// Create Azure Blob storage URL from storage, container, and blob names.
        /// </summary>
        /// <param name="storageName">Name of storage.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="blobName">Name of blob.</param>
        /// <returns></returns>
        public string GetAzureStorageUrl(string storageName, string containerName, string blobName);

        /// <summary>
        /// Get <see cref="BlobContainerClient"/> configured using a token. Any container client created using the service is cached ContainerClients and can be retrieved using the container name.
        /// </summary>
        /// <param name="storageName">Name of storage.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="tokenCredential">Access credential with access to the container.</param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageName, string containerName, TokenCredential tokenCredential);

        /// <summary>
        /// Get <see cref="BlobContainerClient"/> configured using a Managed Identity Client Id or DefaultAzureCredential. Any container client created using the service is cached ContainerClients and can be retrieved using the container name.
        /// </summary>
        /// <param name="storageName">Name of storage.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="managedIdentityClientId">Managed identity client id with access to the container.</param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageName, string containerName, string? managedIdentityClientId = null);

        /// <summary>
        /// Get <see cref="BlobContainerClient"/> configured using the settings using a connection string. Any container client created using the service is cached ContainerClients and can be retrieved using the container name.
        /// </summary>
        /// <param name="storageConnectionString">Storage connection string.</param>
        /// <param name="containerName">Name of container for blob.</param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageConnectionString, string containerName);

        /// <summary>
        /// Create a new Azure Blob client. Any blob client created using the service is cached BlobClients and can be retrieved using the container name and blob name.
        /// </summary>
        /// <param name="containerClient">Name of client.</param>
        /// <param name="blobName">Name of blob.</param>
        /// <returns></returns>
        public BlobClient GetBlobClient(BlobContainerClient containerClient, string blobName);

        /// <summary>
        /// Get SAS URI for BlobClient.
        /// </summary>
        /// <param name="blobClient"></param>
        /// <returns></returns>
        public string GetSasUri(BlobClient blobClient);

        /// <summary>
        /// Get an Azure Blob by blob name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="blobName">Name of Blob to retrieve from.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Stream?> GetFileByName(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Delete an Azure Blob by blob name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="blobName">Name of Blob to delete.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public bool DeleteImage(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="DeleteImage(string, string, string?, CancellationToken?)"/>
        /// </summary>
        /// <param name="blobName">Name of Blob to delete.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteImageAsync(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Upload a file stream to an Azure Blob by blob name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="blobName">Name of Blob to upload to.</param>
        /// <param name="stream"></param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="contentType"></param>
        /// <param name="overwrite">Whether to overwrite an existing file if they share the same name.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<BlobContentInfo> UploadAsync(string blobName, Stream stream, string containerName, string? contentType = null, bool overwrite = false, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Upload a file stream to an Azure Blob by blob name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="blobName">Name of Blob to upload to.</param>
        /// <param name="stream"></param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="contentType"></param>
        /// <param name="overwrite">Whether to overwrite an existing file if they share the same name.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public BlobContentInfo Upload(string blobName, Stream stream, string containerName, string? contentType = null, bool overwrite = false, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Check if an Azure Blob exists by blob name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="blobName">Name of Blob to check.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> CheckFile(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Get all Blobs from directory with prefix. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="prefix">Prefix of Blobs to retrieve.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="storageConnectionString">Connection string to storage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Pageable<BlobItem>? GetFromDirectory(string prefix, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class BlobService : IBlobService
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Dictionary<string, BlobContainerClient> ContainerClients { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Dictionary<(string ContainerName, string BlobName), BlobClient> BlobClients { get; set; }

        private readonly Func<string, string, string?, TokenCredential?, BlobContainerClient> _blobContainerClientFactory;
        private readonly Func<BlobContainerClient, string, BlobClient> _blobClientFactory;

        public BlobService(Func<string, string, string?, TokenCredential?, BlobContainerClient>? containerClientFactory = null, Func<BlobContainerClient, string, BlobClient>? blobClientFactory = null)
        {
            ContainerClients = new();
            _blobContainerClientFactory = containerClientFactory ?? ((connectionStringOrStorageName, containerName, managedIdentityClientId, tokenCredential) =>
            {
                if (!string.IsNullOrEmpty(managedIdentityClientId))
                {
                    var client = new BlobContainerClientAdapter().CreateClient(connectionStringOrStorageName, containerName, managedIdentityClientId);
                    ContainerClients.Add(containerName, client);
                    return client;
                }
                else if (tokenCredential != null)
                {
                    var client = new BlobContainerClientAdapter().CreateClient(connectionStringOrStorageName, containerName, tokenCredential);
                    ContainerClients.Add(containerName, client);
                    return client;
                }
                else
                {
                    var client = new BlobContainerClientAdapter().CreateClient(connectionStringOrStorageName, containerName);
                    ContainerClients.Add(containerName, client);
                    return client;
                }
            });
            BlobClients = new();
            _blobClientFactory = blobClientFactory ?? ((blobContainerClient, blobName) =>
            {
                var client = new BlobClientAdapter().CreateClient(blobContainerClient, blobName);
                BlobClients.Add((blobContainerClient.Name, blobName), client);
                return client;
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public string GetAzureStorageUrl(string storageName, string containerName, string blobName)
        {
            return $"https://{storageName}.blob.core.windows.net/{containerName}/{blobName}";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="tokenCredential"></param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageName, string containerName, TokenCredential tokenCredential)
        {
            if (!ContainerClients.TryGetValue(containerName, out var client))
            {
                client = _blobContainerClientFactory(storageName, containerName, null, tokenCredential);
                ContainerClients.TryAdd(containerName, client);
            }
            return client;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageName, string containerName, string? managedIdentityClientId = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var client))
            {
                client = _blobContainerClientFactory(storageName, containerName, managedIdentityClientId, null);
                ContainerClients.TryAdd(containerName, client);
            }
            return client;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public BlobContainerClient GetContainerClient(string storageConnectionString, string containerName)
        {
            if (!ContainerClients.TryGetValue(containerName, out var client))
            {
                client = _blobContainerClientFactory(storageConnectionString, containerName, null, null);
                ContainerClients.TryAdd(containerName, client);
            }
            return client;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public BlobClient GetBlobClient(BlobContainerClient containerClient, string blobName)
        {
            if (!BlobClients.TryGetValue((containerClient.Name, blobName), out var client))
            {
                client = _blobClientFactory(containerClient, blobName);
                BlobClients.TryAdd((containerClient.Name, blobName), client);
            }
            return client;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobClient"></param>
        /// <returns></returns>
        public string GetSasUri(BlobClient blobClient)
        {
            var serviceClient = blobClient.GetParentBlobContainerClient().GetParentBlobServiceClient();
            var delegationKey = serviceClient.GetUserDelegationKey(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10));

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(10),
            };

            //Specify read permissisons for the SAS
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            //Add SAS token to blob URI
            var uriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(delegationKey, serviceClient.AccountName)
            };
            return uriBuilder.ToUri().ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream?> GetFileByName(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    return null;
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            if (await blobClient.ExistsAsync())
            {
                if (cancellationToken != null)
                {
                    return await blobClient.OpenReadAsync(cancellationToken: (CancellationToken)cancellationToken);
                }
                else
                {
                    return await blobClient.OpenReadAsync();
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public bool DeleteImage(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    return false;
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            Response<bool> result;
            if (cancellationToken != null)
            {
                result = blobClient.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: (CancellationToken)cancellationToken);
            }
            else
            {
                result = blobClient.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
            }
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageAsync(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    return false;
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            Response<bool> result;
            if (cancellationToken != null)
            {
                result = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: (CancellationToken)cancellationToken);
            }
            else
            {
                result = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="stream"></param>
        /// <param name="containerName"></param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BlobContentInfo> UploadAsync(string blobName, Stream stream, string containerName, string? contentType = null, bool overwrite = false, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    throw new KeyNotFoundException($"{containerName} does not exist in the client cache and could not create a new client without the storage connection string.");
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            stream.Position = 0;
            if (contentType != null)
            {
                var headers = new BlobHttpHeaders { ContentType = contentType };
                blobClient.SetHttpHeaders(headers);
            }
            if (cancellationToken != null)
            {
                return (await blobClient.UploadAsync(stream, overwrite, (CancellationToken)cancellationToken)).Value;
            }
            return (await blobClient.UploadAsync(stream, overwrite)).Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="stream"></param>
        /// <param name="containerName"></param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public BlobContentInfo Upload(string blobName, Stream stream, string containerName, string? contentType = null, bool overwrite = false, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    throw new KeyNotFoundException($"{containerName} does not exist in the client cache and could not create a new client without the storage connection string.");
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            stream.Position = 0;
            if (contentType != null)
            {
                var headers = new BlobHttpHeaders { ContentType = contentType };
                blobClient.SetHttpHeaders(headers);
            }
            if (cancellationToken != null)
            {
                return blobClient.Upload(stream, overwrite, (CancellationToken)cancellationToken).Value;
            }
            return blobClient.Upload(stream, overwrite).Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckFile(string blobName, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    return false;
                }
            }
            if (!BlobClients.TryGetValue((containerName, blobName), out var blobClient))
            {
                blobClient = GetBlobClient(containerClient, blobName);
            }
            Response<bool> result;
            if (cancellationToken != null)
            {
                result = await blobClient.ExistsAsync((CancellationToken)cancellationToken);
            }
            else
            {
                result = await blobClient.ExistsAsync();
            }
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="containerName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Pageable<BlobItem>? GetFromDirectory(string prefix, string containerName, string? storageConnectionString = null, CancellationToken? cancellationToken = null)
        {
            if (!ContainerClients.TryGetValue(containerName, out var containerClient))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    containerClient = GetContainerClient(storageConnectionString, containerName);
                }
                else
                {
                    return null;
                }
            }
            if (cancellationToken != null)
            {
                return containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix, (CancellationToken)cancellationToken);
            }
            else
            {
                return containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix);
            }
        }
    }

    /// <summary>
    /// <inheritdoc cref="IBlobContainerClient"/>
    /// </summary>
    public class BlobContainerClientAdapter : IBlobContainerClient
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="tokenCredential"></param>
        /// <returns></returns>
        public BlobContainerClient CreateClient(string storageName, string containerName, TokenCredential tokenCredential)
        {
            return new BlobContainerClient(new Uri($"https://{storageName}.blob.core.windows.net/{containerName}"), tokenCredential);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="managedIdentityClientId"></param>
        /// <returns></returns>
        public BlobContainerClient CreateClient(string storageName, string containerName, string? managedIdentityClientId = null)
        {
            TokenCredential token = string.IsNullOrEmpty(managedIdentityClientId) ? new DefaultAzureCredential() : new ManagedIdentityCredential(managedIdentityClientId);
            return new BlobContainerClient(new Uri($"https://{storageName}.blob.core.windows.net/{containerName}"), token);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public BlobContainerClient CreateClient(string storageConnectionString, string containerName)
        {
            return new BlobContainerClient(storageConnectionString, containerName);
        }
    }

    /// <summary>
    /// BlobContainerClient wrapper to simplify unit testing and decouple code.
    /// </summary>
    public interface IBlobContainerClient
    {
        /// <summary>
        /// Creates a <see cref="BlobContainerClientAdapter"/> instance that creates a new Blob Container client.
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="tokenCredential"></param>
        /// <returns></returns>
        BlobContainerClient CreateClient(string storageName, string containerName, TokenCredential tokenCredential);

        /// <summary>
        /// Creates a <see cref="BlobContainerClientAdapter"/> instance that creates a new Blob Container client.
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="containerName"></param>
        /// <param name="managedIdentityClientId"></param>
        /// <returns></returns>
        BlobContainerClient CreateClient(string storageName, string containerName, string? managedIdentityClientId = null);

        /// <summary>
        /// Creates a <see cref="BlobContainerClientAdapter"/> instance that creates a new Blob Container client.
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        BlobContainerClient CreateClient(string storageConnectionString, string containerName);
    }

    /// <summary>
    /// <inheritdoc cref="IBlobClient"/>
    /// </summary>
    public class BlobClientAdapter : IBlobClient
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public BlobClient CreateClient(BlobContainerClient containerClient, string blobName)
        {
            return containerClient.GetBlobClient(blobName);
        }
    }

    /// <summary>
    /// BlobClient wrapper to simplify unit testing and decouple code.
    /// </summary>
    public interface IBlobClient
    {
        /// <summary>
        /// Creates a <see cref="BlobClientAdapter"/> instance that creates a new Blob Container client.
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        BlobClient CreateClient(BlobContainerClient containerClient, string blobName);
    }
}
