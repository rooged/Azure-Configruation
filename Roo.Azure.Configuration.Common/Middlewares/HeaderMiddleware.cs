using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using System.Net;
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
        /// Initializes a new instance of the <see cref="HeaderMiddleware"/> class
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
            if (context.Request.Path.Value?.Contains("swagger") == true)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            var headers = context.Request.Headers;

            if (!header.IsSessionIdValid(headers) || !header.IsTransactionIdValid(headers) || !header.IsChannelIdValid(headers))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var responseMessage = $"Headers in request: {Models.Constants.SessionIdHeaderName} {header.IsSessionIdValid(headers)}, {Models.Constants.TransactionIdHeaderName} {header.IsTransactionIdValid(headers)}, {Models.Constants.ChannelIdHeaderName} {header.IsChannelIdValid(headers)}";
                var originalStream = context.Response.Body;
                var encodedMessage = Encoding.UTF8.GetBytes(responseMessage);
                using var stream = new MemoryStream(encodedMessage);
                context.Response.Body = stream;
                stream.Dispose();
            }
        }
    }
}
