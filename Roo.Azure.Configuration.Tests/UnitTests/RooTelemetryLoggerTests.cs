using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class RooTelemetryLoggerTests
    {
        //Common
        private string name = "name";
        private Dictionary<string, string> properties = new() { { "keyProperties", "valueProperties" } };
        private Dictionary<string, double> metrics = new() { { "keyMetrics", 1 } };
        private TelemetryData data = new()
        {
            User = new()
            {
                Id = "id",
                AccountId = "accountId",
                UserAgent = "userAgent",
                AuthenticatedUserId = "authenticatedUserId"
            },
            Session = new()
            {
                Id = "id",
                IsFirst = true
            },
            Device = new()
            {
                Id = "id",
                Type = "type",
                OperatingSystem = "operatingSystem",
                OemName = "oemName",
                Model = "model"
            },
            Component = new()
            {
                Version = "version"
            },
            Cloud = new()
            {
                RoleName = "roleName",
                RoleInstance = "roleInstance"
            },
            Operation = new()
            {
                Id = "id",
                ParentId = "parentId",
                SyntheticSource = "syntheticSource",
                Name = "name"
            },
            Location = new()
            {
                Ip = "ip"
            }
        };

        [Test]
        public void TelemetryDataRemains_VerifyLog()
        {
            //Arrange
            var telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            var service = new RooTelemetryLogger(telemetryClient);
            service.TelemetryData = data;

            //Act
            service.TrackEvent(name, properties, metrics);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(service.TelemetryData, Is.Not.Null);
                Assert.That(service.TelemetryData, Is.EqualTo(data));
                Assert.That(telemetryClient.Context.User.Id, Is.EqualTo(data.User?.Id));
            });
        }

        [Test]
        public void TelemetryDataPassedIn_VerifyLog()
        {
            //Arrange
            var telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            var service = new RooTelemetryLogger(telemetryClient);

            //Act
            service.TrackEvent(name, properties, metrics, data);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(service.TelemetryData, Is.Not.Null);
                Assert.That(service.TelemetryData, Is.EqualTo(data));
                Assert.That(telemetryClient.Context.User.Id, Is.EqualTo(data.User?.Id));
            });
        }

        [Test]
        public void TelemetryDataFlushed_VerifyLog()
        {
            //Arrange
            var telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            var service = new RooTelemetryLogger(telemetryClient);

            //Act
            service.TrackEvent(name, properties, metrics, data, true);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(service.TelemetryData, Is.Null);
                Assert.That(service.TelemetryData, Is.Not.EqualTo(data));
            });
        }
    }
}