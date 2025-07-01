using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class RooLoggerTests
    {
        //Common
        Mock<ILogger<RooLogger>> loggerMoq;
        Mock<IHeaderService> headerServiceMoq;
        Mock<IHeaderService> headerServiceWithUserInfoMoq;
        Mock<IHttpContextAccessor> httpContextAccessorMoq;
        UserInfo userInfo = new() { LoginId = "loginId", UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true };
        DefaultHttpContext httpContext = new();
        string message = "test";
        Exception exception = new();

        [SetUp]
        public void Setup()
        {
            loggerMoq = new();
            headerServiceMoq = new();
            headerServiceMoq.Setup(x => x.GetSessionId(It.IsAny<IHeaderDictionary>())).Returns("sessionId");
            headerServiceMoq.Setup(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>())).Returns("transactionId");
            headerServiceMoq.Setup(x => x.GetChannelId(It.IsAny<IHeaderDictionary>())).Returns("channelId");
            headerServiceWithUserInfoMoq = new();
            headerServiceWithUserInfoMoq.Setup(x => x.GetSessionId(It.IsAny<IHeaderDictionary>())).Returns("sessionId");
            headerServiceWithUserInfoMoq.Setup(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>())).Returns("transactionId");
            headerServiceWithUserInfoMoq.Setup(x => x.GetChannelId(It.IsAny<IHeaderDictionary>())).Returns("channelId");
            headerServiceWithUserInfoMoq.Setup(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>())).Returns(userInfo);
            headerServiceWithUserInfoMoq.Setup(x => x.DoesUserInfoHaveInfo(It.IsAny<IHeaderDictionary>())).Returns(true);
            httpContextAccessorMoq = new Mock<IHttpContextAccessor>();
        }

        #region LogInfo
        [Test]
        public void LogInformationWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogInformationWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogInformation(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion

        #region LogError
        [Test]
        public void LogErrorWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogErrorWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogError(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion

        #region LogWarning
        [Test]
        public void LogWarningWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogWarningWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogWarning(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion

        #region LogTrace
        [Test]
        public void LogTraceWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogTraceWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogTrace(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion

        #region LogCritical
        [Test]
        public void LogCriticalWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogCriticalWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogCritical(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion

        #region LogDebug
        [Test]
        public void LogDebugWithUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithOnlyUserInfo_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceWithUserInfoMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceWithUserInfoMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceWithUserInfoMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()));
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithOnlyMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, message, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithoutUserInfoAndMessage_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, null, exception);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithoutException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithoutMessageAndException_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(httpContext, null, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => !x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithoutHttpContext_VerifyLog()
        {
            //Arrange
            httpContextAccessorMoq.SetupProperty(x => x.HttpContext);
            httpContextAccessorMoq.Object.HttpContext = httpContext;
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()));
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            });
        }

        [Test]
        public void LogDebugWithoutHttpContextAvailable_VerifyLog()
        {
            //Arrange
            var service = new RooLogger(loggerMoq.Object, headerServiceMoq.Object, httpContextAccessorMoq.Object);

            //Act
            service.LogDebug(null, message, null);

            //Assert
            Assert.Multiple(() =>
            {
                headerServiceMoq.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetChannelId(It.IsAny<IHeaderDictionary>()), Times.Never);
                headerServiceMoq.Verify(x => x.GetUserInfo(It.IsAny<IHeaderDictionary>()), Times.Never);
                loggerMoq.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((x, y) => x.ToString().Contains(message)), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
            });
        }
        #endregion
    }
}