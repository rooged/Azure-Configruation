using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Http.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Common.Http.AzureAdAuthentication
{
    /// <summary>
    /// Authentication using Azure Active Directory.
    /// </summary>
    public interface IAzureAdClientAssertion
    {
        /// <summary>
        /// Get Azure Active Directory token using certificate authentication. Certificate must have a RSA private key.
        /// </summary>
        /// <param name="tenantId">Azure Ad Tenant Id.</param>
        /// <param name="clientId">Azure Ad Client Id.</param>
        /// <param name="scope">Scope of application, not the Azure AD subscription.</param>
        /// <param name="certificate">Certificate to authenticate against in Azure Active Directory. Must have a RSA private key.</param>
        /// <param name="tokenName">Token name stored in cache.</param>
        /// <returns></returns>
        public Task<string?> GetTokenAsync(string tenantId, string clientId, string scope, X509Certificate2 certificate, string? tokenName = null);

        /// <summary>
        /// Get Azure Active Directory token using client secret authentication.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tokenName"></param>
        /// <returns></returns>
        public Task<string?> GetTokenAsync(string tenantId, string clientId, string scope, string clientSecret, string? tokenName = null);

        /// <summary>
        /// Get a certificate stored in Azure Key Vault.
        /// </summary>
        /// <param name="certName">Name of certificate stored in Key Vault.</param>
        /// <param name="keyVaultUri">Uri of Key Vault.</param>
        /// <param name="token">Credential with authorization to the Key Vault, thie credential must have Certificate Get permission.</param>
        /// <returns></returns>
        public Task<X509Certificate2?> GetCertificateFromKeyVault(string certName, string keyVaultUri, TokenCredential? token = null);

        /// <summary>
        /// Convert a string to a certificate with a password.
        /// </summary>
        /// <param name="certString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public X509Certificate2? GenerateCertificateFromString(string certString, string password);
    }

    /// <summary>
    /// Authentication using Azure Active Directory.
    /// </summary>
    public class AzureAdClientAssertion : IAzureAdClientAssertion
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="cache"></param>
        public AzureAdClientAssertion(IMemoryCache cache)
        {
            _cache = cache;
        }

        private static string GenerateClientAssertion(string clientId, string tenantId, X509Certificate2 certificate)
        {
            //Create signing credentials
            var signingCredentials = new X509SigningCredentials(certificate, SecurityAlgorithms.RsaSha256);

            //Define token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "https://login.microsoftonline.com/" + tenantId + "/v2.0",
                Issuer = clientId,
                Subject = new System.Security.Claims.ClaimsIdentity([new Claim("sub", clientId)]),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = signingCredentials
            };

            //Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Create token
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            //Write token as a string
            var clientAssertion = tokenHandler.WriteToken(securityToken);
            return clientAssertion;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="certificate"></param>
        /// <param name="tokenName"></param>
        /// <returns></returns>
        public async Task<string?> GetTokenAsync(string tenantId, string clientId, string scope, X509Certificate2 certificate, string? tokenName = null)
        {
            //Check if token is already in cache
            var token = !string.IsNullOrEmpty(tokenName) ? _cache.Get<string>(tokenName) : null;

            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            var clientAssertion = GenerateClientAssertion(clientId, tenantId, certificate);

            var requestContent = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_assertion", clientAssertion),
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", scope),
            ]);

            return await RequestToken(tenantId, requestContent, tokenName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tokenName"></param>
        /// <returns></returns>
        public async Task<string?> GetTokenAsync(string tenantId, string clientId, string scope, string clientSecret, string? tokenName = null)
        {
            //Check if token is already in cache
            var token = !string.IsNullOrEmpty(tokenName) ? _cache.Get<string>(tokenName) : null;

            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            var requestContent = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", scope),
            ]);

            return await RequestToken(tenantId, requestContent, tokenName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="certName"></param>
        /// <param name="keyVaultUri"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<X509Certificate2?> GetCertificateFromKeyVault(string certName, string keyVaultUri, TokenCredential? token = null)
        {
            var client = new CertificateClient(new Uri(keyVaultUri), token ?? new DefaultAzureCredential());
            var download = await client.DownloadCertificateAsync(certName);
            if (download != null)
            {
                return download.Value;
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="certString"></param>
        /// <returns></returns>
        public X509Certificate2? GenerateCertificateFromString(string certString, string password)
        {
            var certBytes = Convert.FromBase64String(certString);
            var cert = new X509Certificate2(certBytes, password);
            return cert;
        }

        private async Task<string?> RequestToken(string tenantId, FormUrlEncodedContent requestContent, string? tokenName = null)
        {
            using (HttpClient httpClient = new())
            {
                //Create request URL
                var endpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

                //Send request
                var response = await httpClient.PostAsync(endpoint, requestContent);

                //Ensure response is successful
                response.EnsureSuccessStatusCode();

                //Read response content
                var responseContent = await response.Content.ReadAsStringAsync();

                //Extract token from response
                var tokenObject = JsonConvert.DeserializeObject<AzureAdTokenResponse>(responseContent);
                var token = tokenObject?.AccessToken;

                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(tokenName))
                {
                    _cache.Set(tokenName, token, DateTimeOffset.UtcNow.AddMinutes(58));
                }
                return token;
            }
        }
    }
}
