using Microsoft.AspNetCore.Http;
using Roo.Azure.Configuration.Common.Http.Models;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Http
{
    /// <summary>
    /// HttpClient wrapper to standardize HTTP calls, add the headers, authorize tokens, and deserialize responses.<br/>
    /// Common input parameters:<br/>
    /// relativeUrl: Relative URL with query parameters already configured, unless queryParameters is used.<br/>
    /// httpClientName: Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.<br/>
    /// queryParameters: Dictionary of query parameter names and values that will be configured and appended to URL.<br/>
    /// </summary>
    public interface IRooHttpClient
    {
        /// <summary>
        /// Current HttpContext, required. Use DefaultHttpContext if used in a non-context application
        /// </summary>
        public HttpContext? HttpContext { get; set; }

        /// <summary>
        /// Used to overwrite existing channel-id header
        /// </summary>
        public string? ChannelId { get; set; }

        /// <summary>
        /// Used to overwrite existing session-id header
        /// </summary>
        public string? SessionId { get; set; }

        /// <summary>
        /// Used to overwrite existing transaction-id header
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// Use to overwrite/set user-info header
        /// </summary>
        public UserInfo? UserInfo { get; set; }

        /// <summary>
        /// <inheritdoc cref="Models.AuthenticationInfo"/>
        /// </summary>
        public AuthenticationInfo? AuthenticationInfo { get; set; }

        /// <summary>
        /// <inheritdoc cref="Models.SerializationSettings"/>
        /// </summary>
        public SerializationSettings? SerializationSettings { get; set; }

        /// <summary>
        /// GetAsync without object deserialization.
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// GetAsync without object deserialization and an object query parameter.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Relative URL with query parameters already configured.</param>
        /// <param name="queryStringObject">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string relativeUrl, string httpClientName, object queryStringObject, CancellationToken? cancellationToken = null);

        /// <summary>
        /// GetAsync with object deserialization.
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// GetAsync with object deserialization and an object query parameter.
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Relative URL with query parameters already configured.</param>
        /// <param name="queryStringObject">Object that will be converted to a query string.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string relativeUrl, string httpClientName, object queryStringObject, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="GetAsync(string, string, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="GetAsync(string, string, object, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Relative URL with query parameters already configured.</param>
        /// <param name="queryStringObject">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string relativeUrl, Enum httpClientName, object queryStringObject, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="GetAsync{TResult}(string, string, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="GetAsync{TResult}(string, string, object, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Relative URL with query parameters already configured.</param>
        /// <param name="queryStringObject">Object that will be converted to a query string.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string relativeUrl, Enum httpClientName, object queryStringObject, CancellationToken? cancellationToken = null);

        /// <summary>
        /// PostAsync without object deserialization.
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with POST. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="content">HttpContent to be sent with POST. Will be ignored if object is used but takes precedence over MultipartFormDataContent and MultipartContent if more than one is used.</param>
        /// <param name="formData">MultipartFormDataContent to be sent with POST. Will be ignored if object or HttpContent is used but takes precedence over MultipartContent if more than one is used.</param>
        /// <param name="multipartContent">MultipartContent to be sent with POST. Will be ignored if object, HttpContent, or MultipartFormDataContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PostAsync(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// PostAsync with object deserialization.
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with POST. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> PostAsync<TResult>(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="PostAsync(string, string, object?, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with POST. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="content">HttpContent to be sent with POST. Will be ignored if object is used but takes precedence over MultipartFormDataContent and MultipartContent if more than one is used.</param>
        /// <param name="formData">MultipartFormDataContent to be sent with POST. Will be ignored if object or HttpContent is used but takes precedence over MultipartContent if more than one is used.</param>
        /// <param name="multipartContent">MultipartContent to be sent with POST. Will be ignored if object, HttpContent, or MultipartFormDataContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PostAsync(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="PostAsync{TResult}(string, string, object?, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with POST. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> PostAsync<TResult>(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// PutAsync without object deserialization.
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with PUT. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PutAsync(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// PutAsync with object deserialization.
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with PUT. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> PutAsync<TResult>(string relativeUrl, string httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="PutAsync(string, string, object?, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <param name="relativeUrl"><Relative URL with query parameters already configured, unless a query Dictionary is used.></param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with PUT. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> PutAsync(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="PutAsync{TResult}(string, string, object?, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with PUT. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> PutAsync<TResult>(string relativeUrl, Enum httpClientName, object? data = null, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// DeleteAsync with object deserialization.
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with DELETE. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteAsync(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// DeleteAsync with object deserialization.
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with DELETE. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> DeleteAsync<TResult>(string relativeUrl, string httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="DeleteAsync(string, string, List{ValueTuple{string, string}}?, CancellationToken?)"/>>
        /// </summary>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with DELETE. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> DeleteAsync(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// <inheritdoc cref="DeleteAsync{TResult}(string, string, List{ValueTuple{string, string}}?, CancellationToken?)"/>
        /// </summary>
        /// <typeparam name="TResult">Object that response should be deserialized into.</typeparam>
        /// <param name="relativeUrl">Relative URL with query parameters already configured, unless a query Dictionary is used.</param>
        /// <param name="httpClientName">Name of client; either a client configured at startup or, if a non-configured client is used, then the base URL must included with with the relative URL.</param>
        /// <param name="data">Object to be sent with DELETE. Takes precedence over HttpContent, MultipartFormDataContent, and MultipartContent if more than one is used.</param>
        /// <param name="queryParameters">Dictionary of query parameter names and values that will be configured and appended to URL.</param>
        /// <param name="cancellationToken">Cancellation token to terminate the HTTP call.</param>
        /// <returns></returns>
        public Task<TResult> DeleteAsync<TResult>(string relativeUrl, Enum httpClientName, List<(string Parameter, string Value)>? queryParameters = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Clears existing token currently stored in cache. If RooHttpClient was initialized without a cache then this does nothing.
        /// </summary>
        /// <param name="tokeName">Name of token stored in cache.</param>
        /// <returns></returns>
        public Task<bool> ClearToken(string? tokeName = null);
    }
}
