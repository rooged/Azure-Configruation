using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.ServiceExceptions;
using Roo.Azure.Configuration.Common.Services;

namespace Roo.Azure.Configuration.Common.Middlewares
{
    /// <summary>
    /// Handle custom exceptions.
    /// </summary>
    public class ServiceExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request.</param>
        /// <param name="env">The hosts environment.</param>
        public ServiceExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            this.next = next;
            this.env = env;
        }

        /// <summary>
        /// Invoked if exception occurs during request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="header"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IHeaderService headerService, IRooLogger logger)
        {

            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var serviceException = ServiceExceptionConverter.ConvertTo(ex, headerService.GetTransactionId(context.Request.Headers));
                logger.LogError(context, $"An error occured while handling a request.", serviceException);
                
                //Clear exception details in production
                if (env.IsProduction() && serviceException.Error.Details is { } details)
                {
                    details.Clear();
                }

                //Write exception to response
                context.Response.Clear();
                context.Response.StatusCode = (int)serviceException.Error.Code;
                context.Response.ContentType = "application/json";
                var errorResponse = JsonConvert.SerializeObject(serviceException.Error);

                await context.Response.WriteAsync(errorResponse).ConfigureAwait(false);
            }
        }
    }
}
