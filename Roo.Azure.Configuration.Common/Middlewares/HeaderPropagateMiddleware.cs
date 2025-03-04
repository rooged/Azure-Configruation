using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Roo.Azure.Configuration.Common.Middlewares
{
    /// <summary>
    /// Propagate custom headers for all HTTP requests.
    /// </summary>
    public class HeaderPropagateMiddleware : DelegatingHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IConfiguration Configuration { get; }
        private IHeaderService HeaderService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderPropagateMiddleware"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">HTTPContext accessor.</param>
        /// <param name="configuration">Configuration for access envrionment variables.</param>
        /// <param name="headerService">Header service for accessing custom headers.</param>
        public HeaderPropagateMiddleware(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHeaderService headerService)
        {
            HttpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            HeaderService = headerService;
        }

        /// <summary>
        /// Propagate custom headers for all HTTP requests.
        /// </summary>
        /// <param name="request">Current out going HTTP request.</param>
        public void Run(HttpRequestMessage request)
        {
            //Note: HttpContextAccessor.HttpContext.Request is the incoming request from the client, whereas "request" is the outgoing request.

            //Append "SessionId" if not found in a request header
            CheckIfHeaderIsEmpty(request.Headers, Constants.SessionIdHeaderName);

            //Append "TransactionId" if not found in a request header
            CheckIfHeaderIsEmpty(request.Headers, Constants.TransactionIdHeaderName);

            //Append "ChannelId" if not found in a request header
            CheckIfHeaderIsEmpty(request.Headers, Constants.ChannelIdHeaderName);

            //Append "UserInfo" if not found in a request header
            if (HttpContextAccessor.HttpContext != null && HttpContextAccessor.HttpContext.User.Claims.Any() && !request.Headers.TryGetValues(Constants.UserInfoHeaderName, out _))
            {
                var userInfo = new UserInfo()
                {
                    Username = HttpContextAccessor.HttpContext.User.FindFirstValue(Constants.UserInfoUsername) ?? "",
                    Email = HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? "",
                    UserId = HttpContextAccessor.HttpContext.User.FindFirstValue("UserId") ?? "",
                    IsAuthenticated = HttpContextAccessor.HttpContext.User.Identity?.IsAuthenticated ?? false
                };
                request.Headers.Add(Constants.UserInfoHeaderName, JsonConvert.SerializeObject(userInfo));
            }
        }

        private void CheckIfHeaderIsEmpty(HttpRequestHeaders requestHeaders, string headerName)
        {
            requestHeaders.TryGetValues(headerName, out var header);
            if (header == null || string.IsNullOrEmpty(header.FirstOrDefault()))
            {
                switch (headerName)
                {
                    case Constants.SessionIdHeaderName:
                        var sessionId = HeaderService.GetSessionId(HttpContextAccessor.HttpContext?.Request.Headers);
                        if (string.IsNullOrEmpty(HttpContextAccessor.HttpContext?.Session.GetString(Constants.SessionId)))
                        {
                            HttpContextAccessor.HttpContext?.Session.SetString(Constants.SessionId, sessionId ?? HttpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString());
                        }
                        requestHeaders.Add(Constants.SessionIdHeaderName, sessionId ?? HttpContextAccessor.HttpContext?.Session.GetString(Constants.SessionId) ?? HttpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString());
                        break;

                    case Constants.TransactionIdHeaderName:
                        var transactionId = Guid.NewGuid().ToString("N");
                        if (HttpContextAccessor.HttpContext != null)
                        {
                            HttpContextAccessor.HttpContext.TraceIdentifier = transactionId;
                        }
                        requestHeaders.Add(Constants.TransactionIdHeaderName, transactionId);
                        break;

                    case Constants.ChannelIdHeaderName:
                        requestHeaders.Add(Constants.ChannelIdHeaderName, Configuration[Constants.ChannelId]);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Run(request);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
