using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roo.Azure.Configuration.Common.Services;
using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Common.Startup
{
    /// <summary>
    /// Configuration for basic client certificate authorization.
    /// </summary>
    public static class CertificateConfiguration
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
            services.AddSingleton<ICertificateValidationService>(new CertificateValidationService(validCertificateValues));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                //Only loopback proxies are allowed by default.
                //Clear that restriction because forwarders are being enabled for explicit configuration
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            //Set up ASE certificate header
            services.AddCertificateForwarding(options => options.CertificateHeader = ASECERTIFICATEHEADER);

            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(options =>
            {
                options.AllowedCertificateTypes = GetAllowedCertificateTypesFromSetting(config.GetValue<string>(CONFIGNAMEALLOWEDCERTIFICATETYPES) ?? string.Empty);
                options.RevocationFlag = GetRevocationFlagFromSetting(config.GetValue<string>(CONFIGNAMEX509REVOCATIONFLAG) ?? string.Empty);
                options.RevocationMode = GetRevocationModeFromSetting(config.GetValue<string>(CONFIGNAMEX509REVOCATIONMODE) ?? string.Empty);

                options.Events = GetCustomValidationEvent();
            });
        }

        private static Dictionary<string, string> GetValidCertificateValuesFromSetting(IConfiguration config)
        {
            var validThumbprint = new[] { config.GetValue<string>(CONFIGNAMEVALIDTHUMBPRINT) ?? "", config["DefaultThumbprint"] ?? "" }.First();

            var validCertificateValues = new Dictionary<string, string>() { { VALIDTHUMBPRINT, validThumbprint } };

            return validCertificateValues;
        }

        private static CertificateTypes GetAllowedCertificateTypesFromSetting(string setting)
        {
            var map = new Dictionary<string, CertificateTypes>()
            {
                { "all", CertificateTypes.All },
                { "chained", CertificateTypes.Chained },
                { "selfsigned", CertificateTypes.SelfSigned }
            };

            return map.TryGetValue(setting.ToLower(), out var value) ? value : CertificateTypes.Chained;
        }

        private static X509RevocationFlag GetRevocationFlagFromSetting(string setting)
        {
            var map = new Dictionary<string, X509RevocationFlag>()
            {
                { "endcertificateonly", X509RevocationFlag.EndCertificateOnly },
                { "entirechain", X509RevocationFlag.EntireChain },
                { "excluderoot", X509RevocationFlag.ExcludeRoot }
            };

            return map.TryGetValue(setting.ToLower(), out var value) ? value : X509RevocationFlag.ExcludeRoot;
        }

        private static X509RevocationMode GetRevocationModeFromSetting(string setting)
        {
            var map = new Dictionary<string, X509RevocationMode>()
            {
                { "NoCheck", X509RevocationMode.NoCheck },
                { "Offline", X509RevocationMode.Offline },
                { "Online", X509RevocationMode.Online }
            };

            return map.TryGetValue(setting.ToLower(), out var value) ? value : X509RevocationMode.Online;
        }

        public static CertificateAuthenticationEvents GetCustomValidationEvent()
        {
            return new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetService<ICertificateValidationService>();
                    if (validationService != null && validationService.ValidateCertificate(context.ClientCertificate))
                    {
                        context.Success();
                    }
                    else
                    {
                        context.Fail("Invalid certificate");
                    }
                    return Task.CompletedTask;
                }
            };
        }
    }
}
