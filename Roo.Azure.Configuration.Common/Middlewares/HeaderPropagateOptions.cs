using Roo.Azure.Configuration.Common.Models;

namespace Roo.Azure.Configuration.Common.Middlewares
{
    /// <summary>
    /// Options for the HeaderPropagateMiddleware.
    /// </summary>
    public class HeaderPropagateOptions
    {
        private readonly List<string> headers;

        /// <summary>
        /// Initializes a list of header names that will be configured and added with the <see cref="HeaderPropagateMiddleware>"/>.<br/>
        /// Default headers: session-id, transaction-id, channel-id, user-info
        /// </summary>
        /// <param name="headers">A list of headers to propagate.</param>
        public HeaderPropagateOptions(ICollection<string> headers)
        {
            this.headers = new List<string>()
            {
                Constants.SessionIdHeaderName,
                Constants.TransactionIdHeaderName,
                Constants.ChannelIdHeaderName,
                Constants.UserInfoHeaderName
            };
            this.headers.AddRange(headers);
        }

        /// <summary>
        /// Initializes the default list of header names that will be configured and added with the <see cref="HeaderPropagateMiddleware>"/>.<br/>
        /// Default headers: session-id, transaction-id, channel-id, user-info
        /// </summary>
        public HeaderPropagateOptions()
        {
            headers = new List<string>()
            {
                Constants.SessionIdHeaderName,
                Constants.TransactionIdHeaderName,
                Constants.ChannelIdHeaderName,
                Constants.UserInfoHeaderName
            };
        }

        /// <summary>
        /// Gets the headers taht should be propagated.
        /// </summary>
        public ICollection<string> Headers
        {
            get => headers;
        }
    }
}
