using Microsoft.AspNetCore.Builder;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Startup
{
    /// <summary>
    /// Application startup configuration for a web application.
    /// </summary>
    public static class WebApplicationConfiguration
    {
        /// <summary>
        /// WebApplication builder for common library items.
        /// </summary>
        /// <param name="app"></param>
        /// <returns>WebApplication for continued configuration</returns>
        public static WebApplication CommonWebApplication(this WebApplication app, StartupModel model)
        {
            return app;
        }
    }
}
