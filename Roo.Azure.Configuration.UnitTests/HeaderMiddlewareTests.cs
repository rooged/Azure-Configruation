using Microsoft.AspNetCore.Http;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Middlewares;
using System.Security.Claims;

namespace Roo.Azure.Configuration.UnitTests
{
    public class HeaderMiddlewareTests
    {
        //Common
        private Mock<IRooLogger> loggerMoq;
        private RequestDelegate next;

        [SetUp]
        public void Setup()
        {
            loggerMoq = new();
            next = (hc) => Task.CompletedTask;
        }

        [Test]
        public async Task InvokeSwaggerNoValidation_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/path/swagger/path";
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);

            //Assert
            using (Assert.EnterMultipleScope())
            {
                headerService.Verify(x => x.GetSessionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task InvokeSessionIdInvalid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>())).Returns(false);
            headerService.Setup(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            using (Assert.EnterMultipleScope())
            {
                headerService.Verify(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>()), Times.Once);
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(432));
                Assert.That(response, Is.EqualTo("Error 432: Header(s) not found or invalid. session-id valid: False. transaction-id valid: True. channel-id valid: True."));
            }
        }

        [Test]
        public async Task InvokeTransactionIdInvalid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>())).Returns(false);
            headerService.Setup(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            using (Assert.EnterMultipleScope())
            {
                headerService.Verify(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>()), Times.Once);
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(433));
                Assert.That(response, Is.EqualTo("Error 433: Header(s) not found or invalid. session-id valid: True. transaction-id valid: False. channel-id valid: True."));
            }
        }

        [Test]
        public async Task InvokeChannelIdInvalid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>())).Returns(false);

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            using (Assert.EnterMultipleScope())
            {
                headerService.Verify(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>()), Times.Once);
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(434));
                Assert.That(response, Is.EqualTo("Error 434: Header(s) not found or invalid. session-id valid: True. transaction-id valid: True. channel-id valid: False."));
            }
        }

        [Test]
        public async Task InvokeUserInfoInvalid_VerifyLog()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            Mock<ISession> session = new();
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, "email", ClaimValueTypes.Email)
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.DoesUserInfoHaveInfo(It.IsAny<IHeaderDictionary>())).Returns(false);

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            using (Assert.EnterMultipleScope())
            {
                headerService.Verify(x => x.DoesUserInfoHaveInfo(It.IsAny<IHeaderDictionary>()), Times.Once);
                //Assert.That(httpContext.Response.StatusCode, Is.EqualTo(435));
                //Assert.That(response, Is.EqualTo("Error 435: Header(s) not found or invalid. user-info valid: False. session-id valid: True. transaction-id valid: True. channel-id valid: True."));
            }
        }

        [Test]
        public async Task InvokeValid_VerifyLog()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            Mock<ISession> session = new();
            httpContext.Session = session.Object;
            var service = new HeaderMiddleware(next);
            var headerService = new Mock<IHeaderService>();
            headerService.Setup(x => x.IsSessionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsTransactionIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.IsChannelIdValid(It.IsAny<IHeaderDictionary>())).Returns(true);
            headerService.Setup(x => x.DoesUserInfoHaveInfo(It.IsAny<IHeaderDictionary>())).Returns(false);

            //Act
            await service.Invoke(httpContext, headerService.Object, loggerMoq.Object);

            //Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(200));
        }
    }
}