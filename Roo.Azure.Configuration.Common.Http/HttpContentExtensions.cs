namespace Roo.Azure.Configuration.Common.Http
{
    /// <summary>
    /// Extension methods for HttpContent.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Read as string safely so a <see cref="NullReferenceException"/> isn't thrown if content is null.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static async Task<string?> SafeReadAsStringAsync(this HttpContent content, string? defaultValue = null)
        {
            return (content != null ? await content.ReadAsStringAsync().ConfigureAwait(false) : null) ?? defaultValue;
        }

        /// <summary>
        /// Read as stream safely so a <see cref="NullReferenceException"/> isn't thrown if content is null.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<Stream?> SafeReadAsStreamAsync(this HttpContent content)
        {
            return content != null ? await content.ReadAsStreamAsync().ConfigureAwait(false) : null;
        }
    }
}
