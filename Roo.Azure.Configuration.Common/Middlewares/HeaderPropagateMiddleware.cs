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
    /// Propagate custom headers for all outgoing HTTP requests, not just ones that go through RooClient.<br/>
    /// Additional headers can be configured with HeaderPropagateOptions.<br/>
    /// Additional header values set in order of importance: HttpConext.Request.Headers, Configuration, HttpContext.Session.
    /// </summary>
    public class HeaderPropagateMiddleware : DelegatingHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IConfiguration Configuration { get; }
        private IHeaderService HeaderService { get; }
        private HeaderPropagateOptions _headerPropagateOptions { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderPropagateMiddleware"/>.
        /// </summary>
        /// <param name="httpContextAccessor">HTTPContext accessor.</param>
        /// <param name="configuration">Configuration for access envrionment variables.</param>
        /// <param name="headerService">Header service for accessing custom headers.</param>
        public HeaderPropagateMiddleware(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHeaderService headerService, HeaderPropagateOptions? headerPropagateOptions = null)
        {
            HttpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            HeaderService = headerService;
            _headerPropagateOptions = headerPropagateOptions ?? new HeaderPropagateOptions();
        }

        /// <summary>
        /// Propagate custom headers for all HTTP requests.
        /// </summary>
        /// <param name="request">Current out going HTTP request.</param>
        public void Run(HttpRequestMessage request)
        {
            //Note: HttpContextAccessor.HttpContext.Request is the incoming request from the client, whereas "request" is the outgoing request.

            foreach (var header in _headerPropagateOptions.Headers)
            {
                CheckIfHeaderIsEmpty(request.Headers, header);
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
                        var channelId = Configuration[Constants.ChannelId];
                        if (!string.IsNullOrEmpty(channelId))
                        {
                            requestHeaders.Add(Constants.ChannelIdHeaderName, Configuration[Constants.ChannelId]);
                        }
                        break;

                    case Constants.UserInfoHeaderName:
                        if (HttpContextAccessor.HttpContext != null && HttpContextAccessor.HttpContext.User.Claims.Any() && !requestHeaders.TryGetValues(Constants.UserInfoHeaderName, out _))
                        {
                            var userInfo = HeaderService.GetUserInfo(HttpContextAccessor.HttpContext.Request.Headers);
                            if (userInfo == null)
                            {
                                userInfo = new UserInfo()
                                {
                                    LoginId = HttpContextAccessor.HttpContext.User.FindFirstValue(Constants.UserInfoLoginId) ?? "",
                                    Email = HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? "",
                                    UserId = HttpContextAccessor.HttpContext.User.FindFirstValue("UserId") ?? "",
                                    IsAuthenticated = HttpContextAccessor.HttpContext.User.Identity?.IsAuthenticated ?? false
                                };
                            }
                            requestHeaders.Add(Constants.UserInfoHeaderName, JsonConvert.SerializeObject(userInfo));
                        }
                        break;

                    default:
                        if (HttpContextAccessor.HttpContext != null)
                        {
                            string? headerValue = null;
                            if (HttpContextAccessor.HttpContext.Request.Headers.TryGetValue(headerName, out var headerStringValue))
                            {
                                if (headerStringValue.Count > 1)
                                {
                                    requestHeaders.Add(headerName, (IEnumerable<string?>)headerStringValue);
                                }
                                else if (headerStringValue.Count > 0)
                                {
                                    headerValue = headerStringValue.First();
                                }
                            }
                            requestHeaders.Add(headerName, headerValue ?? Configuration[headerName] ?? HttpContextAccessor.HttpContext.Session.GetString(headerName));
                        }
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
