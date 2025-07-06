namespace Roo.Azure.Configuration.Common.Utilities.AzureBlob
{
    /// <summary>
    /// Settings for Azure File Share.
    /// </summary>
    public class AzureFileShareSettings
    {
        /// <summary>
        /// Connection string for storage.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Name of File Share.
        /// </summary>
        public string FileShare { get; set; }

        /// <summary>
        /// Initialize <see cref="AzureFileShareSettings"/> with properties set.
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="fileShare"></param>
        public AzureFileShareSettings(string storageConnectionString, string fileShare)
        {
            StorageConnectionString = storageConnectionString;
            FileShare = fileShare;
        }
    }
}
