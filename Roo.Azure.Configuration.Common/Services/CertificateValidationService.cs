using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Configuration for basic client certificate authorization.
    /// </summary>
    public static class CertificateValidationService
    {
        private const string ASECERTIFICATEHEADER = "X-ARR-ClientCert";
        private const string CONFIGNAMEALLOWEDCERTIFICATETYPES = "CERTIFICATE_VALIDATION_ALLOWED_CERTIFICATE_TYPES";
        private const string CONFIGNAMEX509REVOCATIONFLAG = "CERTIFICATE_VALIDATION_X509_REVOCATION_FLAGS";
        private const string CONFIGNAMEX509REVOCATIONMODE = "CERTIFICATE_VALIDATION_X509_REVOCATION_MODE";
        private const string CONFIGNAMEVALIDTHUMBPRINT = "VALID_THUMBPRINT";
        private const string VALIDTHUMBPRINT = "validThumbPrint";
        
        /// <summary>
        /// Sets up client certificate validaiton.
        /// </summary>
        /// <param name="services"></param>
        public static void AddCertificateValidation(this IServiceCollection services)
        {
            using var provider = services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();

            if (config == null)
            {
                return;
            }

            var validCertificateValues = GetValidCertificateValuesFromSetting(config);
            //services.AddSingleton
        }

        private static Dictionary<string, string> GetValidCertificateValuesFromSetting(IConfiguration config)
        {
            var validThumbprint = new[] { config.GetValue<string>(CONFIGNAMEVALIDTHUMBPRINT) ?? "", config["DefaultThumbprint"] ?? "" }.First();

            var validCertificateValues = new Dictionary<string, string>() { { VALIDTHUMBPRINT, validThumbprint } };

            return validCertificateValues;
        }
    }
}
