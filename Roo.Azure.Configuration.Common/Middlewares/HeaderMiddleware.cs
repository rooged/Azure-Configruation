using Microsoft.AspNetCore.Http;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.ServiceExceptions;
using Roo.Azure.Configuration.Common.Services;
using System.Text;

namespace Roo.Azure.Configuration.Common.Middlewares
{
    /// <summary>
    /// Validates that the custom headers are on every HTTP request.
    /// </summary>
    public class HeaderMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request.</param>
        public HeaderMiddleware(RequestDelegate next) => this.next = next;

        /// <summary>
        /// Validates that the custom headers are on every HTTP request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="header"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHeaderService header, IRooLogger logger)
        {
            if (!string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("swagger"))
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            var requestHeaders = context.Request.Headers;

            //Check if standard headers are there and valid
            if (!header.IsSessionIdValid(requestHeaders) || !header.IsTransactionIdValid(requestHeaders) || !header.IsChannelIdValid(requestHeaders))
            {
                var errorCodes = new List<int>();
                if (!header.IsSessionIdValid(requestHeaders))
                {
                    errorCodes.Add((int)ErrorCode.SessionIdHeaderNotFound);
                }
                if (!header.IsTransactionIdValid(requestHeaders))
                {
                    errorCodes.Add((int)ErrorCode.TransactionIdHeaderNotFound);
                }
                if (!header.IsChannelIdValid(requestHeaders))
                {
                    errorCodes.Add((int)ErrorCode.ChannelIdHeaderNotFound);
                }
                context.Response.StatusCode = errorCodes.First();
                var message = $"Error {string.Join(", ", errorCodes)}: Header(s) not found or invalid. {Constants.SessionIdHeaderName} valid: {header.IsSessionIdValid(requestHeaders)}. {Constants.TransactionIdHeaderName} valid: {header.IsTransactionIdValid(requestHeaders)}. {Constants.ChannelIdHeaderName} valid: {header.IsChannelIdValid(requestHeaders)}.";
                var encodedMessage = Encoding.UTF8.GetBytes(message);
                using var stream = new MemoryStream(encodedMessage);
                context.Response.Body.Write(stream.GetBuffer(), 0, (int)stream.Length);
                stream.Dispose();
                logger.LogError(context, message, new ServiceException((ErrorCode)errorCodes.First(), message, null, null, header.GetTransactionId(requestHeaders)));
                return;
            }

            //Set SessionId in HttpContext if its not there
            if (string.IsNullOrEmpty(context.Session.GetString(Constants.SessionId)))
            {
                context.Session.SetString(Constants.SessionId, header.GetSessionId(requestHeaders) ?? "");
            }

            //Check if user is authenticated and if the user info header has been set and is valid
            if (context.User.Claims.Any() && !header.DoesUserInfoHaveInfo(requestHeaders))
            {
                context.Response.StatusCode = (int)ErrorCode.UserInfoHeaderNotFound;
                var message = $"Error {(int)ErrorCode.UserInfoHeaderNotFound}: Header(s) not found or invalid. {Constants.UserInfoHeaderName} valid: {header.DoesUserInfoHaveInfo(requestHeaders)}. {Constants.SessionIdHeaderName} valid: {header.IsSessionIdValid(requestHeaders)}. {Constants.TransactionIdHeaderName} valid: {header.IsTransactionIdValid(requestHeaders)}. {Constants.ChannelIdHeaderName} valid: {header.IsChannelIdValid(requestHeaders)}.";
                var encodedMessage = Encoding.UTF8.GetBytes(message);
                using var stream = new MemoryStream(encodedMessage);
                context.Response.Body.Write(stream.GetBuffer(), 0, (int)stream.Length);
                stream.Dispose();
                logger.LogError(context, message, new ServiceException(ErrorCode.UserInfoHeaderNotFound, message, null, null, header.GetTransactionId(requestHeaders)));
                return;
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
