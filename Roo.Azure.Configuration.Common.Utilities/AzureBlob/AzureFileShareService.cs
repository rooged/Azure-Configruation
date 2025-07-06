using Azure;
using Azure.Storage.Files.Shares;
using System.Text;

namespace Roo.Azure.Configuration.Common.Utilities.AzureBlob
{
    /// <summary>
    /// Service for using Azure File Share easily.
    /// </summary>
    public class AzureFileShareService
    {
        private readonly ShareClient _client;

        /// <summary>
        /// Initialize <see cref="AzureFileShareService"/> with the variables for <see cref="ShareClient"/>.
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="fileShareName"></param>
        public AzureFileShareService(string storageConnectionString, string fileShareName)
        {
            _client = new ShareClient(storageConnectionString, fileShareName);
        }

        /// <summary>
        /// Check if a File Share directory exists.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <returns></returns>
        public bool DoesShareDirectoryExists(string path)
        {
            try
            {
                var directoy = GetShareDirectoryClient(path);
                return directoy.Exists();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Build a File Share directory with the provided path.
        /// </summary>
        /// <param name="path">Path of the intended directory.</param>
        /// <returns></returns>
        public ShareDirectoryClient? CreateShareDirectory(string path)
        {
            try
            {
                var arrayPath = path.Split('/');
                var buildPath = new StringBuilder();
                ShareDirectoryClient? directory = null;

                for (var i = 0; i < arrayPath.Length; i++)
                {
                    buildPath.Append(arrayPath[i]);
                    directory = GetShareDirectoryClient(buildPath.ToString());
                    directory.CreateIfNotExists();
                    buildPath.Append('/');
                }
                return directory;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShareFileClient GetAllFilesFromShareDirectory(string path)
        {
            var directory = GetShareDirectoryClient(path);
            var files = directory.GetFilesAndDirectories().Where(x => !x.IsDirectory);
            var fileName = files.Select(x => x.Name).FirstOrDefault();
            return directory.GetFileClient(fileName);
        }

        /// <summary>
        /// Delete a file from a File Share directory using the directory path and file name.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileFromShareDirectory(string path, string fileName)
        {
            try
            {
                var directory = GetShareDirectoryClient(path);
                var file = directory.GetFileClient(fileName);
                var result = await file.DeleteIfExistsAsync();
                if (result != null)
                {
                    return result.Value;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Download a file from an Azure File Share directory.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be deleted.</param>
        /// <returns></returns>
        public async Task<Stream?> GetFileFromShareDirectory(string path, string fileName)
        {
            try
            {
                var directory = GetShareDirectoryClient(path);
                var file = directory.GetFileClient(fileName);
                var result = await file.ExistsAsync();
                if (result != null && result.Value)
                {
                    var fileDownloadInfo = await file.DownloadAsync();
                    if (fileDownloadInfo != null)
                    {
                        return fileDownloadInfo.Value.Content;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read a file from an Azure File Share directory.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be read.</param>
        /// <returns></returns>
        public async Task<Stream?> ReadFileFromShareDirectory(string path, string fileName)
        {
            try
            {
                var directory = GetShareDirectoryClient(path);
                var file = directory.GetFileClient(fileName);
                var result = await file.ExistsAsync();
                if (result != null && result.Value)
                {
                    return await file.OpenReadAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Upload a file from an Azure File Share directory.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="fileName">Name of the file to be uploaded.</param>
        /// <param name="stream">Stream of the file to be uploaded.</param>
        /// <returns></returns>
        public async Task<bool> UploadFileToShareDirectory(string path, string fileName, Stream stream)
        {
            try
            {
                var directory = GetShareDirectoryClient(path);
                var directoryExists = await directory.ExistsAsync();
                if (directoryExists == null || !directoryExists.Value)
                {
                    await directory.CreateAsync();
                }
                var file = directory.GetFileClient(fileName);
                await file.CreateAsync(stream.Length);
                var result = await file.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private ShareDirectoryClient GetShareDirectoryClient(string directory)
        {
            return _client.GetDirectoryClient(directory);
        }
    }
}
