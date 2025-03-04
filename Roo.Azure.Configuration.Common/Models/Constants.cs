namespace Roo.Azure.Configuration.Common.Models
{
    /// <summary>
    /// Constant values for headers.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// SessionId header name.
        /// </summary>
        public const string SessionIdHeaderName = "session-id";

        /// <summary>
        /// TransactionId header name.
        /// </summary>
        public const string TransactionIdHeaderName = "transaction-id";

        /// <summary>
        /// ChannelId header name.
        /// </summary>
        public const string ChannelIdHeaderName = "channel-id";

        /// <summary>
        /// User info header name.
        /// </summary>
        public const string UserInfoHeaderName = "user-info";

        /// <summary>
        /// SessionId environment variable name.
        /// </summary>
        public const string SessionId = "SessionId";

        /// <summary>
        /// ChannelId environment variable name.
        /// </summary>
        public const string ChannelId = "ChannelId";

        /// <summary>
        /// UserInfo Username environment variable name.
        /// </summary>
        public const string UserInfoUsername = "Username";

        /// <summary>
        /// Name of HTTP client used by Telemetry.
        /// </summary>
        public const string HTTPClientTelemetry = "telemetry-dependency";
    }
}
