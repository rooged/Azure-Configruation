using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Roo.Azure.Configuration.Common.Logging;
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

        public async Task InvokeAsync(HttpContext context, IHeaderService header, IRooLogger logger)
        {

            try
            {
                await this.next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
