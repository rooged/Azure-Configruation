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
        /// Initializes a new instance of <see cref="TelemetryInitializer"/>.
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
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                telemetry.Context.GlobalProperties["HttpContextError"] = "Request made without HttpContext available for telemetry to consume. Custom headers unable to be set correctly, used default values.";
            }

            //Sets "SessionId" default
            telemetry.Context.GlobalProperties[Constants.SessionIdHeaderName] = httpContext?.Session.GetString(Constants.SessionId) ?? httpContext?.Session.Id ?? Guid.NewGuid().ToString();

            //Sets "TransactionId" default
            telemetry.Context.GlobalProperties[Constants.TransactionIdHeaderName] = Guid.NewGuid().ToString("N");

            //Sets "ChannelId" from configuration
            if (!string.IsNullOrEmpty(Configuration[Constants.ChannelId]))
            {
                telemetry.Context.GlobalProperties[Constants.ChannelIdHeaderName] = Configuration[Constants.ChannelId];
            }

            //Return since headers and user-info aren't available
            if (httpContext == null)
            {
                return;
            }

            //Sets "UserInfo" default
            if (httpContext.User.Claims.Any())
            {
                var loginId = httpContext.User.FindFirstValue(Constants.UserInfoLoginId);
                if (!string.IsNullOrEmpty(loginId))
                {
                    telemetry.Context.GlobalProperties[Constants.UserInfoLoginId] = loginId;
                }
            }

            //Get headers of HTTP request
            var headers = httpContext.Request.Headers;
            if (headers == null || headers.Count == 0)
            {
                telemetry.Context.GlobalProperties["HttpRequestHeadersError"] = "Request made without headers available for telemetry to consume. Custom headers unable to be set correctly from existing request, used default values.";
                return;
            }

            //Set "SessionId" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.SessionIdHeaderName);

            //Set "TransactionId" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.TransactionIdHeaderName);

            //Set "ChannelId" from header, overrides value from configuration
            SetTelemetryFromHeader(telemetry, headers, Constants.ChannelIdHeaderName);

            //Set "UserInfo" from header
            SetTelemetryFromHeader(telemetry, headers, Constants.UserInfoHeaderName);
        }

        private void SetTelemetryFromHeader(ITelemetry telemetry, IHeaderDictionary headers, string headerName)
        {
            if (!string.IsNullOrEmpty(headers[headerName]))
            {
                telemetry.Context.GlobalProperties.TryGetValue(headerName, out var telemetryValue);
                switch (headerName)
                {
                    case Constants.SessionIdHeaderName:
                        
                        telemetryValue = HeaderService.GetSessionId(headers) ?? telemetryValue ?? Guid.NewGuid().ToString();
                        break;

                    case Constants.TransactionIdHeaderName:
                        telemetryValue = HeaderService.GetTransactionId(headers) ?? telemetryValue;
                        break;

                    case Constants.ChannelIdHeaderName:
                        telemetryValue = HeaderService.GetChannelId(headers) ?? telemetryValue;
                        break;

                    case Constants.UserInfoHeaderName:
                        telemetry.Context.GlobalProperties.TryGetValue(Constants.UserInfoLoginId, out var telemetryLoginIdValue);
                        telemetryValue = HeaderService.GetUserInfoLoginId(headers) ?? telemetryLoginIdValue;
                        headerName = Constants.UserInfoLoginId;
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
