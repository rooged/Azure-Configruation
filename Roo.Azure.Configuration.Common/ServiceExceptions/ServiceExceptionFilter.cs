using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// ServiceException filter to handle exceptions and automatically convert to a ServiceException.
    /// </summary>
    public class ServiceExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// On any exception this method activates and convert it to a service exception.
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

            context.Exception = serviceException;

            context.Result = new JsonResult(serviceException);

            base.OnException(context);
        }
    }
}
