using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace Roo.Azure.Configuration.Common.Utilities.AzureBlob
{
    /// <summary>
    /// Settings and helper functions for integrating Azure Blob and Azure File Share.
    /// </summary>
    public interface IAzureSettings
    {
        /// <summary>
        /// Name of storage for blob.
        /// </summary>
        public string? StorageName { get; set; }

        /// <summary>
        /// Name of container for blob.
        /// </summary>
        public string? ContainerName { get; set; }

        /// <summary>
        /// Managed Identity Client Id with access to the Blob resource.
        /// </summary>
        public string? ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Storage connection string.
        /// </summary>
        public string? StorageConnectionString { get; set; }

        //public AzureSettingsOptions Settings { get; set; }

        /// <summary>
        /// Get Azure Blob storage url.
        /// </summary>
        /// <param name="storageName">Name of storage.</param>
        /// <param name="containerName">Name of container.</param>
        /// <param name="blobName">Name of blob.</param>
        /// <returns></returns>
        public string GetAzureStorageUrl(string storageName, string containerName, string blobName);

        /// <summary>
        /// Get Azure Blob client.
        /// </summary>
        /// <param name="client">Name of client.</param>
        /// <param name="blobName">Name of blob.</param>
        /// <returns></returns>
        public BlobClient GetBlobClient(BlobContainerClient client, string blobName);

        /// <summary>
        /// Get <see cref="BlobContainerClient"/>  configured using the settings using an acess token.
        /// </summary>
        /// <param name="azureSettings">Azure settings to configure using.</param>
        /// <param name="tokenCredential">Access credential with access to the container.</param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(IAzureSettings azureSettings, TokenCredential tokenCredential);
        //public BlobContainerClient GetCloudClient(TokenCredential tokenCredential);

        /// <summary>
        /// Get <see cref="BlobContainerClient"/> configured using the settings using a Managed Identity Client Id or DefaultAzureCredential.
        /// </summary>
        /// <param name="azureSettings">Azure settings to configure using.</param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(IAzureSettings azureSettings);
        //public BlobContainerClient GetCloudClient();

        /// <summary>
        /// Get <see cref="BlobContainerClient"/> configured using the settings using a connection string.
        /// </summary>
        /// <param name="storageConnectionString">Storage connection string.</param>
        /// <param name="containerName">Name of container for blob.</param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(string storageConnectionString, string containerName);

        /// <summary>
        /// Get SAS Uri for BlobClient.
        /// </summary>
        /// <param name="blobClient"></param>
        /// <returns></returns>
        public string GetSasUri(BlobClient blobClient);
    }

    public class AzureSettings : IAzureSettings
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? StorageName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? ContainerName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? ManagedIdentityClientId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? StorageConnectionString { get; set; }

        //public AzureSettingsOptions Settings { get; set; } = new();

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
        /// <param name="client"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public BlobClient GetBlobClient(BlobContainerClient client, string blobName)
        {
            return client.GetBlobClient(blobName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="tokenCredential"></param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(IAzureSettings settings, TokenCredential tokenCredential)
        {
            return new BlobContainerClient(new Uri($"https://{settings.StorageName}.blob.core.windows.net/{settings.ContainerName}"), tokenCredential ?? new DefaultAzureCredential());
        }
        /*public BlobContainerClient GetCloudClient(TokenCredential tokenCredential)
        {
            return new BlobContainerClient(new Uri($"https://{_settings.StorageName}.blob.core.windows.net/{_settings.ContainerName}"), tokenCredential ?? new DefaultAzureCredential());
        }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(IAzureSettings settings)
        {
            var token = string.IsNullOrEmpty(settings.ManagedIdentityClientId) ? (TokenCredential)new DefaultAzureCredential() : new ManagedIdentityCredential(settings.ManagedIdentityClientId);
            return new BlobContainerClient(new Uri($"https://{settings.StorageName}.blob.core.windows.net/{settings.ContainerName}"), token);
        }
        /*public BlobContainerClient GetCloudClient()
        {
            var token = string.IsNullOrEmpty(_settings.ManagedIdentityClientId) ? (TokenCredential)new DefaultAzureCredential() : new ManagedIdentityCredential(_settings.ManagedIdentityClientId);
            return new BlobContainerClient(new Uri($"https://{_settings.StorageName}.blob.core.windows.net/{_settings.ContainerName}"), token);
        }*/

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public BlobContainerClient GetCloudClient(string storageConnectionString, string containerName)
        {
            return new BlobContainerClient(storageConnectionString, containerName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="blobClient"></param>
        /// <returns></returns>
        public string GetSasUri(BlobClient blobClient)
        {
            var serviceClient = blobClient.GetParentBlobContainerClient().GetParentBlobServiceClient();
            
            //Get a key that's valid for 10 minutes
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
    }

    /*public class AzureSettingsOptions
    {
        public string? StorageName { get; set; }

        public string? ContainerName { get; set; }

        public string? ManagedIdentityClientId { get; set; }

        public string? StorageConnectionString { get; set; }
    }*/
}
