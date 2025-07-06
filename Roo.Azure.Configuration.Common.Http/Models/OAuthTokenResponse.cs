namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Token response for OAuth
    /// </summary>
    public class OAuthTokenResponse : BaseTokenResponse
    {
        /// <summary>
        /// Time till token expires
        /// </summary>
        public int Expires_In { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
