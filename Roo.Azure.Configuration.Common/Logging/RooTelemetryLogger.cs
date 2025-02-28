using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Metrics;
using Prometheus;
using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Logging
{
    /// <summary>
    /// Custom event wrapper to add custom headers to telemetry and custom event logs in App Insights.
    /// <list type="table">session-id: Stays constant throughout a session.<br/>
    /// transaction-id: Unique to each HTTP request.<br/>
    /// channel-id: Where a request is coming from.</list>
    /// HttpContext is required since the headers are default in context. If used for logging in non-HTTP applications, such as batch jobs, use DefaultHttpContext in startup to set headers.
    /// </summary>
    public interface IRooTelemetryLogger
    {
        /// <summary>
        /// Additional information that can be sent with telemetry logs.
        /// </summary>
        public TelemetryData? TelemetryData { get; set; }

        /// <summary>
        /// Track event initiated by a user.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="properties">Additional properties about the event.</param>
        /// <param name="metrics">Additional metrics about the event.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackEvent(string name, Dictionary<string, string>? properties, Dictionary<string, double>? metrics, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track and aggregate values before sending a singular metric to App Insights.
        /// </summary>
        /// <param name="id">Name of the metric.</param>
        /// <param name="nameSpace">Namespace for metric values, required when using dimensions.</param>
        /// <param name="dimensions">Names of values to be tracked, maximum of 10 dimensions can be tracked. Namespace required when using this.</param>
        /// <param name="config">Limit series count, values per dimension, and if negative ints should be tracked.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        /// <returns></returns>
        Metric GetMetric(string id, string? nameSpace, List<string>? dimensions, MetricConfiguration? config, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track a page view, tab specific url, duration spent on page, and id of page.
        /// </summary>
        /// <param name="name">Name of page.</param>
        /// <param name="url">Url of page, will be converted to a URI and should be valid.</param>
        /// <param name="duration">Amount of time spent on the page.</param>
        /// <param name="id">Id of page.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackPageView(string name, string? url, TimeSpan? duration, string? id, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track a HTTP request, should be used when the request acts as an operation context. Use with StartOperation/RequestTelemetry/({operationName})
        /// </summary>
        /// <param name="name">Name of request.</param>
        /// <param name="startTime">Start time of request.</param>
        /// <param name="duration">Duration of request.</param>
        /// <param name="responseCode">Response code from request.</param>
        /// <param name="success">Whether the request was successful.</param>
        /// <param name="url">Url of page, will be converted to a URI and should be valid.</param>
        /// <param name="pageId">Id of page.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool? success, string? url, string? pageId, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Used to correlate telemetry items together by sharing the same operation ID with all telemetry logs sent within the operation.<br/>
        /// Must dispose of this method either with 'using' or with StopOperation, othewise telemetry won't be sent.<br/>
        /// Note: This creates the "Related Items" list in App Insights.
        /// </summary>
        /// <param name="name">Name of operation.</param>
        /// <param name="data">Telemetry data.</param>
        /// <returns></returns>
        IOperationHolder<RequestTelemetry> StartOperation(string name, TelemetryData? data);
        /// <summary>
        /// Ends an operation and sends all telemetry from it to App Insights.<br/>
        /// Not required to end the operation if StartOperation is initialized in a using.
        /// </summary>
        /// <param name="operation">The operation.</param>
        void StopOperation(IOperationHolder<RequestTelemetry> operation);
        /// <summary>
        /// Track an exception. Most exceptions are already tracked by App Insights, use to add additional information to the telemetry.
        /// </summary>
        /// <param name="exception">The exception to track.</param>
        /// <param name="properties">Additional properties about the event.</param>
        /// <param name="metrics">Additional metrics about the event.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackException(Exception exception, Dictionary<string, string>? properties, Dictionary<string, double>? metrics, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Standard trace logging. Message can handle large inputs such as encoded POST data to track HTTP requests and responses.
        /// </summary>
        /// <param name="message">Trace message.</param>
        /// <param name="severityLevel">Severity level of trace.</param>
        /// <param name="properties">Additional properties about the event.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackTrace(string message, SeverityLevel? severityLevel, Dictionary<string, string>? properties, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track response times and success rates of calls. Must use with a timer and manually set dependency data.<br/>
        /// In most cases, <see cref="StartOperation(string, TelemetryData?)"/> should be used instead because it does the same without needing a timer and manual data entry.
        /// </summary>
        /// <param name="name">Name of request to track.</param>
        /// <param name="dependencyData">Data to be included with telemetry.</param>
        /// <param name="startTime">Start time of request.</param>
        /// <param name="duration">Duration of request.</param>
        /// <param name="success">Whether the request was a success.</param>
        /// <param name="dependencyType">Type of dependency, for example SQL, Azure Table, or HTTP.</param>
        /// <param name="target">Target of request.</param>
        /// <param name="resultCode">Result code of request.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackDependency(string name, string dependencyData, DateTimeOffset startTime, TimeSpan duration, bool success, string? dependencyType, string? target, string? resultCode, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Clear all stored telemetry data that is currently cached.
        /// </summary>
        /// <param name="sleepTime">Time to wait after flush to allow completion, time is in milliseconds.</param>
        void TelemetryFlush(int sleepTime);
        /// <summary>
        /// Clear all stored telemetry data that is currently cached asynchronously.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Whether telemetry was flushed.</returns>
        Task<bool> TelemetryFlushAsync(CancellationToken? token);
    }

    /// <summary>
    /// Implementation of <see cref="IRooTelemetryLogger"/>.
    /// </summary>
    public class RooTelemetryLogger : IRooTelemetryLogger
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TelemetryData? TelemetryData { get; set; }

        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Initialize <see cref="RooTelemetryLogger"/>.
        /// </summary>
        /// <param name="telemetryClient"></param>
        public RooTelemetryLogger(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Set telemetry data in client.
        /// </summary>
        /// <param name="data"></param>
        public void SetContext(TelemetryData data)
        {
            if (data.User != null)
            {
                _telemetryClient.Context.User.Id = data.User.Id;
                _telemetryClient.Context.User.AccountId = data.User.AccountId;
                _telemetryClient.Context.User.UserAgent = data.User.UserAgent;
                _telemetryClient.Context.User.AuthenticatedUserId = data.User.AuthenticatedUserId;
            }
            if (data.Session != null)
            {
                _telemetryClient.Context.Session.Id = data.Session.Id;
                _telemetryClient.Context.Session.IsFirst = data.Session.IsFirst;
            }
            if (data.Device != null)
            {
                _telemetryClient.Context.Device.Id = data.Device.Id;
                _telemetryClient.Context.Device.Type = data.Device.Type;
                _telemetryClient.Context.Device.OperatingSystem = data.Device.OperatingSystem;
                _telemetryClient.Context.Device.OemName = data.Device.OemName;
                _telemetryClient.Context.Device.Model = data.Device.Model;
            }
            if (data.Component != null)
            {
                _telemetryClient.Context.Component.Version = data.Component.Version;
            }
            if (data.Cloud != null)
            {
                _telemetryClient.Context.Cloud.RoleName = data.Cloud.RoleName;
                _telemetryClient.Context.Cloud.RoleInstance = data.Cloud.RoleInstance;
            }
            if (data.Operation != null)
            {
                _telemetryClient.Context.Operation.Id = data.Operation.Id;
                _telemetryClient.Context.Operation.ParentId = data.Operation.ParentId;
                _telemetryClient.Context.Operation.SyntheticSource = data.Operation.SyntheticSource;
                _telemetryClient.Context.Operation.Name = data.Operation.Name;
            }
            if (data.Location != null)
            {
                _telemetryClient.Context.Location.Ip = data.Location.Ip;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <param name="metrics"></param>
        /// <param name="data"></param>
        /// <param name="clearTelemetryData"></param>
        public void TrackEvent(string name, Dictionary<string, string>? properties, Dictionary<string, double>? metrics, TelemetryData? data, bool clearTelemetryData = false)
        {
            if (data != null)
            {
                SetContext(data);
            }
            _telemetryClient.TrackEvent(name, properties, metrics);
            if (clearTelemetryData)
            {
                TelemetryData = null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameSpace"></param>
        /// <param name="dimensions"></param>
        /// <param name="config"></param>
        /// <param name="data"></param>
        /// <param name="clearTelemetryData"></param>
        /// <returns></returns>
        public Metric GetMetric(string id, string? nameSpace, List<string>? dimensions, MetricConfiguration? config, TelemetryData? data, bool clearTelemetryData = false)
        {
            if (data != null)
            {
                SetContext(data);
            }
            var metricId = new MetricIdentifier(id);
            if (!string.IsNullOrEmpty(nameSpace))
            {
                metricId = new MetricIdentifier(nameSpace, id, dimensions?.ElementAtOrDefault(0), dimensions?.ElementAtOrDefault(1), dimensions?.ElementAtOrDefault(2), dimensions?.ElementAtOrDefault(3),
                    dimensions?.ElementAtOrDefault(4), dimensions?.ElementAtOrDefault(5), dimensions?.ElementAtOrDefault(6), dimensions?.ElementAtOrDefault(7), dimensions?.ElementAtOrDefault(8), dimensions?.ElementAtOrDefault(9));
            }
            _telemetryClient.TrackEvent(name, properties, metrics);
            if (clearTelemetryData)
            {
                TelemetryData = null;
            }
        }

        /// <summary>
        /// Track a page view, tab specific url, duration spent on page, and id of page.
        /// </summary>
        /// <param name="name">Name of page.</param>
        /// <param name="url">Url of page, will be converted to a URI and should be valid.</param>
        /// <param name="duration">Amount of time spent on the page.</param>
        /// <param name="id">Id of page.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackPageView(string name, string? url, TimeSpan? duration, string? id, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track a HTTP request, should be used when the request acts as an operation context. Use with StartOperation/RequestTelemetry/({operationName})
        /// </summary>
        /// <param name="name">Name of request.</param>
        /// <param name="startTime">Start time of request.</param>
        /// <param name="duration">Duration of request.</param>
        /// <param name="responseCode">Response code from request.</param>
        /// <param name="success">Whether the request was successful.</param>
        /// <param name="url">Url of page, will be converted to a URI and should be valid.</param>
        /// <param name="pageId">Id of page.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool? success, string? url, string? pageId, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Used to correlate telemetry items together by sharing the same operation ID with all telemetry logs sent within the operation.<br/>
        /// Must dispose of this method either with 'using' or with StopOperation, othewise telemetry won't be sent.<br/>
        /// Note: This creates the "Related Items" list in App Insights.
        /// </summary>
        /// <param name="name">Name of operation.</param>
        /// <param name="data">Telemetry data.</param>
        /// <returns></returns>
        IOperationHolder<RequestTelemetry> StartOperation(string name, TelemetryData? data);
        /// <summary>
        /// Ends an operation and sends all telemetry from it to App Insights.<br/>
        /// Not required to end the operation if StartOperation is initialized in a using.
        /// </summary>
        /// <param name="operation">The operation.</param>
        void StopOperation(IOperationHolder<RequestTelemetry> operation);
        /// <summary>
        /// Track an exception. Most exceptions are already tracked by App Insights, use to add additional information to the telemetry.
        /// </summary>
        /// <param name="exception">The exception to track.</param>
        /// <param name="properties">Additional properties about the event.</param>
        /// <param name="metrics">Additional metrics about the event.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackException(Exception exception, Dictionary<string, string>? properties, Dictionary<string, double>? metrics, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Standard trace logging. Message can handle large inputs such as encoded POST data to track HTTP requests and responses.
        /// </summary>
        /// <param name="message">Trace message.</param>
        /// <param name="severityLevel">Severity level of trace.</param>
        /// <param name="properties">Additional properties about the event.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackTrace(string message, SeverityLevel? severityLevel, Dictionary<string, string>? properties, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Track response times and success rates of calls. Must use with a timer and manually set dependency data.<br/>
        /// In most cases, <see cref="StartOperation(string, TelemetryData?)"/> should be used instead because it does the same without needing a timer and manual data entry.
        /// </summary>
        /// <param name="name">Name of request to track.</param>
        /// <param name="dependencyData">Data to be included with telemetry.</param>
        /// <param name="startTime">Start time of request.</param>
        /// <param name="duration">Duration of request.</param>
        /// <param name="success">Whether the request was a success.</param>
        /// <param name="dependencyType">Type of dependency, for example SQL, Azure Table, or HTTP.</param>
        /// <param name="target">Target of request.</param>
        /// <param name="resultCode">Result code of request.</param>
        /// <param name="data">Telemetry data.</param>
        /// <param name="clearTelemetryData">Clear existing telemetry data.</param>
        void TrackDependency(string name, string dependencyData, DateTimeOffset startTime, TimeSpan duration, bool success, string? dependencyType, string? target, string? resultCode, TelemetryData? data, bool clearTelemetryData = false);
        /// <summary>
        /// Clear all stored telemetry data that is currently cached.
        /// </summary>
        /// <param name="sleepTime">Time to wait after flush to allow completion, time is in milliseconds.</param>
        void TelemetryFlush(int sleepTime);
        /// <summary>
        /// Clear all stored telemetry data that is currently cached asynchronously.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Whether telemetry was flushed.</returns>
        Task<bool> TelemetryFlushAsync(CancellationToken? token);
    }
}
