using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Models;
using System.Text;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// ServiceException filter to handle exceptions and automatically convert to a ServiceException.
    /// </summary>
    public class ServiceExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// On any exception this method activates and converts it to a service exception.
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            ServiceException serviceException;
            string? transactionId = null;
            context.HttpContext.Request.Headers.TryGetValue(Constants.TransactionIdHeaderName, out var values);
            if (values.Count > 0)
            {
                transactionId = values.First();
            }
            if (context.Exception is not ServiceException)
            {
                serviceException = ServiceExceptionConverter.ConvertTo(context.Exception, transactionId);
            }
            else
            {
                serviceException = context.Exception as ServiceException ?? ServiceExceptionConverter.ConvertTo(context.Exception, transactionId);
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                context.HttpContext.Response.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            }

            context.HttpContext.Response.StatusCode = (int)serviceException.Error.Code;
            var encodedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceException));
            using var stream = new MemoryStream(encodedMessage);
            var buffer = stream.ToArray();
            context.HttpContext.Response.Body.Write(buffer, 0, buffer.Length);
            stream.Dispose();

            base.OnException(context);
        }

        /// <summary>
        /// <inheritdoc cref="OnException(ExceptionContext)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            ServiceException serviceException;
            string? transactionId = null;
            context.HttpContext.Request.Headers.TryGetValue(Constants.TransactionIdHeaderName, out var values);
            if (values.Count > 0)
            {
                transactionId = values.First();
            }
            if (context.Exception is not ServiceException)
            {
                serviceException = ServiceExceptionConverter.ConvertTo(context.Exception, transactionId);
            }
            else
            {
                serviceException = context.Exception as ServiceException ?? ServiceExceptionConverter.ConvertTo(context.Exception, transactionId);
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                context.HttpContext.Response.Headers.TryAdd(Constants.TransactionIdHeaderName, transactionId);
            }

            context.HttpContext.Response.StatusCode = (int)serviceException.Error.Code;
            var encodedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceException));
            using var stream = new MemoryStream(encodedMessage);
            var buffer = stream.ToArray();
            await context.HttpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);

            base.OnException(context);
        }
    }
}
