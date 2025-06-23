using Azure.Core;
using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Common.Http.Models
{
    /// <summary>
    /// Required for APIM and OAuth authentication platforms.<br/>
    /// Properties state which authentication strategy requires them.<br/>
    /// If no strategy is listed then it's used by all.
    /// </summary>
    public class AuthenticationInfo
    {
        /// <summary>
        /// Required.
        /// Authentication strategy to get token; OAuth, APIM certificate, APIM client secret, or basic.<br/>
        /// </summary>
        public AuthenticationStrategy AuthenticationStrategy { get; set; } = 0;

        /// <summary>
        /// Login id used for all authentication strategies:<br/>
        /// OAuth: Username<br/>
        /// ApimCertificate: Azure Active Directory client-id<br/>
        /// ApimClientSecret: Azure Active Directory client-id
        /// </summary>
        public string? LoginId { get; set; } = null;

        /// <summary>
        /// Password for authentication<br/>
        /// Used by authentication strategy: OAuth
        /// </summary>
        public string? Password { get; set; } = null;

        /// <summary>
        /// URL to request a token from. If authentication client base URL isn't configured in startup you must pass the URL including the base, otherwise only pass the relative.<br/>
        /// Used by authentication strategy: OAuth and Basic
        /// </summary>
        public string? TokenUrl { get; set; } = null;

        /// <summary>
        /// Name of client to use for authentication if a client is configured for it in startup.<br/>
        /// Used by authentication strategy: OAuth and Basic
        /// </summary>
        public string? AuthenticationHttpClientName { get; set; } = null;

        /// <summary>
        /// Azure APIM scope<br/>
        /// Used by authentication strategy: ApimCertificate and ApimClientSecret
        /// </summary>
        public string? ApimScope { get; set; } = null;

        /// <summary>
        /// Azure APIM subscription key<br/>
        /// Used by authentication strategy: ApimCertificate and ApimClientSecret
        /// </summary>
        public string? ApimSubscriptionKey { get; set; } = null;

        /// <summary>
        /// Azure APIM tenant id<br/>
        /// Used by authentication strategy: ApimCertificate and ApimClientSecret
        /// </summary>
        public string? ApimTenantId { get; set; } = null;

        /// <summary>
        /// Name of certificate stored in Azure Key Vault for APIM authentication<br/>
        /// Used by authentication strategy: ApimCertificate (via Azure Key Vault)
        /// </summary>
        public string? ApimCertificateName { get; set; } = null;

        /// <summary>
        /// Uri of Azure Key Vault that stores the certificate for APIM authentication<br/>
        /// Used by authentication strategy: ApimCertificate (via Azure Key Vault)
        /// </summary>
        public string? ApimAzureKeyVaultUri { get; set; } = null;

        /// <summary>
        /// Token credential with Certificate Read access to the Azure Key Vault<br/>
        /// Used by authentication strategy: ApimCertificate (via Azure Key Vault)
        /// </summary>
        public TokenCredential? ApimAzureToken { get; set; } = null;

        /// <summary>
        /// Certificate to use for APIM authentication if the certificate isn't stored in Azure Key Vault<br/>
        /// Used by authentication strategy: ApimCertificate (via direct certificate)
        /// </summary>
        public X509Certificate2? ApimCertificate { get; set; } = null;

        /// <summary>
        /// Certificate as a string with RSA password to use for APIM authentication if the certificate isn't stored in Azure Key Vault<br/>
        /// Used by authentication strategy: ApimCertificate (via generating certificate)
        /// </summary>
        public (string Certificate, string Password)? ApimCertificateStringAndPassword { get; set; } = null;

        /// <summary>
        /// APIM client secret for APIM authentication<br/>
        /// Used by authentication strategy: ApimClientSecret
        /// </summary>
        public string? ApimClientSecret { get; set; } = null;

        /// <summary>
        /// Encoded content to send with the authentication request<br/>
        /// Used by authentication strategy: Basic
        /// </summary>
        public List<(string Name, string Value)>? BasicEncodedContent { get; set; } = null;

        /// <summary>
        /// APIM client secret for APIM authentication<br/>
        /// Used by authentication strategy: PassInToken
        /// </summary>
        public string? Token { get; set; } = null;

        /// <summary>
        /// Name for storing token in the cache, overrides HttpClientName
        /// </summary>
        public string? TokenName { get; set; }

        /// <summary>
        /// Force new token generation, will delete cached token and create a new one
        /// </summary>
        public bool ForceToken { get; set; } = false;

        /// <summary>
        /// Add additional headers to the HTTP request. If a header is needed in both the request and authentication, add it to both Lists.
        /// </summary>
        public List<(string Name, string Value)>? AdditionalRequestHeaders { get; set; } = null;

        /// <summary>
        /// Add additional headers required for specific authentication strategies.If a header is needed in both the request and authentication, add it to both Lists.<br/>
        /// Can be used by authentication strategy: OAuth and Basic
        /// </summary>
        public List<(string Name, string Value)>? AuthenticationHeaders { get; set; } = null;
    }
}
