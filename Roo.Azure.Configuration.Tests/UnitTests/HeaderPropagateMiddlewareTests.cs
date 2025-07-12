using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Middlewares;
using Roo.Azure.Configuration.Common.Models;
using System.Security.Claims;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class HeaderPropagateMiddlewareTests
    {
        //Common
        private string sessionId = "sessionId";
        private string transactionId = "transactionId";
        private string channelId = "channelId";
        private string existingSessionId = "existingSessionId";
        private string existingTransactionId = "existingTransactionId";
        private string existingChannelId = "existingChannelId";
        private Mock<IConfiguration> configurationMoq;
        private Mock<IHeaderService> headerServiceMoq;

        [SetUp]
        public void Setup()
        {
            configurationMoq = new();
            headerServiceMoq = new();
        }

        [Test]
        public void RequestMissingSessionIdGetFromIncomingRequest_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Object.HttpContext = httpContext;
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.GetSessionId(It.IsAny<IHeaderDictionary>())).Returns(existingSessionId);
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configurationMoq.Object, headerService.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);

            //Assert
            Assert.Multiple(() =>
            {
                headerService.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Once);
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(existingSessionId));
            });
        }

        [Test]
        public void RequestMissingSessionIdGetFromSession_Verify()
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
            session.Object.SetString(Constants.SessionId, existingSessionId);
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty(x => x.HttpContext);
            httpContextAccessor.Object.HttpContext = httpContext;
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.GetSessionId(It.IsAny<IHeaderDictionary>())).Returns((string?)null);
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configurationMoq.Object, headerService.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);

            //Assert
            Assert.Multiple(() =>
            {
                headerService.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Once);
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(existingSessionId));
            });
        }

        [Test]
        public void RequestMissingTransactionId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, existingTransactionId);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Object.HttpContext = httpContext;
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configurationMoq.Object, headerServiceMoq.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.SessionIdHeaderName, sessionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First(), Is.Not.EqualTo(existingSessionId));
            });
        }

        [Test]
        public void RequestMissingChannelId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.ChannelIdHeaderName, existingChannelId);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Object.HttpContext = httpContext;
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x[Constants.ChannelId]).Returns(channelId);
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configuration.Object, headerServiceMoq.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.SessionIdHeaderName, sessionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
            });
        }

        [Test]
        public void RequestMissingUserInfo_VerifyLog()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "email", ClaimValueTypes.Email)
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty(x => x.HttpContext);
            httpContextAccessor.Object.HttpContext = httpContext;
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>())).Returns(new UserInfo { LoginId = "loginId", UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true });
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configurationMoq.Object, headerService.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.SessionIdHeaderName, sessionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            Assert.That(userInfoHeader, Is.Not.Null);
            var userInfo = JsonConvert.DeserializeObject<UserInfo?>(userInfoHeader.First());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(userInfo, Is.Not.Null);
                Assert.That(userInfo?.LoginId, Is.EqualTo("loginId"));
            });
        }

        [Test]
        public void RequestMissingCustomHeader_VerifyLog()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty(x => x.HttpContext);
            httpContextAccessor.Object.HttpContext = httpContext;
            var customHeaderName = "customHeaderName";
            var customHeaderValue = "customHeaderValue";
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x[customHeaderName]).Returns(customHeaderValue);
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configuration.Object, headerServiceMoq.Object, new HeaderPropagateOptions([customHeaderName]));
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.SessionIdHeaderName, sessionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);
            httpRequestMessage.Headers.TryGetValues(customHeaderName, out var customHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(customHeader, Is.Not.Null);
                Assert.That(customHeader?.First(), Is.EqualTo(customHeaderValue));
            });
        }

        [Test]
        public void RequestHasRequiredHeaders_VerifyLog()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Object.HttpContext = httpContext;
            var headerService = new Mock<IHeaderService>();
            var service = new HeaderPropagateMiddleware(httpContextAccessor.Object, configurationMoq.Object, headerService.Object);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.SessionIdHeaderName, sessionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.TransactionIdHeaderName, transactionId);
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.ChannelIdHeaderName, channelId);

            //Act
            service.Run(httpRequestMessage);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(200));
            });
        }
    }
}