using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.Text;

namespace Roo.Azure.Configuration.Common.Utilities.Storage
{
    /// <summary>
    /// Service for Azure File Share to retrieve files, upload files, and manage directory's.
    /// </summary>
    public interface IFileShareService
    {
        /// <summary>
        /// File Share clients cache. Any client created using the service is cached here and can be retrieved using the File Share name.
        /// </summary>
        public Dictionary<string, ShareClient> Clients { get; }

        /// <summary>
        /// Create a new Share Client. Cache's the client and can be retrieved using the fileShareName.
        /// </summary>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <returns></returns>
        public ShareClient CreateFileShareClient(string storageConnectionString, string fileShareName);

        /// <summary>
        /// Check if a File Share directory exists.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DirectoryExists(ShareClient client, string path, CancellationToken? token = null);

        /// <summary>
        /// Check if a File Share directory exists. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DirectoryExists(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="DirectoryExists(ShareClient, string, CancellationToken?)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> DirectoryExistsAsync(ShareClient client, string path, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="DirectoryExists(string, string, string?, CancellationToken?)"/>
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> DirectoryExistsAsync(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Build a File Share directory with the provided path.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the intended directory.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareDirectoryClient? CreateDirectory(ShareClient client, string path, CancellationToken? token = null);

        /// <summary>
        /// Build a File Share directory with the provided path. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the intended directory.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareDirectoryClient? CreateDirectory(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="CreateDirectory(ShareClient, string, CancellationToken?)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the intended directory.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ShareDirectoryClient?> CreateDirectoryAsync(ShareClient client, string path, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="CreateDirectory(string, string, string?, CancellationToken?)"/>
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the intended directory.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ShareDirectoryClient?> CreateDirectoryAsync(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Get all files from a File Share directory.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path in directory to retrieve all files from.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareFileClient GetAllFilesFromDirectory(ShareClient client, string path, CancellationToken? token = null);

        /// <summary>
        /// Get all files from a File Share directory. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path in directory to retrieve all files from.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareFileClient GetAllFilesFromDirectory(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Delete a file from a File Share directory using the directory path and file name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null);

        /// <summary>
        /// Delete a file from a File Share directory using the directory path and file name. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="DeleteFileFromDirectory(ShareClient, string, string, CancellationToken?)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> DeleteFileFromDirectoryAsync(ShareClient client, string path, string fileName, CancellationToken? token = null);

        /// <summary>
        /// <inheritdoc cref="DeleteFileFromDirectory(string, string, string, string?, CancellationToken?)"/>
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> DeleteFileFromDirectoryAsync(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Download a file from a File Share directory.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Stream?> GetFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null);

        /// <summary>
        /// Download a file from a File Share directory. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Stream?> GetFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Read a file from a File Share directory.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be read.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Stream?> ReadFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null);

        /// <summary>
        /// Read a file from a File Share directory. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be read.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Stream?> ReadFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null);

        /// <summary>
        /// Upload a file to a File Share directory.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be uploaded.</param>
        /// <param name="stream">Stream of the file to be uploaded.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ShareFileUploadInfo> UploadFileToDirectory(ShareClient client, string path, string fileName, Stream stream, CancellationToken? token = null);

        /// <summary>
        /// Upload a file to a File Share directory. If a client has already been cached you only need to pass in the File Share name, otherwise the connection string is required to create a new client.
        /// </summary>
        /// <param name="fileShareName">Name of File Share.</param>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be uploaded.</param>
        /// <param name="stream">Stream of the file to be uploaded.</param>
        /// <param name="storageConnectionString">Connection string to File Share.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ShareFileUploadInfo?> UploadFileToDirectory(string fileShareName, string path, string fileName, Stream stream, string? storageConnectionString = null, CancellationToken? token = null);
    }

    /// <summary>
    /// Service for Azure File Share to retrieve files, upload files, and manage directory's.
    /// </summary>
    public class FileShareService : IFileShareService
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Dictionary<string, ShareClient> Clients { get; set; }

        private readonly Func<string, string, ShareClient> _clientFactory;

        public FileShareService(Func<string, string, ShareClient>? clientFactory = null)
        {
            Clients = new();
            _clientFactory = clientFactory ?? ((storageConnectionString, fileShareName) =>
            {
                var client = new FileShareClientAdapter().CreateClient(storageConnectionString, fileShareName);
                Clients.Add(fileShareName, client);
                return client;
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="fileShareName"></param>
        /// <returns></returns>
        public ShareClient CreateFileShareClient(string storageConnectionString, string fileShareName)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                client = _clientFactory(storageConnectionString, fileShareName);
                Clients.TryAdd(fileShareName, client);
            }
            return client;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DirectoryExists(ShareClient client, string path, CancellationToken? token = null)
        {
            if (token != null)
            {
                return client.GetDirectoryClient(path).Exists((CancellationToken)token).Value;
            }
            else
            {
                return client.GetDirectoryClient(path).Exists().Value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DirectoryExists(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return false;
                }
            }
            if (token != null)
            {
                return client.GetDirectoryClient(path).Exists((CancellationToken)token).Value;
            }
            else
            {
                return client.GetDirectoryClient(path).Exists().Value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DirectoryExistsAsync(ShareClient client, string path, CancellationToken? token = null)
        {
            if (token != null)
            {
                return (await client.GetDirectoryClient(path).ExistsAsync((CancellationToken)token)).Value;
            }
            else
            {
                return (await client.GetDirectoryClient(path).ExistsAsync()).Value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DirectoryExistsAsync(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return false;
                }
            }
            if (token != null)
            {
                return (await client.GetDirectoryClient(path).ExistsAsync((CancellationToken)token)).Value;
            }
            else
            {
                return (await client.GetDirectoryClient(path).ExistsAsync()).Value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareDirectoryClient? CreateDirectory(ShareClient client, string path, CancellationToken? token = null)
        {
            var arrayPath = path.Split('/');
            var buildPath = new StringBuilder();
            ShareDirectoryClient? directory = null;
            for (var i = 0; i < arrayPath.Length; i++)
            {
                buildPath.Append(arrayPath[i]);
                directory = client.GetDirectoryClient(buildPath.ToString());
                if (token != null)
                {
                    directory.CreateIfNotExists(null, (CancellationToken)token);
                }
                else
                {
                    directory.CreateIfNotExists();
                }
                buildPath.Append('/');
            }
            return directory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareDirectoryClient? CreateDirectory(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return null;
                }
            }
            var arrayPath = path.Split('/');
            var buildPath = new StringBuilder();
            ShareDirectoryClient? directory = null;
            for (var i = 0; i < arrayPath.Length; i++)
            {
                buildPath.Append(arrayPath[i]);
                directory = client.GetDirectoryClient(buildPath.ToString());
                if (token != null)
                {
                    directory.CreateIfNotExists(null, (CancellationToken)token);
                }
                else
                {
                    directory.CreateIfNotExists();
                }
                buildPath.Append('/');
            }
            return directory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ShareDirectoryClient?> CreateDirectoryAsync(ShareClient client, string path, CancellationToken? token = null)
        {
            var arrayPath = path.Split('/');
            var buildPath = new StringBuilder();
            ShareDirectoryClient? directory = null;
            for (var i = 0; i < arrayPath.Length; i++)
            {
                buildPath.Append(arrayPath[i]);
                directory = client.GetDirectoryClient(buildPath.ToString());
                if (token != null)
                {
                    await directory.CreateIfNotExistsAsync(null, (CancellationToken)token);
                }
                else
                {
                    await directory.CreateIfNotExistsAsync();
                }
                buildPath.Append('/');
            }
            return directory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ShareDirectoryClient?> CreateDirectoryAsync(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return null;
                }
            }
            var arrayPath = path.Split('/');
            var buildPath = new StringBuilder();
            ShareDirectoryClient? directory = null;
            for (var i = 0; i < arrayPath.Length; i++)
            {
                buildPath.Append(arrayPath[i]);
                directory = client.GetDirectoryClient(buildPath.ToString());
                if (token != null)
                {
                    await directory.CreateIfNotExistsAsync(null, (CancellationToken)token);
                }
                else
                {
                    await directory.CreateIfNotExistsAsync();
                }
                buildPath.Append('/');
            }
            return directory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareFileClient GetAllFilesFromDirectory(ShareClient client, string path, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            IEnumerable<ShareFileItem> files;
            if (token != null)
            {
                files = directory.GetFilesAndDirectories(cancellationToken: (CancellationToken)token).Where(x => !x.IsDirectory);
            }
            else
            {
                files = directory.GetFilesAndDirectories().Where(x => !x.IsDirectory);
            }
            var fileName = files.Select(x => x.Name).FirstOrDefault();
            return directory.GetFileClient(fileName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ShareFileClient GetAllFilesFromDirectory(string fileShareName, string path, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    throw new KeyNotFoundException($"{fileShareName} does not exist in the client cache and could not create a new client without the storage connection string.");
                }
            }
            var directory = client.GetDirectoryClient(path);
            IEnumerable<ShareFileItem> files;
            if (token != null)
            {
                files = directory.GetFilesAndDirectories(cancellationToken: (CancellationToken)token).Where(x => !x.IsDirectory);
            }
            else
            {
                files = directory.GetFilesAndDirectories().Where(x => !x.IsDirectory);
            }
            var fileName = files.Select(x => x.Name).FirstOrDefault();
            return directory.GetFileClient(fileName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = file.DeleteIfExists(cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = file.DeleteIfExists();
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
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return false;
                }
            }
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = file.DeleteIfExists(cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = file.DeleteIfExists();
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
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileFromDirectoryAsync(ShareClient client, string path, string fileName, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.DeleteIfExistsAsync(cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = await file.DeleteIfExistsAsync();
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
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileFromDirectoryAsync(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return false;
                }
            }
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.DeleteIfExistsAsync(cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = await file.DeleteIfExistsAsync();
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
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Stream?> GetFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.ExistsAsync((CancellationToken)token);
            }
            else
            {
                result = await file.ExistsAsync();
            }
            if (result != null && result.Value)
            {
                Response<ShareFileDownloadInfo> fileDownloadInfo;
                if (token != null)
                {
                    fileDownloadInfo = await file.DownloadAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    fileDownloadInfo = await file.DownloadAsync();
                }
                if (fileDownloadInfo != null)
                {
                    return fileDownloadInfo.Value.Content;
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Stream?> GetFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return null;
                }
            }
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.ExistsAsync((CancellationToken)token);
            }
            else
            {
                result = await file.ExistsAsync();
            }
            if (result != null && result.Value)
            {
                Response<ShareFileDownloadInfo> fileDownloadInfo;
                if (token != null)
                {
                    fileDownloadInfo = await file.DownloadAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    fileDownloadInfo = await file.DownloadAsync();
                }
                if (fileDownloadInfo != null)
                {
                    return fileDownloadInfo.Value.Content;
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Stream?> ReadFileFromDirectory(ShareClient client, string path, string fileName, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.ExistsAsync((CancellationToken)token);
            }
            else
            {
                result = await file.ExistsAsync();
            }
            if (result != null && result.Value)
            {
                if (token != null)
                {
                    return await file.OpenReadAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    return await file.OpenReadAsync();
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Stream?> ReadFileFromDirectory(string fileShareName, string path, string fileName, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return null;
                }
            }
            var directory = client.GetDirectoryClient(path);
            var file = directory.GetFileClient(fileName);
            Response<bool>? result;
            if (token != null)
            {
                result = await file.ExistsAsync((CancellationToken)token);
            }
            else
            {
                result = await file.ExistsAsync();
            }
            if (result != null && result.Value)
            {
                if (token != null)
                {
                    return await file.OpenReadAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    return await file.OpenReadAsync();
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ShareFileUploadInfo> UploadFileToDirectory(ShareClient client, string path, string fileName, Stream stream, CancellationToken? token = null)
        {
            var directory = client.GetDirectoryClient(path);
            Response<bool>? directoryExists;
            if (token != null)
            {
                directoryExists = await directory.ExistsAsync((CancellationToken)token);
            }
            else
            {
                directoryExists = await directory.ExistsAsync();
            }
            if (directoryExists == null || !directoryExists.Value)
            {
                if (token != null)
                {
                    await directory.CreateAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    await directory.CreateAsync();
                }
            }
            var file = directory.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            Response<ShareFileUploadInfo> result;
            if (token != null)
            {
                result = await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream, cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }
            return result.Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileShareName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <param name="storageConnectionString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ShareFileUploadInfo?> UploadFileToDirectory(string fileShareName, string path, string fileName, Stream stream, string? storageConnectionString = null, CancellationToken? token = null)
        {
            if (!Clients.TryGetValue(fileShareName, out var client))
            {
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    client = CreateFileShareClient(storageConnectionString, fileShareName);
                }
                else
                {
                    return null;
                }
            }
            var directory = client.GetDirectoryClient(path);
            Response<bool>? directoryExists;
            if (token != null)
            {
                directoryExists = await directory.ExistsAsync((CancellationToken)token);
            }
            else
            {
                directoryExists = await directory.ExistsAsync();
            }
            if (directoryExists == null || !directoryExists.Value)
            {
                if (token != null)
                {
                    await directory.CreateAsync(cancellationToken: (CancellationToken)token);
                }
                else
                {
                    await directory.CreateAsync();
                }
            }
            var file = directory.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            Response<ShareFileUploadInfo> result;
            if (token != null)
            {
                result = await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream, cancellationToken: (CancellationToken)token);
            }
            else
            {
                result = await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }
            return result.Value;
        }
    }

    /// <summary>
    /// <inheritdoc cref="IFileShareClient"/>
    /// </summary>
    public class FileShareClientAdapter : IFileShareClient
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="fileShareName"></param>
        /// <returns></returns>
        public ShareClient CreateClient(string storageConnectionString, string fileShareName)
        {
            return new ShareClient(storageConnectionString, fileShareName);
        }
    }

    /// <summary>
    /// FileShareClient wrapper to simplify unit testing and decouple code.
    /// </summary>
    public interface IFileShareClient
    {
        /// <summary>
        /// Creates a <see cref="FileShareClientAdapter"/> instance that creates a new File Share client.
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="fileShareName"></param>
        /// <returns></returns>
        ShareClient CreateClient(string storageConnectionString, string fileShareName);
    }
}
