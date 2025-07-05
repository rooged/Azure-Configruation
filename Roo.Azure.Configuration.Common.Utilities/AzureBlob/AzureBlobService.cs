using Azure;
using Azure.Storage.Blobs.Models;

namespace Roo.Azure.Configuration.Common.Utilities.AzureBlob
{
    /// <summary>
    /// Service to integrate Azure Blob easily.
    /// </summary>
    public interface IAzureBlobService
    {
        /// <summary>
        /// Get an Azure Blob by blob name.
        /// </summary>
        /// <param name="name">Name of Blob to retrieve.</param>
        /// <param name="settings">Azure Settings required for access to Blob. StorageConnectionString and ContainerName required.</param>
        /// <returns></returns>
        public Task<Stream?> GetBlobFileByName(string name, IAzureSettings settings);

        /// <summary>
        /// Delete an Azure Blob by blob name.
        /// </summary>
        /// <param name="name">Name of Blob to delete.</param>
        /// <param name="settings">Azure Settings required for access to Blob. StorageConnectionString and ContainerName required.</param>
        /// <returns></returns>
        public Task<bool> DeleteBlobImage(string name, IAzureSettings settings);

        /// <summary>
        /// Upload a file stream to an Azure Blob by blob name.
        /// </summary>
        /// <param name="name">Name of Blob to upload to.</param>
        /// <param name="stream"></param>
        /// <param name="settings">Azure Settings required for access to Blob. StorageConnectionString and ContainerName required.</param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Task UploadFileStreamToBlob(string name, Stream stream, IAzureSettings settings, string? contentType = null, bool overwrite = false);

        /// <summary>
        /// Check if an Azure Blob exists by blob name.
        /// </summary>
        /// <param name="name">Name of Blob to check.</param>
        /// <param name="settings">Azure Settings required for access to Blob. StorageConnectionString and ContainerName required.</param>
        /// <returns></returns>
        public Task<bool> CheckFileInBlob(string name, IAzureSettings settings);

        /// <summary>
        /// Get all Blobs by with prefix.
        /// </summary>
        /// <param name="prefix">Prefix of Blobs to retrieve.</param>
        /// <param name="settings">Azure Settings required for access to Blob. StorageConnectionString and ContainerName required.</param>
        /// <returns></returns>
        public Task<Pageable<BlobItem>?> GetBlobsFromDirectory(string prefix, IAzureSettings settings);
        /*public Task<Stream?> GetBlobFileByName(string name, AzureSettingsOptions settings);
        public Task<bool> DeleteBlobImage(string name, AzureSettingsOptions settings);
        public Task UploadFileStreamToBlob(string name, Stream stream, AzureSettingsOptions settings, string? contentType = null, bool overwrite = false);
        public Task<bool> CheckFileInBlob(string name, AzureSettingsOptions settings);
        public Task<Pageable<BlobItem>?> GetBlobsFromDirectory(string name, AzureSettingsOptions settings);*/
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class AzureBlobService : IAzureBlobService
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<Stream?> GetBlobFileByName(string name, IAzureSettings settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return null;
            }
            var containerClient = settings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = settings.GetBlobClient(containerClient, name);
            if (await blobClient.ExistsAsync())
            {
                return await blobClient.OpenReadAsync();
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBlobImage(string name, IAzureSettings settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return false;
            }
            var containerClient = settings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = settings.GetBlobClient(containerClient, name);
            var result = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        /// <param name="contentType"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task UploadFileStreamToBlob(string name, Stream stream, IAzureSettings settings, string? contentType = null, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return;
            }
            var containerClient = settings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = settings.GetBlobClient(containerClient, name);
            stream.Position = 0;
            var headers = new BlobHttpHeaders { ContentType = contentType };
            blobClient.SetHttpHeaders(headers);
            await blobClient.UploadAsync(stream, overwrite: overwrite);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<bool> CheckFileInBlob(string name, IAzureSettings settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return false;
            }
            var containerClient = settings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = settings.GetBlobClient(containerClient, name);
            var result = await blobClient.ExistsAsync();
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
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<Pageable<BlobItem>?> GetBlobsFromDirectory(string prefix, IAzureSettings settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return null;
            }
            var containerClient = settings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var result = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix);
            return await Task.FromResult(result);

        }

        /*private readonly IAzureSettings _azureSettings;

        public AzureBlobService(IAzureSettings azureSettings)
        {
            _azureSettings = azureSettings;
        }*/

        /*public async Task<Stream?> GetBlobFileByName(string name, AzureSettingsOptions settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return null;
            }
            var containerClient = _azureSettings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = _azureSettings.GetBlobClient(containerClient, name);
            if (await blobClient.ExistsAsync())
            {
                return await blobClient.OpenReadAsync();
            }
            return null;
        }

        public async Task<bool> DeleteBlobImage(string name, AzureSettingsOptions settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return false;
            }
            var containerClient = _azureSettings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = _azureSettings.GetBlobClient(containerClient, name);
            var result = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        public async Task UploadFileStreamToBlob(string name, Stream stream, AzureSettingsOptions settings, string? contentType = null, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return;
            }
            var containerClient = _azureSettings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = _azureSettings.GetBlobClient(containerClient, name);
            stream.Position = 0;
            var headers = new BlobHttpHeaders { ContentType = contentType };
            blobClient.SetHttpHeaders(headers);
            await blobClient.UploadAsync(stream, overwrite: overwrite);
        }

        public async Task<bool> CheckFileInBlob(string name, AzureSettingsOptions settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return false;
            }
            var containerClient = _azureSettings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var blobClient = _azureSettings.GetBlobClient(containerClient, name);
            var result = await blobClient.ExistsAsync();
            if (result != null)
            {
                return result.Value;
            }
            return false;
        }

        public async Task<Pageable<BlobItem>?> GetBlobsFromDirectory(string name, AzureSettingsOptions settings)
        {
            if (string.IsNullOrEmpty(settings.StorageConnectionString) || string.IsNullOrEmpty(settings.ContainerName))
            {
                return null;
            }
            var containerClient = _azureSettings.GetCloudClient(settings.StorageConnectionString, settings.ContainerName);
            var result = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, name);
            return await Task.FromResult(result);

        }*/
    }
}
