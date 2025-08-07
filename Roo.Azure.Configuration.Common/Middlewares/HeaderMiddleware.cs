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
            var errorCodes = new List<int>();
            var sessionIdValid = header.IsSessionIdValid(requestHeaders);
            if (!sessionIdValid)
            {
                errorCodes.Add((int)ErrorCode.SessionIdHeaderNotFound);
            }
            var transactionIdValid = header.IsTransactionIdValid(requestHeaders);
            if (!transactionIdValid)
            {
                errorCodes.Add((int)ErrorCode.TransactionIdHeaderNotFound);
            }
            var channelIdValid = header.IsChannelIdValid(requestHeaders);
            if (!channelIdValid)
            {
                errorCodes.Add((int)ErrorCode.ChannelIdHeaderNotFound);
            }
            if (errorCodes.Count > 0)
            {
                context.Response.StatusCode = errorCodes.First();
                var message = $"Error {string.Join(", ", errorCodes)}: Header(s) not found or invalid. {Constants.SessionIdHeaderName} valid: {sessionIdValid}. {Constants.TransactionIdHeaderName} valid: {transactionIdValid}. {Constants.ChannelIdHeaderName} valid: {channelIdValid}.";
                var encodedMessage = Encoding.UTF8.GetBytes(message);
                using var stream = new MemoryStream(encodedMessage);
                var buffer = stream.ToArray();
                await context.Response.Body.WriteAsync(buffer);
                await stream.DisposeAsync();
                logger.LogError(context, message, new ServiceException((ErrorCode)errorCodes.First(), message, null, null, header.GetTransactionId(requestHeaders)));
                return;
            }

            //Set SessionId in HttpContext if its not there
            if (string.IsNullOrEmpty(context.Session.GetString(Constants.SessionId)))
            {
                context.Session.SetString(Constants.SessionId, header.GetSessionId(requestHeaders) ?? "");
            }

            //Check if user is authenticated and if the user info header has been set and is valid
            var useHasInfo = header.DoesUserInfoHaveInfo(requestHeaders);
            if (context.User.Claims.Any() && !useHasInfo)
            {
                //context.Response.StatusCode = (int)ErrorCode.UserInfoHeaderNotFound;
                var message = $"Error {(int)ErrorCode.UserInfoHeaderNotFound}: Header(s) not found or invalid. {Constants.UserInfoHeaderName} valid: {useHasInfo}. {Constants.SessionIdHeaderName} valid: {sessionIdValid}. {Constants.TransactionIdHeaderName} valid: {transactionIdValid}. {Constants.ChannelIdHeaderName} valid: {channelIdValid}.";
                //var encodedMessage = Encoding.UTF8.GetBytes(message);
                //using var stream = new MemoryStream(encodedMessage);
                //var buffer = stream.ToArray();
                //await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                //await stream.DisposeAsync();
                logger.LogError(context, message, new ServiceException(ErrorCode.UserInfoHeaderNotFound, message, null, null, header.GetTransactionId(requestHeaders)));
                return;
            }

            await next(context).ConfigureAwait(false);
        }
    }
}
