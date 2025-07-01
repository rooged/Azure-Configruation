using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Middlewares;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Telemetry;
using System.Security.Claims;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class TelemetryInitializerTests
    {
        //Common
        string sessionId = "sessionId";
        string transactionId = "transactionId";
        string channelId = "channelId";
        string loginId = "loginId";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InitializeCloudRoleNameTelemetry_Verify()
        {
            //Arrange
            var service = new CloudRoleNameTelemetryInitializer("cloudRoleName");
            var telemetry = new Mock<ITelemetry>();
            var context = new TelemetryContext();
            telemetry.Setup(x => x.Context).Returns(context);

            //Act
            service.Initialize(telemetry.Object);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(telemetry.Object.Context.Cloud.RoleName, Is.EqualTo("cloudRoleName"));
            });
        }

        [Test]
        public void InitializeWithoutHttpContext_Verify()
        {
            //Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var configuration = new Mock<IConfiguration>();
            var headerService = new Mock<IHeaderService>();
            var service = new TelemetryInitializer(httpContextAccessor.Object, configuration.Object, headerService.Object);
            var telemetry = new Mock<ITelemetry>();
            var context = new TelemetryContext();
            telemetry.Setup(x => x.Context).Returns(context);

            //Act
            service.Initialize(telemetry.Object);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.SessionIdHeaderName, out var sessionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.TransactionIdHeaderName, out var transactionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.ChannelIdHeaderName, out var channelIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.UserInfoLoginId, out var userInfoLoginIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue("HttpContextError", out var httpContextError);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdValue, Is.Not.Null);
                Assert.That(sessionIdValue?.Length, Is.EqualTo(36));
                Assert.That(transactionIdValue, Is.Not.Null);
                Assert.That(transactionIdValue?.Length, Is.EqualTo(32));
                Assert.That(channelIdValue, Is.Null);
                Assert.That(userInfoLoginIdValue, Is.Null);
                Assert.That(httpContextError, Is.Not.Null);
                Assert.That(httpContextError, Is.EqualTo("Request made without HttpContext available for telemetry to consume. Custom headers unable to be set correctly, used default values."));
            });
        }

        [Test]
        public void InitializeWithHttpContextWithoutHeaders_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            Mock<ISession> session = new();
            var sessionStorage = new Dictionary<string, byte[]>();
            session.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>())).Callback<string, byte[]>((key, value) => sessionStorage[key] = value);
            session.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]?>.IsAny)).Returns((string key, out byte[]? value) =>
            {
                if (sessionStorage.TryGetValue(key, out var storedValue))
                {
                    value = storedValue;
                    return true;
                }
                value = null;
                return false;
            });
            session.Object.SetString(Constants.SessionId, sessionId);
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, loginId)
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x[Constants.ChannelId]).Returns(channelId);
            var headerService = new Mock<IHeaderService>();
            var service = new TelemetryInitializer(httpContextAccessor.Object, configuration.Object, headerService.Object);
            var telemetry = new Mock<ITelemetry>();
            var context = new TelemetryContext();
            telemetry.Setup(x => x.Context).Returns(context);

            //Act
            service.Initialize(telemetry.Object);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.SessionIdHeaderName, out var sessionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.TransactionIdHeaderName, out var transactionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.ChannelIdHeaderName, out var channelIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.UserInfoLoginId, out var userInfoLoginIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue("HttpContextError", out var httpContextError);
            telemetry.Object.Context.GlobalProperties.TryGetValue("HttpRequestHeadersError", out var httpRequestHeadersError);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdValue, Is.Not.Null);
                Assert.That(sessionIdValue, Is.EqualTo(sessionId));
                Assert.That(transactionIdValue, Is.Not.Null);
                Assert.That(transactionIdValue?.Length, Is.EqualTo(32));
                Assert.That(channelIdValue, Is.Not.Null);
                Assert.That(channelIdValue, Is.EqualTo(channelId));
                Assert.That(userInfoLoginIdValue, Is.Not.Null);
                Assert.That(userInfoLoginIdValue, Is.EqualTo(loginId));
                Assert.That(httpRequestHeadersError, Is.Not.Null);
                Assert.That(httpRequestHeadersError, Is.EqualTo("Request made without headers available for telemetry to consume. Custom headers unable to be set correctly from existing request, used default values."));
            });
        }

        [Test]
        public void InitializeWithHttpContextAndHeaders_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            Mock<ISession> session = new();
            var sessionStorage = new Dictionary<string, byte[]>();
            session.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>())).Callback<string, byte[]>((key, value) => sessionStorage[key] = value);
            session.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]?>.IsAny)).Returns((string key, out byte[]? value) =>
            {
                if (sessionStorage.TryGetValue(key, out var storedValue))
                {
                    value = storedValue;
                    return true;
                }
                value = null;
                return false;
            });
            session.Object.SetString(Constants.SessionId, "existingSessionId");
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "existingLoginId")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.Request.Headers.TryAdd(Constants.SessionIdHeaderName, sessionId);
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            httpContext.Request.Headers.TryAdd(Constants.ChannelIdHeaderName, channelId);
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, JsonConvert.SerializeObject(new UserInfo() { LoginId = loginId, UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true }));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x[Constants.ChannelId]).Returns("existingChannelId");
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.GetSessionId(It.IsAny<IHeaderDictionary>())).Returns(sessionId);
            headerService.Setup(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>())).Returns(transactionId);
            headerService.Setup(x => x.GetChannelId(It.IsAny<IHeaderDictionary>())).Returns(channelId);
            headerService.Setup(x => x.GetUserInfoLoginId(It.IsAny<IHeaderDictionary>())).Returns(loginId);
            var service = new TelemetryInitializer(httpContextAccessor.Object, configuration.Object, headerService.Object);
            var telemetry = new Mock<ITelemetry>();
            var context = new TelemetryContext();
            telemetry.Setup(x => x.Context).Returns(context);

            //Act
            service.Initialize(telemetry.Object);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.SessionIdHeaderName, out var sessionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.TransactionIdHeaderName, out var transactionIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.ChannelIdHeaderName, out var channelIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue(Constants.UserInfoLoginId, out var userInfoLoginIdValue);
            telemetry.Object.Context.GlobalProperties.TryGetValue("HttpContextError", out var httpContextError);
            telemetry.Object.Context.GlobalProperties.TryGetValue("HttpRequestHeadersError", out var httpRequestHeadersError);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdValue, Is.Not.Null);
                Assert.That(sessionIdValue, Is.EqualTo(sessionId));
                Assert.That(transactionIdValue, Is.Not.Null);
                Assert.That(transactionIdValue, Is.EqualTo(transactionId));
                Assert.That(channelIdValue, Is.Not.Null);
                Assert.That(channelIdValue, Is.EqualTo(channelId));
                Assert.That(userInfoLoginIdValue, Is.Not.Null);
                Assert.That(userInfoLoginIdValue, Is.EqualTo(loginId));
                Assert.That(httpContextError, Is.Null);
                Assert.That(httpRequestHeadersError, Is.Null);
            });
        }
    }
}