using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using System.Text.RegularExpressions;
using System.Web;

namespace Roo.Azure.Configuration.Common.Logging
{
    /// <summary>
    /// ILogger wrapper to add custom headers to logs, allowing better tracking in App Insights.
    /// <list type="table">session-id: Stays constant throughout a session.<br/>
    /// transaction-id: Unique to each HTTP request.<br/>
    /// channel-id: Where a request is coming from.</list>
    /// HttpContext is required since the headers are default in context. If used for logging in non-HTTP applications, such as batch jobs, use DefaultHttpContext in startup to set headers.
    /// </summary>
    public interface IRooLogger
    {
        /// <summary>
        /// Create formatted log at the information level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogInformation(HttpContext context, string? message = null, Exception? ex = null);

        /// <summary>
        /// Create formatted log at the error level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogError(HttpContext context, string? message = null, Exception? ex = null);

        /// <summary>
        /// Create formatted log at the warning level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogWarning(HttpContext context, string? message = null, Exception? ex = null);

        /// <summary>
        /// Create formatted log at the trace level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogTrace(HttpContext context, string? message = null, Exception? ex = null);

        /// <summary>
        /// Create formatted log at the critical level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogCritical(HttpContext context, string? message = null, Exception? ex = null);

        /// <summary>
        /// Create formatted log at the debug level
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogDebug(HttpContext context, string? message = null, Exception? ex = null);
    }

    /// <summary>
    /// Implementation of IRooLogger
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="logger"></param>
    /// <param name="headerService"></param>
    public partial class RooLogger(ILogger<RooLogger> logger, IHeaderService headerService) : IRooLogger
    {
        private readonly ILogger<RooLogger> _logger = logger;
        private readonly IHeaderService _headerService = headerService;

        private const string encodePattern = @"\s+";

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="ex">Exception</param>
        public void LogInformation(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogInformation(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            } else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers)) {
                _logger.LogInformation(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            } else if (!string.IsNullOrEmpty(message)) {
                _logger.LogInformation(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            } else
            {
                _logger.LogInformation(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="ex">Exception</param>
        public void LogError(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogError(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers))
            {
                _logger.LogError(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            }
            else if (!string.IsNullOrEmpty(message))
            {
                _logger.LogError(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else
            {
                _logger.LogError(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogWarning(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogWarning(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers))
            {
                _logger.LogWarning(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            }
            else if (!string.IsNullOrEmpty(message))
            {
                _logger.LogWarning(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else
            {
                _logger.LogWarning(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        /// <summary>
        /// Log trace
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogTrace(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogTrace(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers))
            {
                _logger.LogTrace(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            }
            else if (!string.IsNullOrEmpty(message))
            {
                _logger.LogTrace(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else
            {
                _logger.LogTrace(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        /// <summary>
        /// Log critical
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogCritical(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogCritical(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers))
            {
                _logger.LogCritical(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            }
            else if (!string.IsNullOrEmpty(message))
            {
                _logger.LogCritical(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else
            {
                _logger.LogCritical(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="message">Log message</param>
        /// <param name="e">Exception</param>
        public void LogDebug(HttpContext context, string? message = null, Exception? ex = null)
        {
            if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers) && !string.IsNullOrEmpty(message))
            {
                _logger.LogDebug(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else if (_headerService.DoesUserInfoHaveInfo(context.Request.Headers))
            {
                _logger.LogDebug(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, {UserInfoHeaderName}: {GetUserInfo}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), Constants.UserInfoHeaderName, JsonConvert.SerializeObject(_headerService.GetUserInfo(context.Request.Headers)));
            }
            else if (!string.IsNullOrEmpty(message))
            {
                _logger.LogDebug(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}, message: {message}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers), HttpUtility.HtmlEncode(EncodeLogRegex().Replace(message ?? "", " ")));
            }
            else
            {
                _logger.LogDebug(ex, "{SessionIdHeaderName}: {GetSessionId}, {TransactionIdHeaderName}: {GetTransactionId}, {ChannelIdHeaderName}: {GetChannelId}",
                    Constants.SessionIdHeaderName, _headerService.GetSessionId(context.Request.Headers), Constants.TransactionIdHeaderName, _headerService.GetTransactionId(context.Request.Headers), Constants.ChannelIdHeaderName,
                    _headerService.GetChannelId(context.Request.Headers));
            }
        }

        [GeneratedRegex(encodePattern)]
        private static partial Regex EncodeLogRegex();
    }
}
