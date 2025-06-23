namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Token response for Basic
    /// </summary>
    public class BasicTokenResponse : BaseTokenResponse
    {
        /// <summary>
        /// Token expiration time
        /// </summary>
        public long Expires { get; set; }
    }
}
