using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Certificate authentication methods
    /// </summary>
    public interface ICertificateValidationService
    {
        /// <summary>
        /// Valid thumbprint response.
        /// </summary>
        public const string VALIDTHUMBPRINT = "validThumbPrint";

        /// <summary>
        /// Validates the certificate against already authorized certs in the applications.<br/>
        /// Does NOT do root chain validation or check dates.
        /// </summary>
        /// <param name="clientCertificate">Client cert to be validated.</param>
        /// <returns>Whether certificate is valid or not.</returns>
        bool ValidateCertificate(X509Certificate2 clientCertificate);
    }

    /// <summary>
    /// Implementation of <see cref="ICertificateValidationService"/>.
    /// </summary>
    public class CertificateValidationService : ICertificateValidationService
    {
        private readonly string thumbprintValues;

        /// <summary>
        /// Initialize <see cref="CertificateValidationService"/>.
        /// </summary>
        /// <param name="validCertificateValues"></param>
        public CertificateValidationService(Dictionary<string, string> validCertificateValues)
        {
            thumbprintValues = validCertificateValues[ICertificateValidationService.VALIDTHUMBPRINT];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="clientCertificate"></param>
        /// <returns></returns>
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            var isValidCertificate = clientCertificate.Verify();

            var isThumbprintMatch = string.Equals(clientCertificate.Thumbprint, thumbprintValues, StringComparison.OrdinalIgnoreCase);

            return isValidCertificate && isThumbprintMatch;
        }
    }
}
