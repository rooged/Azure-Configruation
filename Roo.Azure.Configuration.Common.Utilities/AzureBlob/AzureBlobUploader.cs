using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs.Models;

namespace Roo.Azure.Configuration.Common.Utilities.AzureBlob
{
    /// <summary>
    /// Service for uploading to Azure Blob.
    /// </summary>
    public interface IAzureBlobUploader
    {
        /// <summary>
        /// Upload to Azure blob using Managed Identity Client Id.
        /// </summary>
        /// <param name="containerName">Name of container for blob.</param>
        /// <param name="key">Key to store the upload under.</param>
        /// <param name="stream">Stream being uploaded.</param>
        /// <param name="storageName">Name of storage for blob.</param>
        /// <param name="managedIdentityClientId">Managed Identity Client Id with access to blob.</param>
        /// <param name="contentType">Type of content being uploaded.</param>
        /// <param name="overwrite">Overwrite existing if blob already contains the key.</param>
        public void Upload(string containerName, string key, Stream stream, string storageName, string managedIdentityClientId, string? contentType = "", bool overwrite = false);

        /// <summary>
        /// Upload to Azure blob using Managed Identity Client Id.
        /// </summary>
        /// <param name="containerName">Name of container for blob.</param>
        /// <param name="key">Key to store the upload under.</param>
        /// <param name="stream">Stream being uploaded.</param>
        /// <param name="storageName">Name of storage for blob.</param>
        /// <param name="contentType">Type of content being uploaded.</param>
        /// <param name="overwrite">Overwrite existing if blob already contains the key.</param>
        /// <param name="token">Token credential with access to the blob.</param>
        public void Upload(string containerName, string key, Stream stream, string storageName, string? contentType = "", bool overwrite = false, TokenCredential? token = null);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class AzureBlobUploader : IAzureBlobUploader
    {
        private readonly IAzureSettings _settings;

        /// <summary>
        /// Initialize <see cref="AzureBlobUploader"/>.
        /// </summary>
        /// <param name="settings"></param>
        public AzureBlobUploader(IAzureSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="key"></param>
        /// <param name="stream"></param>
        /// <param name="storageName"></param>
        /// <param name="managedIdentityClientId"></param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        public void Upload(string containerName, string key, Stream stream, string storageName, string managedIdentityClientId, string? contentType = "", bool overwrite = false)
        {
            var token = string.IsNullOrEmpty(managedIdentityClientId) ? (TokenCredential)new DefaultAzureCredential() : new ManagedIdentityCredential(managedIdentityClientId);
            Upload(containerName, key, stream, storageName, contentType, overwrite, token);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="key"></param>
        /// <param name="stream"></param>
        /// <param name="storageName"></param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        /// /// <param name="token"></param>
        public void Upload(string containerName, string key, Stream stream, string storageName, string? contentType = "", bool overwrite = false, TokenCredential? token = null)
        {
            if (string.IsNullOrEmpty(_settings.StorageConnectionString) || string.IsNullOrEmpty(_settings.ContainerName))
            {
                return;
            }
            var cloudClient = _settings.GetCloudClient(_settings.StorageConnectionString, _settings.ContainerName);
            var blobClient = _settings.GetBlobClient(cloudClient, key);
            stream.Position = 0;
            var headers = new BlobHttpHeaders { ContentType = contentType };
            blobClient.SetHttpHeaders(headers);
            blobClient.Upload(stream, overwrite: overwrite);
        }
    }
}
