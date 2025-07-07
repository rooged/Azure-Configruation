using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Middlewares;

namespace Roo.Azure.Configuration.UnitTests
{
    public class ServiceExceptionMiddlewareTests
    {
        //Common
        private Mock<IRooLogger> loggerMoq;
        private Mock<IWebHostEnvironment> webHostEnvironment;

        [SetUp]
        public void Setup()
        {
            loggerMoq = new();
            webHostEnvironment = new();
        }

        [Test]
        public async Task InvokeConvertException_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var next = new RequestDelegate(_ => Task.CompletedTask);
            var service = new ServiceExceptionMiddleware(next, webHostEnvironment.Object);
            var headerService = new Mock<IHeaderService>();

            //Act
            await service.InvokeAsync(httpContext, headerService.Object, loggerMoq.Object);

            //Assert
            Assert.Multiple(() =>
            {
                headerService.Verify(x => x.GetTransactionId(It.IsAny<IHeaderDictionary>()), Times.Never);
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(200));
            });
        }

        [Test]
        public async Task InvokeNoException_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            using var memoryStream = new MemoryStream();
            httpContext.Response.Body = memoryStream;
            var next = new RequestDelegate(_ => throw new InvalidOperationException("Exception test"));
            var service = new ServiceExceptionMiddleware(next, webHostEnvironment.Object);
            var headerService = new Mock<IHeaderService>();

            //Act
            await service.InvokeAsync(httpContext, headerService.Object, loggerMoq.Object);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Dispose();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo(549));
                Assert.That(response.Contains("549"), Is.True);
            });
        }
    }
}