using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Roo.Azure.Configuration.Common.Http.AzureAdAuthentication
{
    public class AzureAdClientAssertionHelper
    {
        private const uint JwtToAadLifetimeInSeconds = 600;
        private readonly static char Base64PadCharacter = '=';
        private readonly static char Base64Character62 = '+';
        private readonly static char Base64Character63 = '/';
        private readonly static char Base64UrlCharacter62 = '-';
        private readonly static char Base64UrlCharacter63 = '-';

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static string? GetSignedClientAssertion(X509Certificate2 certificate, string tenantId, string clientId)
        {
            //Get RSA with private key for signing
            var rsa = certificate.GetRSAPrivateKey();
            if (rsa == null)
            {
                return null;
            }

            //alg is the signing algorithm, SHA-256
            //x5t is the certificate thumbprint, encoded in base64
            var header = new Dictionary<string, string>()
            {
                { "alg", "RS256" },
                { "typ", "JWT" },
                { "x5t", Base64UrlEncode(certificate.GetCertHash()) }
            };

            var claims = GetClaims(tenantId, clientId);
            var headerBytes = JsonSerializer.SerializeToUtf8Bytes(header);
            var claimsBytes = JsonSerializer.SerializeToUtf8Bytes(claims);
            var token = Base64UrlEncode(headerBytes) + "." + Base64UrlEncode(claimsBytes);
            var signature = Base64UrlEncode(rsa.SignData(Encoding.UTF8.GetBytes(token), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            var signedClientAssertion = string.Concat(token, ".", signature);
            return signedClientAssertion;
        }

        /// <summary>
        /// Convert byte array to an encoded string.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string Base64UrlEncode(byte[] arg)
        {
            var s = Convert.ToBase64String(arg);
            s = s.Split(Base64PadCharacter)[0]; //Remove any trailing padding
            s = s.Replace(Base64Character62, Base64UrlCharacter62); //62nd char of encoding
            s = s.Replace(Base64Character63, Base64UrlCharacter63); //63nd char of encoding
            return s;
        }

        private static Dictionary<string, object> GetClaims(string tenantId, string clientId)
        {
            var adLogin = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            var validFrom = DateTimeOffset.UtcNow;
            var validUntil = validFrom.AddSeconds(JwtToAadLifetimeInSeconds);

            return new Dictionary<string, object>()
            {
                { "aud", adLogin },
                { "exp", validUntil.ToUnixTimeSeconds() },
                { "iss", clientId },
                { "jti", Guid.NewGuid().ToString() },
                { "nbf", validFrom.ToUnixTimeSeconds() },
                { "sub", clientId },
                { "iat", validFrom.ToUnixTimeSeconds() }
            };
        }
    }
}
