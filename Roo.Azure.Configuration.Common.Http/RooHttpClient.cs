using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Roo.Azure.Configuration.Common.Http.AzureAdAuthentication;
using Roo.Azure.Configuration.Common.Http.Models;
using Roo.Azure.Configuration.Common.Logging;
using Roo.Azure.Configuration.Common.Models;
using Roo.Azure.Configuration.Common.Services;
using System.Data;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Roo.Azure.Configuration.Common.Http
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class RooHttpClient : IRooHttpClient
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public HttpContext? HttpContext { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? SessionId { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? TransactionId { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? ChannelId { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public UserInfo? UserInfo { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public AuthenticationInfo? AuthenticationInfo { get; set; } = null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SerializationSettings? SerializationSettings { get; set; }

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly string _channelId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRooLogger _logger;
        private readonly IAzureAdClientAssertion _azureAdClientAssertion;
        private readonly IRedisService? _redisService;
        private readonly IMemoryCache? _cache;

        /// <summary>
        /// Initialize class with configuration
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        /// <param name="redisService"></param>
        public RooHttpClient(string channelId, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IRooLogger logger, IAzureAdClientAssertion azureAdClientAssertion)
        {
            _channelId = channelId;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _azureAdClientAssertion = azureAdClientAssertion;
        }

        /// <summary>
        /// Initialize class with configuration with Redis
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        /// <param name="redisService"></param>
        public RooHttpClient(string channelId, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IRooLogger logger, IAzureAdClientAssertion azureAdClientAssertion, IRedisService redisService)
        {
            _channelId = channelId;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _azureAdClientAssertion = azureAdClientAssertion;
            _redisService = redisService;
        }

        /// <summary>
        /// Initialize class with configuration with in memory cache
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        /// <param name="redisService"></param>
        public RooHttpClient(string channelId, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IRooLogger logger, IAzureAdClientAssertion azureAdClientAssertion, IMemoryCache cache)
        {
            _channelId = channelId;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _azureAdClientAssertion = azureAdClientAssertion;
            _cache = cache;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            if (queryParameters != null)
            {
                relativeUrl += ToQueryString(queryParameters);
            }
            var httpClient = await HttpClientSetup(httpClientName).ConfigureAwait(false);
            var response = await httpClient.GetAsync(relativeUrl).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && AuthenticationInfo != null)
            {
                AddBearerToken(await GetToken(httpClientName, true).ConfigureAwait(false) ?? "", httpClient);
                response = await httpClient.GetAsync(relativeUrl).ConfigureAwait(false);
            }
            return response;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryStringObject"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string relativeUrl, string httpClientName, object queryStringObject)
        {
            relativeUrl += ToQueryString(queryStringObject);
            var response = await GetAsync(relativeUrl, httpClientName).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            var response = await GetAsync(relativeUrl, httpClientName, queryParameters).ConfigureAwait(false);
            return await DeserializeResult<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryStringObject"></param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string relativeUrl, string httpClientName, object queryStringObject)
        {
            relativeUrl += ToQueryString(queryStringObject);
            var response = await GetAsync(relativeUrl, httpClientName).ConfigureAwait(false);
            return await DeserializeResult<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await GetAsync(relativeUrl, httpClientName.ToString(), queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryStringObject"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string relativeUrl, Enum httpClientName, object queryStringObject)
        {
            return await GetAsync(relativeUrl, httpClientName.ToString(), queryStringObject).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await GetAsync<TResult>(relativeUrl, httpClientName.ToString(), queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryStringObject"></param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string relativeUrl, Enum httpClientName, object queryStringObject)
        {
            return await GetAsync<TResult>(relativeUrl, httpClientName.ToString(), queryStringObject).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            HttpContent? content = null;
            if (data != null)
            {
                if (data is not HttpContent && data is not MultipartFormDataContent && data is not MultipartContent)
                {
                    content = new StringContent(JsonConvert.SerializeObject(data));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }
                else
                {
                    if (data is HttpContent)
                    {
                        content = (HttpContent?)data;
                    }
                    else if (data is MultipartFormDataContent)
                    {
                        content = (MultipartFormDataContent?)data;
                    }
                    else if (data is MultipartContent)
                    {
                        content = (MultipartContent?)data;
                    }
                }
            }
            if (queryParameters != null)
            {
                relativeUrl += ToQueryString(queryParameters);
            }
            var httpClient = await HttpClientSetup(httpClientName).ConfigureAwait(false);
            var response = await httpClient.PostAsync(relativeUrl, content).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && AuthenticationInfo != null)
            {
                AddBearerToken(await GetToken(httpClientName, true).ConfigureAwait(false) ?? "", httpClient);
                response = await httpClient.PostAsync(relativeUrl, content).ConfigureAwait(false);
            }
            return response;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            var response = await PostAsync(relativeUrl, httpClientName, data, queryParameters).ConfigureAwait(false);
            return await DeserializeResult<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await PostAsync(relativeUrl, httpClientName.ToString(), data, queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await PostAsync<TResult>(relativeUrl, httpClientName.ToString(), data, queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            HttpContent? content = null;
            if (data != null)
            {
                if (data is not HttpContent && data is not MultipartFormDataContent && data is not MultipartContent)
                {
                    content = new StringContent(JsonConvert.SerializeObject(data));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }
                else
                {
                    if (data is HttpContent)
                    {
                        content = (HttpContent?)data;
                    }
                    else if (data is MultipartFormDataContent)
                    {
                        content = (MultipartFormDataContent?)data;
                    }
                    else if (data is MultipartContent)
                    {
                        content = (MultipartContent?)data;
                    }
                }
            }
            if (queryParameters != null)
            {
                relativeUrl += ToQueryString(queryParameters);
            }
            var httpClient = await HttpClientSetup(httpClientName).ConfigureAwait(false);
            var response = await httpClient.PutAsync(relativeUrl, content).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && AuthenticationInfo != null)
            {
                AddBearerToken(await GetToken(httpClientName, true).ConfigureAwait(false) ?? "", httpClient);
                response = await httpClient.PutAsync(relativeUrl, content).ConfigureAwait(false);
            }
            return response;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> PutAsync<TResult>(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            var response = await PutAsync(relativeUrl, httpClientName, data, queryParameters).ConfigureAwait(false);
            return await DeserializeResult<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await PutAsync(relativeUrl, httpClientName.ToString(), data, queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="data"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> PutAsync<TResult>(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await PutAsync<TResult>(relativeUrl, httpClientName.ToString(), data, queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            if (queryParameters != null)
            {
                relativeUrl += ToQueryString(queryParameters);
            }
            var httpClient = await HttpClientSetup(httpClientName).ConfigureAwait(false);
            var response = await httpClient.DeleteAsync(relativeUrl).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && AuthenticationInfo != null)
            {
                AddBearerToken(await GetToken(httpClientName, true).ConfigureAwait(false) ?? "", httpClient);
                response = await httpClient.DeleteAsync(relativeUrl).ConfigureAwait(false);
            }
            return response;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> DeleteAsync<TResult>(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            var response = await DeleteAsync(relativeUrl, httpClientName, queryParameters).ConfigureAwait(false);
            return await DeserializeResult<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await DeleteAsync(relativeUrl, httpClientName.ToString(), queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="httpClientName"></param>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<TResult> DeleteAsync<TResult>(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null)
        {
            return await DeleteAsync<TResult>(relativeUrl, httpClientName.ToString(), queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tokenName"></param>
        /// <returns></returns>
        public async Task<bool> ClearToken(string? tokenName = null)
        {
            if (_redisService == null && _cache == null)
            {
                return false;
            }
            var key = tokenName ?? AuthenticationInfo?.TokenName;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            if (_redisService != null)
            {
                return await _redisService.Delete(key).ConfigureAwait(false);
            }
            if (_cache != null)
            {
                _cache.Remove(key);
                return true;
            }
            return false;
        }

        private async Task<HttpClient> HttpClientSetup(string httpClientName)
        {
            if (HttpContext == null)
            {
                HttpContext = _httpContextAccessor.HttpContext;
            }

            var httpClient = CreateHttpClient(httpClientName);

            AddBearerToken(await GetToken(httpClientName).ConfigureAwait(false) ?? "", httpClient);

            AddHeaderParameters(httpClient);

            return httpClient;
        }

        private HttpClient CreateHttpClient(string httpClientName)
        {
            return _httpClientFactory.CreateClient(httpClientName);
        }

        private async Task<TResult> DeserializeResult<TResult>(HttpResponseMessage response)
        {
            if (SerializationSettings != null && SerializationSettings.JsonSerializerOptions != null)
            {
                try
                {
                    if (SerializationSettings.ReadAsString)
                    {
                        var contentString = await response.Content.SafeReadAsStringAsync().ConfigureAwait(false);
                        if (string.IsNullOrEmpty(contentString))
                        {
                            return default!;
                        }
                        return System.Text.Json.JsonSerializer.Deserialize<TResult>(contentString, SerializationSettings.JsonSerializerOptions) ?? default!;
                    }
                    var contentStream = await response.Content.SafeReadAsStreamAsync().ConfigureAwait(false);
                    if (contentStream == null)
                    {
                        return default!;
                    }
                    return System.Text.Json.JsonSerializer.Deserialize<TResult>(contentStream, SerializationSettings.JsonSerializerOptions) ?? default!;
                }
                catch
                {
                    return default!;
                }
            }

            JsonSerializerSettings? serializerSettings = null;
            if (SerializationSettings != null && SerializationSettings.UseDefaultSerializationSettings)
            {
                serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    NullValueHandling = NullValueHandling.Ignore
                };
                serializerSettings.Converters.Add(new StringEnumConverter());
            }

            try
            {
                if (SerializationSettings != null && SerializationSettings.ReadAsString)
                {
                    var contentString = await response.Content.SafeReadAsStringAsync().ConfigureAwait(false);
                    if (string.IsNullOrEmpty(contentString))
                    {
                        return default!;
                    }
                    return JsonConvert.DeserializeObject<TResult>(contentString, SerializationSettings.JsonSerializerSettings ?? serializerSettings) ?? default!;
                }
                using (var json = new JsonTextReader(new StreamReader(await response.Content.SafeReadAsStreamAsync().ConfigureAwait(false) ?? default!)))
                {
                    var serializer = JsonSerializer.Create(SerializationSettings?.JsonSerializerSettings ?? serializerSettings);
                    return serializer.Deserialize<TResult>(json) ?? default!;
                }
            }
            catch
            {
                return default!;
            }
        }

        private void AddHeaderParameters(HttpClient httpClient)
        {
            if (AuthenticationInfo != null)
            {
                if (AuthenticationInfo.AdditionalRequestHeaders != null)
                {
                    foreach (var header in AuthenticationInfo.AdditionalRequestHeaders)
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
                    }
                }

                if (AuthenticationInfo != null && (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimCertificate || AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimClientSecret))
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", AuthenticationInfo.ApimSubscriptionKey);
                }
            }

            //Incoming request headers
            var incomingHeaders = HttpContext?.Request.Headers;

            //Add session-id from incoming request to outgoing request
            var values = new StringValues();
            incomingHeaders?.TryGetValue(Constants.SessionIdHeaderName, out values);
            httpClient.DefaultRequestHeaders.Remove(Constants.SessionIdHeaderName);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Constants.SessionIdHeaderName, SessionId ?? (values.Count > 0 ? values[0] : null) ?? _httpContextAccessor.HttpContext?.Session.GetString(Constants.SessionId) ?? _httpContextAccessor.HttpContext?.Session.Id ?? Guid.NewGuid().ToString());

            //Add new transaction-id to outgoing request
            httpClient.DefaultRequestHeaders.Remove(Constants.TransactionIdHeaderName);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Constants.TransactionIdHeaderName, TransactionId ?? Guid.NewGuid().ToString("N"));

            //Add channel-id of current application to outgoing request
            httpClient.DefaultRequestHeaders.Remove(Constants.ChannelIdHeaderName);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Constants.ChannelIdHeaderName, ChannelId ?? _channelId);

            //Add user-info from incoming request to outgoing request or if the current application has claims
            if (UserInfo != null || (HttpContext != null && HttpContext.User.Claims.Any()))
            {
                var userInfoValues = new StringValues();
                incomingHeaders?.TryGetValue(Constants.UserInfoHeaderName, out userInfoValues);

                UserInfo? userInfo = null;
                if (userInfoValues.Count == 0 || UserInfo != null)
                {
                    userInfo = new()
                    {
                        LoginId = UserInfo?.LoginId ?? HttpContext?.User.FindFirstValue(Constants.UserInfoLoginId),
                        Email = UserInfo?.Email ?? HttpContext?.User.FindFirstValue(ClaimTypes.Email),
                        UserId = UserInfo?.UserId,
                        SubId = UserInfo?.SubId,
                        IsAuthenticated = UserInfo?.IsAuthenticated ?? HttpContext?.User.Identity?.IsAuthenticated ?? false
                    };
                }
                httpClient.DefaultRequestHeaders.Remove(Constants.UserInfoHeaderName);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Constants.UserInfoHeaderName, userInfo != null ? JsonConvert.SerializeObject(userInfo) : userInfoValues[0]);
            }
        }

        private void AddBearerToken(string token, HttpClient client)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string?> GetToken(string httpClientName, bool forceToken = false)
        {
            if (AuthenticationInfo == null || string.IsNullOrEmpty(AuthenticationInfo.TokenName))
            {
                return null;
            }

            if (AuthenticationInfo.ForceToken || forceToken)
            {
                _logger.LogInformation(HttpContext, "Deleting stored token and requesting a new one.");
                await ClearToken().ConfigureAwait(false);
            }

            await _semaphoreSlim.WaitAsync();

            string? token = null;
            if (_redisService != null)
            {
                token = await _redisService.Get(AuthenticationInfo.TokenName ?? httpClientName);
            }
            else if (_cache != null)
            {
                token = _cache.Get<string>(AuthenticationInfo.TokenName ?? httpClientName);
            }

            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation(HttpContext, "Token not found, creating new one.");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.OAuth)
                    {
                        token = await GetTokenOAuth(httpClientName);
                    }
                    else if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimCertificate || AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimClientSecret)
                    {
                        token = await GetTokenApim(httpClientName);
                    }
                    else if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.Basic)
                    {
                        token = await GetTokenBasic(httpClientName);
                    }
                    else if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.PassInToken)
                    {
                        token = AuthenticationInfo.Token;
                    }
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
            return token;
        }

        private async Task<string?> GetTokenOAuth(string httpClientName)
        {
            if (AuthenticationInfo == null || string.IsNullOrEmpty(AuthenticationInfo.LoginId) || string.IsNullOrEmpty(AuthenticationInfo.Password) || string.IsNullOrEmpty(AuthenticationInfo.TokenUrl))
            {
                return null;
            }

            var httpClient = CreateHttpClient(AuthenticationInfo.AuthenticationHttpClientName ?? httpClientName);
            if (AuthenticationInfo.AuthenticationHeaders != null)
            {
                foreach (var header in AuthenticationInfo.AuthenticationHeaders)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
                }
            }

            var content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("username", AuthenticationInfo.LoginId),
                new KeyValuePair<string, string>("password", AuthenticationInfo.Password),
                new KeyValuePair<string, string>("grant_type", "password"),
            ]);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var tokenResponse = await httpClient.PostAsync(AuthenticationInfo.TokenUrl, content);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var token = await tokenResponse.Content.ReadAsAsync<OAuthTokenResponse>();
                if (token != null && !string.IsNullOrEmpty(token.Access_Token))
                {
                    var cacheExpiration = TimeSpan.FromSeconds(token.Expires_In - 60);
                    if (_redisService != null)
                    {
                        await _redisService.Set(AuthenticationInfo.TokenName ?? httpClientName, token.Access_Token, cacheExpiration);
                        _logger.LogInformation(HttpContext, $"OAuth token stored and expires in: {cacheExpiration}.");
                    }
                    else if (_cache != null)
                    {
                        _cache.Set(AuthenticationInfo.TokenName ?? httpClientName, token.Access_Token, cacheExpiration);
                        _logger.LogInformation(HttpContext, $"OAuth token stored and expires in: {cacheExpiration}.");
                    }
                    return token.Access_Token;
                }
            }
            _logger.LogError(HttpContext, $"Error getting OAuth token.");
            return null;
        }

        private async Task<string?> GetTokenApim(string httpClientName)
        {
            if (AuthenticationInfo == null || string.IsNullOrEmpty(AuthenticationInfo.LoginId) || string.IsNullOrEmpty(AuthenticationInfo.ApimTenantId) || string.IsNullOrEmpty(AuthenticationInfo.ApimScope) ||
                string.IsNullOrEmpty(AuthenticationInfo.ApimSubscriptionKey))
            {
                return null;
            }

            string? token = null;
            if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimCertificate)
            {
                X509Certificate2? cert = null;
                if (!string.IsNullOrEmpty(AuthenticationInfo.ApimCertificateName) && !string.IsNullOrEmpty(AuthenticationInfo.ApimAzureKeyVaultUri))
                {
                    cert = await _azureAdClientAssertion.GetCertificateFromKeyVault(AuthenticationInfo.ApimCertificateName, AuthenticationInfo.ApimAzureKeyVaultUri, AuthenticationInfo.ApimAzureToken);
                }
                else if (AuthenticationInfo.ApimCertificate != null)
                {
                    cert = AuthenticationInfo.ApimCertificate;
                }
                else if (AuthenticationInfo.ApimCertificateStringAndPassword != null && !string.IsNullOrEmpty(AuthenticationInfo.ApimCertificateStringAndPassword.Value.Certificate) && !string.IsNullOrEmpty(AuthenticationInfo.ApimCertificateStringAndPassword.Value.Password))
                {
                    cert = _azureAdClientAssertion.GenerateCertificateFromString(AuthenticationInfo.ApimCertificateStringAndPassword.Value.Certificate, AuthenticationInfo.ApimCertificateStringAndPassword.Value.Password);
                }
                if (cert == null)
                {
                    _logger.LogInformation(HttpContext, $"Downloading APIM certificate from Azure Key Vault failed. Certificate not found in key vault or unable to download.");
                    return null;
                }
                token = await _azureAdClientAssertion.GetTokenAsync(AuthenticationInfo.ApimTenantId, AuthenticationInfo.LoginId, AuthenticationInfo.ApimScope, cert, AuthenticationInfo.TokenName ?? httpClientName);
            }
            else if (AuthenticationInfo.AuthenticationStrategy == AuthenticationStrategy.ApimClientSecret && !string.IsNullOrEmpty(AuthenticationInfo.ApimClientSecret))
            {
                token = await _azureAdClientAssertion.GetTokenAsync(AuthenticationInfo.ApimTenantId, AuthenticationInfo.LoginId, AuthenticationInfo.ApimScope, AuthenticationInfo.ApimClientSecret, AuthenticationInfo.TokenName ?? httpClientName);
            }

            if (!string.IsNullOrEmpty(token))
            {
                var cacheExpiration = TimeSpan.FromMinutes(59);
                if (_redisService != null)
                {
                    await _redisService.Set(AuthenticationInfo.TokenName ?? httpClientName, token, cacheExpiration);
                    _logger.LogInformation(HttpContext, $"Azure APIM access token stored and expires in: {cacheExpiration}.");
                }
                else if (_cache != null)
                {
                    _cache.Set(AuthenticationInfo.TokenName ?? httpClientName, token, cacheExpiration);
                    _logger.LogInformation(HttpContext, $"Azure APIM access token stored and expires in: {cacheExpiration}.");
                }
                return token;
            }
            _logger.LogError(HttpContext, $"Error getting APIM token.");
            return null;
        }

        private async Task<string?> GetTokenBasic(string httpClientName)
        {
            if (AuthenticationInfo == null || string.IsNullOrEmpty(AuthenticationInfo.TokenUrl))
            {
                return null;
            }

            var httpClient = CreateHttpClient(AuthenticationInfo.AuthenticationHttpClientName ?? httpClientName);
            if (AuthenticationInfo.AuthenticationHeaders != null)
            {
                foreach (var header in AuthenticationInfo.AuthenticationHeaders)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
                }
            }
            FormUrlEncodedContent? content = null;
            if (AuthenticationInfo.BasicEncodedContent != null)
            {
                var encodedContent = new List<KeyValuePair<string, string>>();
                foreach (var header in AuthenticationInfo.BasicEncodedContent)
                {
                    encodedContent.Add(new(header.Name, header.Value));
                }
                content = new FormUrlEncodedContent(encodedContent);
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            }
            
            var tokenResponse = await httpClient.PostAsync(AuthenticationInfo.TokenUrl, content);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var token = await tokenResponse.Content.ReadAsAsync<BasicTokenResponse>();
                if (token != null && !string.IsNullOrEmpty(token.Access_Token))
                {
                    var cacheExpiration = TimeSpan.FromSeconds(59);
                    if (_redisService != null)
                    {
                        await _redisService.Set(AuthenticationInfo.TokenName ?? httpClientName, token.Access_Token, cacheExpiration);
                        _logger.LogInformation(HttpContext, $"Basic token stored and expires in: {cacheExpiration}.");
                    }
                    else if (_cache != null)
                    {
                        _cache.Set(AuthenticationInfo.TokenName ?? httpClientName, token.Access_Token, cacheExpiration);
                        _logger.LogInformation(HttpContext, $"Basic token stored and expires in: {cacheExpiration}.");
                    }
                    return token.Access_Token;
                }
            }
            _logger.LogError(HttpContext, $"Error getting Basic token.");
            return null;
        }

        private static string ToQueryString(object obj)
        {
            var s = new StringBuilder("?");
            var objType = obj.GetType();
            objType.GetProperties().Where(x => x.GetValue(obj, null) != null).ToList().ForEach(x => s.Append($"{Uri.EscapeDataString(x.Name)}={Uri.EscapeDataString(x.GetValue(obj)?.ToString() ?? "")}&"));
            s.Length--;
            return s.ToString();
        }

        private static string ToQueryString(List<(string Parameter, string Value)> queryParameters)
        {
            var s = new StringBuilder("?");
            foreach (var queryParameter in queryParameters)
            {
                s.Append($"{Uri.EscapeDataString(queryParameter.Parameter)}={Uri.EscapeDataString(queryParameter.Value)}&");
                
            }
            s.Length--;
            return s.ToString();
        }
    }
}
