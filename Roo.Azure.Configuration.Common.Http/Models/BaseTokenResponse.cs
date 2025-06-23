namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Base class for token response models
    /// </summary>
    public class BaseTokenResponse
    {
        /// <summary>
        /// Authenticated token
        /// </summary>
        public string? Access_Token { get; set; }

        /// <summary>
        /// Type of token
        /// </summary>
        public string? Token_Type { get; set; }
    }
}
