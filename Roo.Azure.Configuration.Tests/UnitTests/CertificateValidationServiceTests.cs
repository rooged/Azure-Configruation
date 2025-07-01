using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class CertificateValidationServiceTests
    {
        //Common
        string validThumbprint = "validThumbPrint";

        [SetUp]
        public void Setup() { }

        [Test]
        public void ValidateCertificateFailInvalid_Verify()
        {
            //Arrange
            var cert = CreateSelfSignedCertificate();
            var service = new CertificateValidationService(new Dictionary<string, string>
            {
                { validThumbprint, cert.Thumbprint }
            });

            //Act
            var result = service.ValidateCertificate(cert);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(false));
            });
        }

        private static X509Certificate2 CreateSelfSignedCertificate()
        {
            var ecdsa = ECDsa.Create();
            var request = new CertificateRequest("cn=test", ecdsa, HashAlgorithmName.SHA256);
            return request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
        }
    }
}