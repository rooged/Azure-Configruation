namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Authentication strategies to use when making a request.
    /// </summary>
    public enum AuthenticationStrategy
    {
        /// <summary>
        /// No authentication needed
        /// </summary>
        None = 0,

        /// <summary>
        /// Authentication using OAuth
        /// </summary>
        OAuth = 1,

        /// <summary>
        /// APIM authentication using a certificate
        /// </summary>
        ApimCertificate = 2,

        /// <summary>
        /// APIM authentication using a client secret
        /// </summary>
        ApimClientSecret = 3,

        /// <summary>
        /// Basic authentication. Pass in the token url, headers, encoded content, and auth client name (optional).<br/>
        /// Expiration time in cache will be set to 59 minutes.
        /// </summary>
        Basic = 4,

        /// <summary>
        /// Use if you're handling authentication and only passing the token to the request
        /// </summary>
        PassInToken = 5
    }
}
