using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Http;
using Roo.Azure.Configuration.Common.Http.AzureAdAuthentication;
using Roo.Azure.Configuration.Common.Http.Models;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class RooHttpClientTests
    {
        //Common
        private Mock<IRooLogger> loggerMoq;
        private Mock<IAzureAdClientAssertion> azureAdClientAssertionMoq;
        private string sessionId = "sessionId";
        private string transactionId = "transactionId";
        private string channelId = "channelId";
        private UserInfo userInfo = new() { LoginId = "loginId", UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true };
        private string httpClientName = "httpClientName";
        private string relativeUrl = "http://unittest.com/test/";
        private List<(string Parameter, string Value)> queryParameters = new() { ("parameter", "value") };
        private string relativeUrlWithParameters = "http://unittest.com/test?parameter=value";
        private string relativeUrlWithQueryObject = "http://unittest.com/test?Id=1&Value=value";
        private Mock<ISession> session;
        private Mock<ISession> sessionExistingSessionId;
        private Test testObject = new() { Id = 1, Value = "value" };
        private List<(string Name, string Value)> headers = new() { ("header", "value"), ("header2", "value2") };
        private string token = "token";
        private string authenticationHttpClientName = "authenticationHttpClientName";

        [SetUp]
        public void Setup()
        {
            loggerMoq = new();
            azureAdClientAssertionMoq = new();
            session = new();
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
            sessionExistingSessionId = new();
            var sessionStorageExistingSessionId = new Dictionary<string, byte[]>();
            sessionExistingSessionId.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>())).Callback<string, byte[]>((key, value) => sessionStorageExistingSessionId[key] = value);
            sessionExistingSessionId.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]?>.IsAny)).Returns((string key, out byte[]? value) =>
            {
                if (sessionStorageExistingSessionId.TryGetValue(key, out var storedValue))
                {
                    value = storedValue;
                    return true;
                }
                value = null;
                return false;
            });
            sessionExistingSessionId.Object.SetString(Constants.SessionId, "existingSessionId");
        }

        #region GetAsync
        [Test]
        public async Task GetAsyncNoDeserialization_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "loginId")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName, queryParameters);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);
            requestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            var userInfoHeaderModel = JsonConvert.DeserializeObject<UserInfo>(userInfoHeader?.First() ?? default!);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(sessionId));
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First().Length, Is.EqualTo(32));
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
                Assert.That(userInfoHeader, Is.Not.Null);
                Assert.That(userInfoHeaderModel, Is.Not.Null);
                Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId"));
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithParameters));
            });
        }

        [Test]
        public async Task GetAsyncNoDeserializationQueryStringObject_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName, testObject);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithQueryObject));
            });
        }

        [Test]
        public async Task GetAsyncQueryStringObject_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.GetAsync<Test>(relativeUrl, httpClientName, testObject);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithQueryObject));
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        public async Task GetAsync_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.GetAsync<Test>(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetAsyncEnumClientNames_Verify(bool deserialize)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactoryQueryObject = new Mock<IHttpClientFactory>();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandlerQueryObject = new Mock<DelegatingHandler>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessageQueryObject = null;
            HttpRequestMessage? requestMessage = null;
            if (deserialize)
            {
                var httpResponseMessageQueryObject = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(testObject)) };
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(testObject)) };
                delegatingHandlerQueryObject.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageQueryObject = request).ReturnsAsync(httpResponseMessageQueryObject);
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandlerQueryObject.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageQueryObject = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            }
            delegatingHandlerQueryObject.As<IDisposable>().Setup(x => x.Dispose());
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClientQueryObject = new HttpClient(delegatingHandlerQueryObject.Object);
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactoryQueryObject.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClientQueryObject);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var serviceQueryObject = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactoryQueryObject.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? responseQueryObjectMessage = null;
            HttpResponseMessage? responseMessage = null;
            Test? responseQueryObject = null;
            Test? response = null;
            if (deserialize)
            {
                responseQueryObject = await serviceQueryObject.GetAsync<Test>(relativeUrl, TestClientName.httpClientName, testObject);
                response = await service.GetAsync<Test>(relativeUrl, TestClientName.httpClientName);
            }
            else
            {
                responseQueryObjectMessage = await serviceQueryObject.GetAsync(relativeUrl, TestClientName.httpClientName, testObject);
                responseMessage = await service.GetAsync(relativeUrl, TestClientName.httpClientName);
            }
            if (requestMessage == null || requestMessageQueryObject == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                if (deserialize)
                {
                    Assert.That(requestMessageQueryObject.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithQueryObject));
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                    Assert.That(responseQueryObject, Is.Not.Null);
                    Assert.That(responseQueryObject?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(responseQueryObject?.Value, Is.EqualTo(testObject.Value));
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(response?.Value, Is.EqualTo(testObject.Value));
                }
                else
                {
                    Assert.That(requestMessageQueryObject.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithQueryObject));
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                }
            });
        }
        #endregion

        #region PostAsync
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task PostAsyncNoDeserialization_Verify(int inputModelType)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "loginId")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? response = null;
            if (inputModelType == 0)
            {
                response = await service.PostAsync(relativeUrl, httpClientName, testObject, queryParameters);
            }
            else if (inputModelType == 1)
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(testObject));
                response = await service.PostAsync(relativeUrl, httpClientName, httpContent, queryParameters);
            }
            else if (inputModelType == 2)
            {
                var multipartFormDataContent = new MultipartFormDataContent
                {
                    new StringContent(JsonConvert.SerializeObject(testObject))
                };
                var responses = await service.PostAsync(relativeUrl, httpClientName, multipartFormDataContent, queryParameters);
            }
            else if (inputModelType == 3)
            {
                var multipartContent = new MultipartContent();
                response = await service.PostAsync(relativeUrl, httpClientName, multipartContent, queryParameters);
            }
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);
            requestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            var userInfoHeaderModel = JsonConvert.DeserializeObject<UserInfo>(userInfoHeader?.First() ?? default!);
            Test? requestObject = null;
            if (inputModelType < 2)
            {
                using (var json = new JsonTextReader(new StreamReader(await requestMessage.Content!.SafeReadAsStreamAsync().ConfigureAwait(false) ?? default!)))
                {
                    var serializer = JsonSerializer.Create();
                    requestObject = serializer.Deserialize<Test>(json);
                }
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(sessionId));
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First().Length, Is.EqualTo(32));
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
                Assert.That(userInfoHeader, Is.Not.Null);
                Assert.That(userInfoHeaderModel, Is.Not.Null);
                Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId"));
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithParameters));
                if (inputModelType < 2)
                {
                    Assert.That(requestObject?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(requestObject?.Value, Is.EqualTo(testObject.Value));
                }
            });
        }

        [Test]
        public async Task PostAsync_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.PostAsync<Test>(relativeUrl, httpClientName, testObject);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            Test? requestObject = null;
            using (var json = new JsonTextReader(new StreamReader(await requestMessage.Content!.SafeReadAsStreamAsync().ConfigureAwait(false) ?? default!)))
            {
                var serializer = JsonSerializer.Create();
                requestObject = serializer.Deserialize<Test>(json);
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
                Assert.That(requestObject?.Id, Is.EqualTo(testObject.Id));
                Assert.That(requestObject?.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task PostAsyncEnumClientNames_Verify(bool deserialize)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            if (deserialize)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(testObject)) };
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            }
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? responseMessage = null;
            Test? response = null;
            if (deserialize)
            {
                response = await service.PostAsync<Test>(relativeUrl, TestClientName.httpClientName, testObject);
            }
            else
            {
                responseMessage = await service.PostAsync(relativeUrl, TestClientName.httpClientName, testObject);
            }
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                if (deserialize)
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(response?.Value, Is.EqualTo(testObject.Value));
                }
                else
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                }
            });
        }
        #endregion

        #region PutAsync
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task PutAsyncNoDeserialization_Verify(int inputModelType)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "loginId")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? response = null;
            if (inputModelType == 0)
            {
                response = await service.PutAsync(relativeUrl, httpClientName, testObject, queryParameters);
            }
            else if (inputModelType == 1)
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(testObject));
                response = await service.PutAsync(relativeUrl, httpClientName, httpContent, queryParameters);
            }
            else if (inputModelType == 2)
            {
                var multipartFormDataContent = new MultipartFormDataContent
                {
                    new StringContent(JsonConvert.SerializeObject(testObject))
                };
                var responses = await service.PutAsync(relativeUrl, httpClientName, multipartFormDataContent, queryParameters);
            }
            else if (inputModelType == 3)
            {
                var multipartContent = new MultipartContent();
                response = await service.PutAsync(relativeUrl, httpClientName, multipartContent, queryParameters);
            }
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);
            requestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            var userInfoHeaderModel = JsonConvert.DeserializeObject<UserInfo>(userInfoHeader?.First() ?? default!);
            Test? requestObject = null;
            if (inputModelType < 2)
            {
                using (var json = new JsonTextReader(new StreamReader(await requestMessage.Content!.SafeReadAsStreamAsync().ConfigureAwait(false) ?? default!)))
                {
                    var serializer = JsonSerializer.Create();
                    requestObject = serializer.Deserialize<Test>(json);
                }
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(sessionId));
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First().Length, Is.EqualTo(32));
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
                Assert.That(userInfoHeader, Is.Not.Null);
                Assert.That(userInfoHeaderModel, Is.Not.Null);
                Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId"));
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithParameters));
                if (inputModelType < 2)
                {
                    Assert.That(requestObject?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(requestObject?.Value, Is.EqualTo(testObject.Value));
                }
            });
        }

        [Test]
        public async Task PutAsync_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.PutAsync<Test>(relativeUrl, httpClientName, testObject);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            Test? requestObject = null;
            using (var json = new JsonTextReader(new StreamReader(await requestMessage.Content!.SafeReadAsStreamAsync().ConfigureAwait(false) ?? default!)))
            {
                var serializer = JsonSerializer.Create();
                requestObject = serializer.Deserialize<Test>(json);
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
                Assert.That(requestObject?.Id, Is.EqualTo(testObject.Id));
                Assert.That(requestObject?.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task PutAsyncEnumClientNames_Verify(bool deserialize)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            if (deserialize)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(testObject)) };
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            }
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? responseMessage = null;
            Test? response = null;
            if (deserialize)
            {
                response = await service.PutAsync<Test>(relativeUrl, TestClientName.httpClientName, testObject);
            }
            else
            {
                responseMessage = await service.PutAsync(relativeUrl, TestClientName.httpClientName, testObject);
            }
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                if (deserialize)
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(response?.Value, Is.EqualTo(testObject.Value));
                }
                else
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                }
            });
        }
        #endregion

        #region DeleteAsync
        [Test]
        public async Task DeleteAsyncNoDeserialization_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "loginId")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.DeleteAsync(relativeUrl, httpClientName, queryParameters);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);
            requestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            var userInfoHeaderModel = JsonConvert.DeserializeObject<UserInfo>(userInfoHeader?.First() ?? default!);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(sessionIdHeader?.First(), Is.EqualTo(sessionId));
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First().Length, Is.EqualTo(32));
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
                Assert.That(userInfoHeader, Is.Not.Null);
                Assert.That(userInfoHeaderModel, Is.Not.Null);
                Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId"));
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrlWithParameters));
            });
        }

        [Test]
        public async Task DeleteAsync_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            var response = await service.DeleteAsync<Test>(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task DeleteAsyncEnumClientNames_Verify(bool deserialize)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            if (deserialize)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(testObject)) };
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            }
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);

            //Act
            HttpResponseMessage? responseMessage = null;
            Test? response = null;
            if (deserialize)
            {
                response = await service.DeleteAsync<Test>(relativeUrl, TestClientName.httpClientName);
            }
            else
            {
                responseMessage = await service.DeleteAsync(relativeUrl, TestClientName.httpClientName);
            }
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }

            //Assert
            Assert.Multiple(() =>
            {
                if (deserialize)
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response?.Id, Is.EqualTo(testObject.Id));
                    Assert.That(response?.Value, Is.EqualTo(testObject.Value));
                }
                else
                {
                    Assert.That(requestMessage.RequestUri?.AbsoluteUri, Is.EqualTo(relativeUrl));
                }
            });
        }
        #endregion

        #region HeaderParameters
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task AddHeadersFromIncomingRequest_Verify(bool directlySetHeaders)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.SessionIdHeaderName, sessionId);
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, "existingTransactionId");
            httpContext.Request.Headers.TryAdd(Constants.ChannelIdHeaderName, "existingChannelId");
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, JsonConvert.SerializeObject(userInfo));
            httpContext.Session = sessionExistingSessionId.Object;
            var claims = new List<Claim>
            {
                new Claim(Constants.UserInfoLoginId, "loginId"),
                new Claim(ClaimTypes.Email, "email")
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            if (directlySetHeaders)
            {
                service.SessionId = $"{sessionId}2";
                service.TransactionId = transactionId;
                service.ChannelId = $"{channelId}2";
                service.UserInfo = new() { LoginId = "loginId2", UserId = "userId2", Email = "email2", SubId = "subId2", IsAuthenticated = true };
            }

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(Constants.SessionIdHeaderName, out var sessionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.TransactionIdHeaderName, out var transactionIdHeader);
            requestMessage.Headers.TryGetValues(Constants.ChannelIdHeaderName, out var channelIdHeader);
            requestMessage.Headers.TryGetValues(Constants.UserInfoHeaderName, out var userInfoHeader);
            var userInfoHeaderModel = JsonConvert.DeserializeObject<UserInfo>(userInfoHeader?.First() ?? default!);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(sessionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader, Is.Not.Null);
                Assert.That(transactionIdHeader?.First(), Is.Not.EqualTo("existingTransactionId"));
                Assert.That(channelIdHeader, Is.Not.Null);
                Assert.That(userInfoHeader, Is.Not.Null);
                Assert.That(userInfoHeaderModel, Is.Not.Null);
                Assert.That(userInfoHeaderModel?.IsAuthenticated, Is.EqualTo(true));

                if (!directlySetHeaders)
                {
                    Assert.That(sessionIdHeader?.First(), Is.EqualTo(sessionId));
                    Assert.That(transactionIdHeader?.First().Length, Is.EqualTo(32));
                    Assert.That(channelIdHeader?.First(), Is.EqualTo(channelId));
                    Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId"));
                    Assert.That(userInfoHeaderModel?.UserId, Is.EqualTo("userId"));
                    Assert.That(userInfoHeaderModel?.Email, Is.EqualTo("email"));
                    Assert.That(userInfoHeaderModel?.SubId, Is.EqualTo("subId"));
                }
                else
                {
                    Assert.That(sessionIdHeader?.First(), Is.EqualTo($"{sessionId}2"));
                    Assert.That(transactionIdHeader?.First(), Is.EqualTo(transactionId));
                    Assert.That(channelIdHeader?.First(), Is.EqualTo($"{channelId}2"));
                    Assert.That(userInfoHeaderModel?.LoginId, Is.EqualTo("loginId2"));
                    Assert.That(userInfoHeaderModel?.UserId, Is.EqualTo("userId2"));
                    Assert.That(userInfoHeaderModel?.Email, Is.EqualTo("email2"));
                    Assert.That(userInfoHeaderModel?.SubId, Is.EqualTo("subId2"));
                }
            });
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task AddHeadersForAuthentication_Verify(bool addApimHeader)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            service.AuthenticationInfo = new()
            {
                AdditionalRequestHeaders = headers,
                AuthenticationStrategy = AuthenticationStrategy.OAuth
            };
            if (addApimHeader)
            {
                service.AuthenticationInfo.AuthenticationStrategy = AuthenticationStrategy.ApimCertificate;
                service.AuthenticationInfo.ApimSubscriptionKey = "apimSubscriptionKey";
            }

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues(headers[0].Name, out var authHeader);
            requestMessage.Headers.TryGetValues(headers[1].Name, out var authHeader2);
            requestMessage.Headers.TryGetValues("Ocp-Apim-Subscription-Key", out var apimHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(authHeader, Is.Not.Null);
                Assert.That(authHeader?.First(), Is.EqualTo(headers[0].Value));
                Assert.That(authHeader2, Is.Not.Null);
                Assert.That(authHeader2?.First(), Is.EqualTo(headers[1].Value));

                if (!addApimHeader)
                {
                    Assert.That(apimHeader, Is.Null);
                }
                else
                {
                    Assert.That(apimHeader?.First(), Is.EqualTo("apimSubscriptionKey"));
                }
            });
        }
        #endregion

        #region GetToken
        [Test]
        [TestCase("none", true)]
        [TestCase("none", false)]
        [TestCase("redis", true)]
        [TestCase("redis", false)]
        [TestCase("memoryCache", true)]
        [TestCase("memoryCache", false)]
        public async Task GetOAuthToken_Verify(string tokenStorage, bool tokenReturned)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var delegatingHandlerAuth = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            HttpRequestMessage? requestMessageAuth = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            var tokenResponse = new OAuthTokenResponse()
            {
                Access_Token = token,
                Expires_In = 61,
                RefreshToken = "refreshToken",
                Token_Type = "tokenType"
            };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(tokenResponse)
            };
            if (tokenReturned)
            {
                delegatingHandlerAuth.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageAuth = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandlerAuth.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageAuth = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            delegatingHandlerAuth.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            var httpClientAuth = new HttpClient(delegatingHandlerAuth.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            httpClientFactory.Setup(x => x.CreateClient(authenticationHttpClientName)).Returns(httpClientAuth);
            RooHttpClient service = default!;
            Mock<IRedisService>? redis = null;
            MemoryCache? memoryCache = null;
            if (tokenStorage.Equals("none"))
            {
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            }
            else if (tokenStorage.Equals("redis"))
            {
                redis = new Mock<IRedisService>();
                redis.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string?)null);
                redis.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, redis.Object);
            }
            else if (tokenStorage.Equals("memoryCache"))
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, memoryCache);
            }
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.OAuth,
                AuthenticationHttpClientName = authenticationHttpClientName,
                LoginId = "loginId",
                Password = "password",
                TokenUrl = "http://unittest.com/test/auth",
                AuthenticationHeaders  = headers
            };

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null || requestMessageAuth == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);
            IEnumerable<string>? contentTypeHeader = null;
            requestMessageAuth.Content?.Headers.TryGetValues("Content-Type", out contentTypeHeader);
            var requestAuthContent = await requestMessageAuth.Content.ReadAsFormDataAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(contentTypeHeader, Is.Not.Null);
                Assert.That(contentTypeHeader?.First(), Is.EqualTo("application/x-www-form-urlencoded"));
                Assert.That(requestAuthContent, Is.Not.Null);
                Assert.That(requestAuthContent["username"], Is.Not.Null);
                Assert.That(requestAuthContent["username"], Is.EqualTo("loginId"));
                Assert.That(requestAuthContent["password"], Is.Not.Null);
                Assert.That(requestAuthContent["password"], Is.EqualTo("password"));
                Assert.That(requestAuthContent["grant_type"], Is.Not.Null);
                Assert.That(requestAuthContent["grant_type"], Is.EqualTo("password"));
                if (tokenReturned)
                {
                    Assert.That(authorizationHeader, Is.Not.Null);
                    Assert.That(authorizationHeader?.First(), Is.EqualTo($"Bearer {token}"));
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Once);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.EqualTo(token));
                    }
                }
                else
                {
                    Assert.That(authorizationHeader, Is.Null);
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Never);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.Null);
                    }
                }
                
            });
            if (memoryCache != null)
            {
                memoryCache?.Dispose();
            }
        }

        [Test]
        [TestCase("certKeyVault", "none", true)]
        [TestCase("certKeyVault", "none", false)]
        [TestCase("certKeyVault", "redis", true)]
        [TestCase("certKeyVault", "redis", false)]
        [TestCase("certKeyVault", "memoryCache", true)]
        [TestCase("certKeyVault", "memoryCache", false)]
        [TestCase("certPassedIn", "none", true)]
        [TestCase("certGenerateFromString", "none", true)]
        [TestCase("clientSecret", "none", true)]
        public async Task GetApimCertificateToken_Verify(string apimAuth, string tokenStorage, bool tokenReturned)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            var azureAdClientAssertion = new Mock<IAzureAdClientAssertion>();
            if (apimAuth.Equals("certKeyVault"))
            {
                azureAdClientAssertion.Setup(x => x.GetCertificateFromKeyVault(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenCredential?>())).ReturnsAsync(new Mock<X509Certificate2>().Object);
            }
            else if (apimAuth.Equals("certGenerateFromString"))
            {
                azureAdClientAssertion.Setup(x => x.GenerateCertificateFromString(It.IsAny<string>(), It.IsAny<string>())).Returns(new Mock<X509Certificate2>().Object);
            }
            if (!apimAuth.Equals("clientSecret"))
            {
                if (tokenReturned)
                {
                    azureAdClientAssertion.Setup(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>())).ReturnsAsync(token);
                }
                else
                {
                    azureAdClientAssertion.Setup(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>())).ReturnsAsync((string?)null);
                }
            }
            else
            {
                if (tokenReturned)
                {
                    azureAdClientAssertion.Setup(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(token);
                }
                else
                {
                    azureAdClientAssertion.Setup(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string?)null);
                }
            }
            RooHttpClient service = default!;
            Mock<IRedisService>? redis = null;
            MemoryCache? memoryCache = null;
            if (tokenStorage.Equals("none"))
            {
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertion.Object);
            }
            else if (tokenStorage.Equals("redis"))
            {
                redis = new Mock<IRedisService>();
                redis.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string?)null);
                redis.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertion.Object, redis.Object);
            }
            else if (tokenStorage.Equals("memoryCache"))
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertion.Object, memoryCache);
            }
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.ApimCertificate,
                LoginId = "loginId",
                ApimTenantId = "apimTenantId",
                ApimScope = "apimScope",
                ApimSubscriptionKey = "apimSubscriptionKey"
            };
            if (apimAuth.Equals("certKeyVault"))
            {
                service.AuthenticationInfo.ApimCertificateName = "apimCertificateName";
                service.AuthenticationInfo.ApimAzureKeyVaultUri = "apimAzureKeyVaultUri";
                service.AuthenticationInfo.ApimAzureToken = new DefaultAzureCredential();
            }
            else if (apimAuth.Equals("certPassedIn"))
            {
                service.AuthenticationInfo.ApimCertificate = new Mock<X509Certificate2>().Object;
            }
            else if (apimAuth.Equals("certGenerateFromString"))
            {
                service.AuthenticationInfo.ApimCertificateStringAndPassword = ("apimCertificateString", "apimCertificatePassword");
                service.AuthenticationInfo.ApimAzureKeyVaultUri = "apimAzureKeyVaultUri";
            }
            else if (apimAuth.Equals("clientSecret"))
            {
                service.AuthenticationInfo.AuthenticationStrategy = AuthenticationStrategy.ApimClientSecret;
                service.AuthenticationInfo.ApimClientSecret = "apimClientSecret";
            }

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);

            //Assert
            Assert.Multiple(() =>
            {
                if (apimAuth.Equals("certKeyVault"))
                {
                    azureAdClientAssertion.Verify(x => x.GetCertificateFromKeyVault(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenCredential?>()), Times.Once);
                    azureAdClientAssertion.Verify(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>()), Times.Once);
                }
                else if (apimAuth.Equals("certPassedIn"))
                {
                    azureAdClientAssertion.Verify(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>()), Times.Once);
                }
                else if (apimAuth.Equals("certGenerateFromString"))
                {
                    azureAdClientAssertion.Verify(x => x.GenerateCertificateFromString(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                    azureAdClientAssertion.Verify(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>()), Times.Once);
                }
                else if (apimAuth.Equals("clientSecret"))
                {
                    azureAdClientAssertion.Verify(x => x.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                }
                if (tokenReturned)
                {
                    Assert.That(authorizationHeader, Is.Not.Null);
                    Assert.That(authorizationHeader?.First(), Is.EqualTo($"Bearer {token}"));
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Once);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.EqualTo(token));
                    }
                }
                else
                {
                    Assert.That(authorizationHeader, Is.Null);
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Never);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.Null);
                    }
                }

            });
            if (memoryCache != null)
            {
                memoryCache?.Dispose();
            }
        }

        [Test]
        [TestCase("none", true)]
        [TestCase("none", false)]
        [TestCase("redis", true)]
        [TestCase("redis", false)]
        [TestCase("memoryCache", true)]
        [TestCase("memoryCache", false)]
        public async Task GetBasicToken_Verify(string tokenStorage, bool tokenReturned)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var delegatingHandlerAuth = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            HttpRequestMessage? requestMessageAuth = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            var tokenResponse = new BasicTokenResponse()
            {
                Access_Token = token,
                Expires = 1,
                Token_Type = "tokenType"
            };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(tokenResponse)
            };
            if (tokenReturned)
            {
                delegatingHandlerAuth.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageAuth = request).ReturnsAsync(httpResponseMessage);
            }
            else
            {
                delegatingHandlerAuth.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessageAuth = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            delegatingHandlerAuth.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            var httpClientAuth = new HttpClient(delegatingHandlerAuth.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            httpClientFactory.Setup(x => x.CreateClient(authenticationHttpClientName)).Returns(httpClientAuth);
            RooHttpClient service = default!;
            Mock<IRedisService>? redis = null;
            MemoryCache? memoryCache = null;
            if (tokenStorage.Equals("none"))
            {
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            }
            else if (tokenStorage.Equals("redis"))
            {
                redis = new Mock<IRedisService>();
                redis.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync((string?)null);
                redis.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, redis.Object);
            }
            else if (tokenStorage.Equals("memoryCache"))
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, memoryCache);
            }
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.Basic,
                AuthenticationHttpClientName = authenticationHttpClientName,
                BasicEncodedContent = headers,
                TokenUrl = "http://unittest.com/test/auth",
                AuthenticationHeaders = headers
            };

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null || requestMessageAuth == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);
            IEnumerable<string>? contentTypeHeader = null;
            requestMessageAuth.Content?.Headers.TryGetValues("Content-Type", out contentTypeHeader);
            var requestAuthContent = await requestMessageAuth.Content.ReadAsFormDataAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(contentTypeHeader, Is.Not.Null);
                Assert.That(contentTypeHeader?.First(), Is.EqualTo("application/x-www-form-urlencoded"));
                Assert.That(requestAuthContent, Is.Not.Null);
                Assert.That(requestAuthContent[headers[0].Name], Is.Not.Null);
                Assert.That(requestAuthContent[headers[0].Name], Is.EqualTo(headers[0].Value));
                Assert.That(requestAuthContent[headers[1].Name], Is.Not.Null);
                Assert.That(requestAuthContent[headers[1].Name], Is.EqualTo(headers[1].Value));
                if (tokenReturned)
                {
                    Assert.That(authorizationHeader, Is.Not.Null);
                    Assert.That(authorizationHeader?.First(), Is.EqualTo($"Bearer {token}"));
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Once);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.EqualTo(token));
                    }
                }
                else
                {
                    Assert.That(authorizationHeader, Is.Null);
                    if (tokenStorage.Equals("redis"))
                    {
                        redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                        redis?.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Never);
                    }
                    else if (tokenStorage.Equals("memoryCache"))
                    {
                        Assert.That(memoryCache?.Get(httpClientName), Is.Null);
                    }
                }

            });
            if (memoryCache != null)
            {
                memoryCache?.Dispose();
            }
        }

        [Test]
        public async Task GetPassedInToken_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var delegatingHandlerAuth = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.PassInToken,
                Token = token
            };

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(authorizationHeader, Is.Not.Null);
                Assert.That(authorizationHeader?.First(), Is.EqualTo($"Bearer {token}"));
            });
        }

        [Test]
        [TestCase("redis")]
        [TestCase("memoryCache")]
        public async Task GetTokenExisting_Verify(string tokenStorage)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var delegatingHandlerAuth = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            RooHttpClient service = default!;
            Mock<IRedisService>? redis = null;
            MemoryCache? memoryCache = null;
            if (tokenStorage.Equals("redis"))
            {
                redis = new Mock<IRedisService>();
                redis.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(token);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, redis.Object);
            }
            else if (tokenStorage.Equals("memoryCache"))
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                memoryCache.Set(httpClientName, token);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, memoryCache);
            }
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.PassInToken,
                Token = "token2"
            };

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(authorizationHeader, Is.Not.Null);
                Assert.That(authorizationHeader?.First(), Is.EqualTo($"Bearer {token}"));
                if (tokenStorage.Equals("redis"))
                {
                    redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
                }
                else if (tokenStorage.Equals("memoryCache"))
                {
                    Assert.That(memoryCache?.Get(httpClientName), Is.EqualTo(token));
                }
            });
            if (memoryCache != null)
            {
                memoryCache?.Dispose();
            }
        }

        [Test]
        [TestCase("redis")]
        [TestCase("memoryCache")]
        public async Task ClearToken_Verify(string tokenStorage)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var delegatingHandlerAuth = new Mock<DelegatingHandler>();
            HttpRequestMessage? requestMessage = null;
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => requestMessage = request).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(httpClientName)).Returns(httpClient);
            RooHttpClient service = default!;
            Mock<IRedisService>? redis = null;
            MemoryCache? memoryCache = null;
            if (tokenStorage.Equals("redis"))
            {
                redis = new Mock<IRedisService>();
                redis.Setup(x => x.Delete(It.IsAny<string>())).ReturnsAsync(true);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, redis.Object);
            }
            else if (tokenStorage.Equals("memoryCache"))
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                memoryCache.Set(httpClientName, token);
                service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object, memoryCache);
            }
            service.AuthenticationInfo = new()
            {
                AuthenticationStrategy = AuthenticationStrategy.PassInToken,
                ForceToken = true
            };

            //Act
            var response = await service.GetAsync(relativeUrl, httpClientName);
            if (requestMessage == null)
            {
                Assert.Fail("Request message is null.");
                return;
            }
            requestMessage.Headers.TryGetValues("Authorization", out var authorizationHeader);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(authorizationHeader, Is.Null);
                if (tokenStorage.Equals("redis"))
                {
                    redis?.Verify(x => x.Delete(It.IsAny<string>()), Times.Once);
                    redis?.Verify(x => x.Get(It.IsAny<string>()), Times.Never);
                }
                else if (tokenStorage.Equals("memoryCache"))
                {
                    Assert.That(memoryCache?.Get(httpClientName), Is.Null);
                }
            });
            if (memoryCache != null)
            {
                memoryCache?.Dispose();
            }
        }
        #endregion

        #region Deserialization
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task SystemTextJsonDeserialize_Verify(bool readAsString)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            service.SerializationSettings = new()
            {
                JsonSerializerOptions = new()
                {
                    AllowTrailingCommas = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    IgnoreReadOnlyFields = false
                }
            };
            if (readAsString)
            {
                service.SerializationSettings.ReadAsString = true;
            }

            //Act
            var response = await service.GetAsync<Test>(relativeUrl, httpClientName);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
            });
        }

        [Test]
        [TestCase("none", true)]
        [TestCase("none", false)]
        [TestCase("default", true)]
        [TestCase("default", false)]
        [TestCase("custom", true)]
        [TestCase("custom", false)]
        public async Task NewtonsoftJsonDeserialize_Verify(string settingsToUse, bool readAsString)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session.Object;
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var delegatingHandler = new Mock<DelegatingHandler>();
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testObject))
            };
            delegatingHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpResponseMessage);
            delegatingHandler.As<IDisposable>().Setup(x => x.Dispose());
            var httpClient = new HttpClient(delegatingHandler.Object);
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var service = new RooHttpClient(channelId, httpContextAccessor.Object, httpClientFactory.Object, loggerMoq.Object, azureAdClientAssertionMoq.Object);
            service.SerializationSettings = new();
            if (settingsToUse.Equals("default"))
            {
                service.SerializationSettings.UseDefaultSerializationSettings = true;
            }
            else if(settingsToUse.Equals("custom"))
            {
                service.SerializationSettings.JsonSerializerSettings = new()
                {
                    Culture = CultureInfo.CurrentCulture,
                    NullValueHandling = NullValueHandling.Include,
                    DefaultValueHandling = DefaultValueHandling.Include
                };
            }
            if (readAsString)
            {
                service.SerializationSettings.ReadAsString = true;
            }

            //Act
            var response = await service.GetAsync<Test>(relativeUrl, httpClientName);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Id, Is.EqualTo(testObject.Id));
                Assert.That(response.Value, Is.EqualTo(testObject.Value));
            });
        }
        #endregion

        public class Test
        {
            public int Id { get; set; }
            public string? Value { get; set; } = null;
        }

        public enum TestClientName
        {
            httpClientName = 0
        }
    }
}