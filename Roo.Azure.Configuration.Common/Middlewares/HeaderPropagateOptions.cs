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
        /// Initializes a new instance of the <see cref="HeaderPropagateOptions"/> class.
        /// </summary>
        /// <param name="headers">A list of headers to propagate.</param>
        public HeaderPropagateOptions(ICollection<string> headers)
        {
            this.headers = headers.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderPropagateOptions"/> class.
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
