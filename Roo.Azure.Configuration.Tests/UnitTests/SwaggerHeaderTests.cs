using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Roo.Azure.Configuration.Common.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class SwaggerHeaderTests
    {
        //Common
        string sessionId = "sessionId";
        string channelId = "channelId";

        [SetUp]
        public void Setup() { }

        [Test]
        public void ApplyHeadersToSwagger_Verify()
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
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty(x => x.HttpContext);
            httpContextAccessor.Object.HttpContext = httpContext;
            httpContext.Request.Headers.TryAdd(Constants.SessionIdHeaderName, sessionId);
            var configMemory = new Dictionary<string, string?>() { { Constants.ChannelId, channelId } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configMemory).Build();
            var service = new SwaggerHeader(configuration, httpContextAccessor.Object);
            var operation = new OpenApiOperation();
            var schemaGenerator = new Mock<ISchemaGenerator>();
            var methodInfo = new Mock<MethodInfo>();
            var filterContext = new OperationFilterContext(new(), schemaGenerator.Object, new(), methodInfo.Object);

            //Act
            service.Apply(operation, filterContext);
            var sessionParameter = operation.Parameters.First(x => x.Name == Constants.SessionIdHeaderName);
            var transactionParameter = operation.Parameters.First(x => x.Name == Constants.TransactionIdHeaderName);
            var channelParameter = operation.Parameters.First(x => x.Name == Constants.ChannelIdHeaderName);
            var userInfoParameter = operation.Parameters.First(x => x.Name == Constants.UserInfoHeaderName);
            var sessionString = sessionParameter.Schema.Default as OpenApiString;
            var transactionString = transactionParameter.Schema.Default as OpenApiString;
            var channelString = channelParameter.Schema.Default as OpenApiString;
            var userInfoDictionary = userInfoParameter.Schema.Properties;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionString, Is.Not.Null);
                Assert.That(sessionString?.Value, Is.EqualTo(sessionId));
                Assert.That(transactionString, Is.Not.Null);
                Assert.That(transactionString?.Value.Length, Is.EqualTo(32));
                Assert.That(channelString, Is.Not.Null);
                Assert.That(channelString?.Value, Is.EqualTo(channelId));
                Assert.That(userInfoParameter, Is.Not.Null);
                Assert.That(userInfoDictionary, Is.Not.Null);
                Assert.That(userInfoDictionary.Count, Is.EqualTo(6));
                Assert.That(userInfoDictionary.First().Key, Is.EqualTo("LoginId"));
            });
        }
    }
}