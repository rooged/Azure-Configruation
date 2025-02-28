using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using System.Security.Claims;

namespace Roo.Azure.Configuration.Common.Telemetry
{
    /// <summary>
    /// Azure App Insights telemetry configuration.<br/>
    /// Sets up headers that will be appended to all data sent to App Insights for tracking.
    /// </summary>
    public class TelemetryInitializer : ITelemetryInitializer
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IConfiguration Configuration { get; }
        private IHeaderService HeaderService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryInitializer"/> class.
        /// Initializer can't have a reference to ILogger as it will create a circular dependency.
        /// </summary>
        /// <param name="httpContextAccessor">HTTPContext accessor.</param>
        /// <param name="configuration">Configuration for access envrionment variables.</param>
        /// <param name="headerService">Header service for accessing custom headers.</param>
        public TelemetryInitializer(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHeaderService headerService)
        {
            HttpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            HeaderService = headerService;
        }

        /// <summary>
        /// Sets up headers that will be appended to all data sent to App Insights for tracking.
        /// </summary>
        /// <param name="telemetry"></param>
        public void Initialize(ITelemetry telemetry)
        {
            //Sets "SessionId" default
            telemetry.Context.GlobalProperties[Constants.SessionIdHeaderName] = Guid.NewGuid().ToString();

            //Sets "TransactionId" default
            telemetry.Context.GlobalProperties[Constants.TransactionIdHeaderName] = Guid.NewGuid().ToString("N");

            //Sets "ChannelId" from configuration
            if (!string.IsNullOrEmpty(Configuration[Constants.ChannelId]))
            {
                telemetry.Context.GlobalProperties[Constants.ChannelIdHeaderName] = Configuration[Constants.ChannelId];
            }

            //Sets "UserInfo" default
            if (HttpContextAccessor?.HttpContext != null && HttpContextAccessor.HttpContext.User.Claims.Any())
            {
                telemetry.Context.GlobalProperties[Constants.UserInfoUsername] = HttpContextAccessor.HttpContext.User.FindFirstValue(Constants.UserInfoUsername) ?? "";
            }

            //Get headers of HTTP request
            var headers = HttpContextAccessor?.HttpContext?.Request.Headers;
            if (headers == null)
            {
                telemetry.Context.GlobalProperties["HttpContextError"] = "Request made without HttpContext available for telemetry to consume. Custom headers unable to be set correctly, used default values.";
                return;
            }

            //Set "SessionId" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.SessionIdHeaderName);

            //Set "TransactionId" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.TransactionIdHeaderName);

            //Set "ChannelId" from header, overrides value from configuration
            SetTelemetryFromHeader(telemetry, headers, Constants.ChannelIdHeaderName);

            //Set "UserInfo" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.UserInfoUsername);
        }

        private void SetTelemetryFromHeader(ITelemetry telemetry, IHeaderDictionary headers, string headerName)
        {
            if (!string.IsNullOrEmpty(headers[headerName]))
            {
                var telemetryValue = "";
                switch (headerName)
                {
                    case Constants.SessionIdHeaderName:
                        telemetryValue = HeaderService.GetSessionId(headers) ?? HttpContextAccessor.HttpContext?.Session.GetString(Constants.SessionId) ?? HttpContextAccessor.HttpContext?.Session.Id;
                        break;

                    case Constants.TransactionIdHeaderName:
                        telemetryValue = HeaderService.GetTransactionId(headers);
                        break;

                    case Constants.ChannelIdHeaderName:
                        telemetryValue = HeaderService.GetChannelId(headers);
                        break;

                    case Constants.UserInfoUsername:
                        telemetryValue = HeaderService.GetUserInfoUsername(headers);
                        break;

                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(telemetryValue))
                {
                    telemetry.Context.GlobalProperties[headerName] = telemetryValue;
                }
            }
        }
    }
}
