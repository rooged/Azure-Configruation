using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Converts between <see cref="Exception"/> and <see cref="ServiceException"/>
    /// </summary>
    public static class ServiceExceptionConverter
    {
        private static readonly List<int?> SqlExceptionNumberListToInvalidInput = new List<int?> { 547 };

        private static Dictionary<string, string> GetDictionary(Exception ex, Dictionary<string, string>? additional = null)
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Type", ex.GetType().FullName ?? "" },
                { "BaseMessage", ex.GetBaseException().Message ?? "" },
                { "Source", ex.Source ?? "" },
                { "Method", ex.TargetSite?.Name ?? "" },
                { "StackTrace", ex.StackTrace ?? "" },
                { "HelpLink", ex.HelpLink ?? "" }
            };

            if (additional != null)
            {
                foreach (var item in additional)
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Convert an <see cref="Exception"/> to a <see cref="ServiceException"/>.
        /// </summary>
        /// <param name="exception">Exception to convert.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        /// <returns></returns>
        public static ServiceException ConvertTo(Exception exception, string? transactionId = null)
        {
            return exception switch
            {
                ServiceException ex => ex,
                AccessViolationException ex => new ServiceException(ErrorCode.AccessViolation, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                AggregateException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                AppDomainUnloadedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ArgumentNullException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex.InnerException, transactionId),
                ArgumentOutOfRangeException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex.InnerException, transactionId),
                DuplicateWaitObjectException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex.InnerException, transactionId),
                ArgumentException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex.InnerException, transactionId),
                DivideByZeroException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                NotFiniteNumberException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "OffendingNumber", ex.OffendingNumber.ToString() }
                }), ex.InnerException, transactionId),
                OverflowException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ArithmeticException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ArrayTypeMismatchException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                BadImageFormatException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex.InnerException, transactionId),
                CannotUnloadAppDomainException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ContextMarshalException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                DataMisalignedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                //DbUpdate ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                //AggregateException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                DirectoryNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                DllNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName ?? "" }
                }), ex.InnerException, transactionId),
                EndOfStreamException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                EntryPointNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName ?? "" }
                }), ex.InnerException, transactionId),
                FieldAccessException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                FileNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "FileName", ex.FileName ?? "" },
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex.InnerException, transactionId),
                UriFormatException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                FormatException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                HttpIOException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "HttpRequestError", ex.HttpRequestError.ToString() }
                }), ex.InnerException, transactionId),
                IndexOutOfRangeException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                InsufficientExecutionStackException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                InsufficientMemoryException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                InvalidCastException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                InvalidDataException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ObjectDisposedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ObjectName", ex.ObjectName ?? "" }
                }), ex.InnerException, transactionId),
                InvalidOperationException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                InvalidTimeZoneException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                KeyNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                LockRecursionException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                MissingMethodException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                MethodAccessException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                MissingFieldException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                MissingMemberException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                MemberAccessException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                NotImplementedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                PlatformNotSupportedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                NotSupportedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                NullReferenceException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                OperationCanceledException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                OutOfMemoryException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                PathTooLongException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                RankException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                StackOverflowException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                TimeoutException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                TimeZoneNotFoundException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                TypeAccessException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName ?? "" }
                }), ex.InnerException, transactionId),
                TypeInitializationException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName ?? "" }
                }), ex.InnerException, transactionId),
                TypeLoadException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName ?? "" }
                }), ex.InnerException, transactionId),
                TypeUnloadedException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                UnauthorizedAccessException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex), ex.InnerException, transactionId),
                ValidationException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "ValidationValue", ex.Value?.ToString() ?? "" },
                    { "ValidationAttributeErrorMessage", ex.ValidationAttribute?.ErrorMessage ?? "" },
                    { "ValidationAttributeErrorMessage", ex.ValidationAttribute?.ErrorMessageResourceName ?? "" },
                    { "ValidationResultMemberNames", string.Join(", ", ex.ValidationResult.MemberNames) },
                }), ex.InnerException, transactionId),
                var ex => new ServiceException(ErrorCode.None, ex.Message, GetDictionary(ex), ex.InnerException, transactionId)
            };
        }
    }
}
