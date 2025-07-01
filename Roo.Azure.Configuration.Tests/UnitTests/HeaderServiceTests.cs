using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class HeaderServiceTests
    {
        //Common
        string sessionId = "sessionId";
        string transactionId = "transactionId";
        string channelId = "channelId";
        UserInfo userInfo = new() { LoginId = "loginId", UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true };
        IHeaderDictionary headerDictionaryMissingHeaders;

        [SetUp]
        public void Setup()
        {
            var httpContextMissingHeaders = new DefaultHttpContext();
            headerDictionaryMissingHeaders = httpContextMissingHeaders.Request.Headers;
        }

        #region SessionId
        [Test]
        public void GetSessionId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.SessionIdHeaderName, sessionId);
            var service = new HeaderService();

            //Act
            var result = service.GetSessionId(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(sessionId));
            });
        }

        [Test]
        public void GetSessionIdMissing_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.GetSessionId(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(null));
            });
        }

        [Test]
        public void IsSessionIdValid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.SessionIdHeaderName, sessionId);
            var service = new HeaderService();

            //Act
            var result = service.IsSessionIdValid(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(true));
            });
        }

        [Test]
        public void IsSessionIdInvalid_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.IsSessionIdValid(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(false));
            });
        }
        #endregion

        #region TransactionId
        [Test]
        public void GetTransactionId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            var service = new HeaderService();

            //Act
            var result = service.GetTransactionId(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(transactionId));
            });
        }

        [Test]
        public void GetTransactionIdMissing_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.GetTransactionId(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(null));
            });
        }

        [Test]
        public void IsTransactionIdValid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            var service = new HeaderService();

            //Act
            var result = service.IsTransactionIdValid(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(true));
            });
        }

        [Test]
        public void IsTransactionIdInvalid_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.IsTransactionIdValid(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(false));
            });
        }
        #endregion

        #region ChannelId
        [Test]
        public void GetChannelId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.ChannelIdHeaderName, channelId);
            var service = new HeaderService();

            //Act
            var result = service.GetChannelId(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(channelId));
            });
        }

        [Test]
        public void GetChannelIdMissing_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.GetChannelId(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(null));
            });
        }

        [Test]
        public void IsChannelIdValid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.TryAdd(Constants.ChannelIdHeaderName, channelId);
            var service = new HeaderService();

            //Act
            var result = service.IsChannelIdValid(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(true));
            });
        }

        [Test]
        public void IsChannelIdInvalid_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.IsChannelIdValid(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(false));
            });
        }
        #endregion

        #region UserInfo
        [Test]
        public void GetUserInfo_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var userInfoString = JsonConvert.SerializeObject(userInfo);
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, userInfoString);
            var service = new HeaderService();

            //Act
            var result = service.GetUserInfo(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.LoginId, Is.EqualTo(userInfo.LoginId));
                Assert.That(result?.UserId, Is.EqualTo(userInfo.UserId));
                Assert.That(result?.Email, Is.EqualTo(userInfo.Email));
                Assert.That(result?.SubId, Is.EqualTo(userInfo.SubId));
                Assert.That(result?.IsAuthenticated, Is.EqualTo(userInfo.IsAuthenticated));
            });
        }

        [Test]
        public void GetUserInfoMissing_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.GetUserInfo(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(null));
            });
        }

        [Test]
        public void GetUserInfoLoginId_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var userInfoString = JsonConvert.SerializeObject(userInfo);
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, userInfoString);
            var service = new HeaderService();

            //Act
            var result = service.GetUserInfoLoginId(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(userInfo.LoginId));
            });
        }

        [Test]
        public void GetUserInfoLoginIdMissing_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var userInfoNoLoginId = new UserInfo() { UserId = "userId", Email = "email", SubId = "subId", IsAuthenticated = true };
            var userInfoString = JsonConvert.SerializeObject(userInfoNoLoginId);
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, userInfoString);
            var service = new HeaderService();

            //Act
            var result = service.GetUserInfoLoginId(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(null));
            });
        }

        [Test]
        public void IsUserInfoValid_Verify()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var userInfoString = JsonConvert.SerializeObject(userInfo);
            httpContext.Request.Headers.TryAdd(Constants.UserInfoHeaderName, userInfoString);
            var service = new HeaderService();

            //Act
            var result = service.DoesUserInfoHaveInfo(httpContext.Request.Headers);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(true));
            });
        }

        [Test]
        public void IsUserInfoInvalid_Verify()
        {
            //Arrange
            var service = new HeaderService();

            //Act
            var result = service.DoesUserInfoHaveInfo(headerDictionaryMissingHeaders);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(false));
            });
        }
        #endregion
    }
}