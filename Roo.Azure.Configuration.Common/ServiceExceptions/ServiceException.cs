using System.Runtime.Serialization;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Custom exception model to relate the custom headers to the exception to create better exception logs.
    /// </summary>
    public interface IServiceException
    {
        /// <summary>
        /// Convert an Exception to a ServiceException.
        /// </summary>
        /// <param name="exception">Exception that occured.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        /// <returns>A ServiceException with additional details for logging.</returns>
        ServiceException ConvertToServiceException(Exception exception, string? transactionId = null);

        /// <summary>
        /// Create a new ServiceException.
        /// </summary>
        /// <param name="code">Custom error code associated with exception.</param>
        /// <param name="message">Brief description of the exception.</param>
        /// <param name="details">Key-value pairs associated with the exception.</param>
        /// <param name="innerException">Inner exception that triggered this.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        /// <returns>A ServiceException with additional details for logging.</returns>
        ServiceException CreateServiceException(ErrorCode code, string? message = null, Dictionary<string, string>? details = null, Exception? innerException = null, string? transactionId = null);
    }

    /// <summary>
    /// Implementation of <see cref="IServiceException"/>.
    /// </summary>
    public class ServiceException : Exception, IServiceException
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
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ServiceException(SerializationInfo info, StreamingContext context)
        {
            Error = new ServiceError(0);
        }

        public ServiceException ConvertToServiceException(Exception exception, string? transactionId = null)
        {
            throw new NotImplementedException();
        }

        public ServiceException CreateServiceException(ErrorCode code, string? message = null, Dictionary<string, string>? details = null, Exception? innerException = null, string? transactionId = null)
        {
            throw new NotImplementedException();
        }
    }
}
