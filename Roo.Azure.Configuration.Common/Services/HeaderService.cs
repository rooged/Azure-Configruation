using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Services
{
    /// <summary>
    /// Handles the custom headers in HttpContext. Headers: session-id, transaction-id, channel-id, user-info.
    /// </summary>
    public interface IHeaderService
    {
        /// <summary>
        /// Get session-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>session-id value</returns>
        public string? GetSessionId(IHeaderDictionary? headers);

        /// <summary>
        /// Check if session-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsSessionIdValid(IHeaderDictionary? headers);

        /// <summary>
        /// Get transaction-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>transaction-id value</returns>
        public string? GetTransactionId(IHeaderDictionary? headers);

        /// <summary>
        /// Check if transaction-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsTransactionIdValid(IHeaderDictionary? headers);

        /// <summary>
        /// Get channel-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>channel-id value</returns>
        public string? GetChannelId(IHeaderDictionary? headers);

        /// <summary>
        /// Check if channel-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsChannelIdValid(IHeaderDictionary? headers);

        /// <summary>
        /// Get username from user-info header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>username value</returns>
        public string? GetUserInfoUsername(IHeaderDictionary? headers);

        /// <summary>
        /// Get user-info header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>user-info value</returns>
        public UserInfo? GetUserInfo(IHeaderDictionary? headers);

        /// <summary>
        /// Check if user-info is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool DoesUserInfoHaveInfo(IHeaderDictionary? headers);
    }

    /// <summary>
    /// Implementation of IHeaderService
    /// </summary>
    public class HeaderService : IHeaderService
    {
        /// <summary>
        /// Get session-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>session-id value</returns>
        public string? GetSessionId(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return ExtractHeader(headers, Constants.SessionIdHeaderName);
            }

            return null;
        }

        /// <summary>
        /// Check if session-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsSessionIdValid(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return !string.IsNullOrEmpty(ExtractHeader(headers, Constants.SessionIdHeaderName));
            }

            return false;
        }

        /// <summary>
        /// Get transaction-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>transaction-id value</returns>
        public string? GetTransactionId(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return ExtractHeader(headers, Constants.TransactionIdHeaderName);
            }

            return null;
        }

        /// <summary>
        /// Check if transaction-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsTransactionIdValid(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return !string.IsNullOrEmpty(ExtractHeader(headers, Constants.TransactionIdHeaderName));
            }

            return false;
        }

        /// <summary>
        /// Get channel-id header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>channel-id value</returns>
        public string? GetChannelId(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return ExtractHeader(headers, Constants.ChannelIdHeaderName);
            }

            return null;
        }

        /// <summary>
        /// Check if channel-id is valid
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool IsChannelIdValid(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return !string.IsNullOrEmpty(ExtractHeader(headers, Constants.ChannelIdHeaderName));
            }

            return false;
        }

        /// <summary>
        /// Get username from user-info header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>username value</returns>
        public string? GetUserInfoUsername(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                var userInfo = ExtractUserInfoHeader(headers);

                if (userInfo != null)
                {
                    return userInfo.Username;
                }
            }

            return null;
        }

        /// <summary>
        /// Get user-info header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>user-info value</returns>
        public UserInfo? GetUserInfo(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                return ExtractUserInfoHeader(headers);
            }

            return null;
        }

        /// <summary>
        /// Check if user-info has any info
        /// </summary>
        /// <param name="headers"></param>
        /// <returns>boolean</returns>
        public bool DoesUserInfoHaveInfo(IHeaderDictionary? headers)
        {
            if (headers != null)
            {
                var userInfo = ExtractUserInfoHeader(headers);
                return userInfo != null && userInfo.HasInfo;
            }

            return false;
        }

        private static string? ExtractHeader(IHeaderDictionary headers, string headerName)
        {
            if (headers.TryGetValue(headerName, out var values) || values.Count < 1)
            {
                return null;
            }

            return values.First();
        }

        private static UserInfo? ExtractUserInfoHeader(IHeaderDictionary headers)
        {
            if (headers.TryGetValue(Constants.UserInfoHeaderName, out var values) || values.Count < 1)
            {
                return null;
            }

            var userInfo = values.FirstOrDefault();

            try
            {
                if (!string.IsNullOrEmpty(userInfo))
                {
                    return JsonConvert.DeserializeObject<UserInfo>(userInfo);
                }
            } catch
            {
                return null;
            }

            return null;
        }
    }
}
