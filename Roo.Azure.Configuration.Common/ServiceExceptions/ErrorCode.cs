namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Custom error codes to simplify error handling related to errors specific to this package.<br/>
    /// 43x: Request Header Error
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// session-id not found in request headers.
        /// </summary>
        SessionIdHeaderNotFound = 432,
        /// <summary>
        /// transaction-id not found in request headers.
        /// </summary>
        TransactionIdHeaderNotFound = 433,
        /// <summary>
        /// channel-id not found in request headers.
        /// </summary>
        ChannelIdHeaderNotFound = 434,
        /// <summary>
        /// user-info not found in request headers.
        /// </summary>
        UserInfoHeaderNotFound = 435
    }
}
