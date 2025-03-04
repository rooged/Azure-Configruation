namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Custom exception model to relate the custom headers to the exception to create better exception logs.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// The <see cref="ServiceError"/> for this exception.
        /// </summary>
        public ServiceError Error { get; set; }

        /// <summary>
        /// Inner exception that caused the error.
        /// </summary>
        public new Exception? InnerException { get; set; }

        /// <summary>
        /// Initialize a default <see cref="ServiceError"/> with no specific error, can be modified after creation.
        /// </summary>
        public ServiceException()
        {
            Error = new ServiceError(0);
        }

        /// <summary>
        /// Initialize an exception with full details.
        /// </summary>
        /// <param name="code">Custom error code associated with exception.</param>
        /// <param name="message">Brief description of the exception.</param>
        /// <param name="details">Key-value pairs associated with the exception.</param>
        /// <param name="innerException">Inner exception that triggered this.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        public ServiceException(ErrorCode code, string? message = null, Dictionary<string, string>? details = null, Exception? innerException = null, string? transactionId = null)
        {
            Error = new ServiceError(code, message, details, transactionId);
            InnerException = innerException;
        }

        /// <summary>
        /// Initialize an exception with a ServiceError.
        /// </summary>
        /// <param name="error">ServiceError associated with exception.</param>
        /// <param name="innerException">Inner exception that triggered this.</param>
        public ServiceException(ServiceError error, Exception? innerException = null)
        {
            Error = error;
            InnerException = innerException;
        }
    }
}
