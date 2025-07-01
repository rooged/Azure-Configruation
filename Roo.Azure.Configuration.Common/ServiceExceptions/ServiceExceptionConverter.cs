using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Converts between <see cref="Exception"/> and <see cref="ServiceException"/>
    /// </summary>
    public static class ServiceExceptionConverter
    {
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
        /// <inheritdoc cref="ConvertTo(Exception, string?)"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static ServiceException ConvertToServiceException(this Exception exception, string? transactionId)
        {
            return ConvertTo(exception, transactionId);
        }

        /// <summary>
        /// <inheritdoc cref="ConvertFrom(ServiceException)"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Exception ConvertFromServiceException(this ServiceException exception)
        {
            return ConvertFrom(exception);
        }

        /// <summary>
        /// Convert an <see cref="Exception"/> to a <see cref="ServiceException"/>.
        /// </summary>
        /// <param name="exception">Exception to convert.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        /// <returns>A ServiceException configured for the input exception.</returns>
        public static ServiceException ConvertTo(Exception exception, string? transactionId = null)
        {
            return exception switch
            {
                ServiceException ex => ex,
                AccessViolationException ex => new ServiceException(ErrorCode.AccessViolation, ex.Message, GetDictionary(ex), ex, transactionId),
                AggregateException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "Exceptions", string.Join(", ", ex.InnerExceptions.Select(x => x.GetType().Name)) }
                }), ex, transactionId),
                AppDomainUnloadedException ex => new ServiceException(ErrorCode.AppDomainUnloaded, ex.Message, GetDictionary(ex), ex, transactionId),
                ArgumentNullException ex => new ServiceException(ErrorCode.ArgumentNull, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                ArgumentOutOfRangeException ex => new ServiceException(ErrorCode.ArgumentOutOfRange, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" },
                    { "Value", ex.ActualValue?.ToString() ?? "" }
                }), ex, transactionId),
                DuplicateWaitObjectException ex => new ServiceException(ErrorCode.DuplicateWaitObject, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                ArgumentException ex => new ServiceException(ErrorCode.ArgumentInvalid, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                DivideByZeroException ex => new ServiceException(ErrorCode.DivideByZero, ex.Message, GetDictionary(ex), ex, transactionId),
                NotFiniteNumberException ex => new ServiceException(ErrorCode.NotFiniteNumber, ex.Message, GetDictionary(ex, new()
                {
                    { "OffendingNumber", ex.OffendingNumber.ToString() }
                }), ex, transactionId),
                OverflowException ex => new ServiceException(ErrorCode.OverflowFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                ArithmeticException ex => new ServiceException(ErrorCode.ArithmeticInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                ArrayTypeMismatchException ex => new ServiceException(ErrorCode.ArrayTypeMismatch, ex.Message, GetDictionary(ex), ex, transactionId),
                BadImageFormatException ex => new ServiceException(ErrorCode.BadImageFormat, ex.Message, GetDictionary(ex, new()
                {
                    { "FileName", ex.FileName ?? "" },
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex, transactionId),
                CannotUnloadAppDomainException ex => new ServiceException(ErrorCode.CannotUnloadAppDomain, ex.Message, GetDictionary(ex), ex, transactionId),
                ContextMarshalException ex => new ServiceException(ErrorCode.ContextMashalFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                DataMisalignedException ex => new ServiceException(ErrorCode.DataMisaligned, ex.Message, GetDictionary(ex), ex, transactionId),
                DbUpdateConcurrencyException ex => new ServiceException(ErrorCode.DbUpdateConcurrency, ex.Message, GetDictionary(ex), ex, transactionId),
                DbUpdateException ex => new ServiceException(ErrorCode.DbUpdateFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                DirectoryNotFoundException ex => new ServiceException(ErrorCode.DirectoryNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                DllNotFoundException ex => new ServiceException(ErrorCode.DllNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                EndOfStreamException ex => new ServiceException(ErrorCode.EndOfStream, ex.Message, GetDictionary(ex), ex, transactionId),
                EntryPointNotFoundException ex => new ServiceException(ErrorCode.EntryPointNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                FieldAccessException ex => new ServiceException(ErrorCode.FieldAccessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                FileNotFoundException ex => new ServiceException(ErrorCode.FileNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "FileName", ex.FileName ?? "" },
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex, transactionId),
                UriFormatException ex => new ServiceException(ErrorCode.UriFormatException, ex.Message, GetDictionary(ex), ex, transactionId),
                FormatException ex => new ServiceException(ErrorCode.FormatInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                HttpIOException ex => new ServiceException(ErrorCode.HttpIOFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "HttpRequestError", ex.HttpRequestError.ToString() }
                }), ex, transactionId),
                IndexOutOfRangeException ex => new ServiceException(ErrorCode.IndexOutOfRange, ex.Message, GetDictionary(ex), ex, transactionId),
                InsufficientExecutionStackException ex => new ServiceException(ErrorCode.InsufficientExecutionStack, ex.Message, GetDictionary(ex), ex, transactionId),
                InsufficientMemoryException ex => new ServiceException(ErrorCode.InsufficientMemory, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidCastException ex => new ServiceException(ErrorCode.InvalidCast, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidDataException ex => new ServiceException(ErrorCode.InvalidData, ex.Message, GetDictionary(ex), ex, transactionId),
                ObjectDisposedException ex => new ServiceException(ErrorCode.ObjectDisposed, ex.Message, GetDictionary(ex, new()
                {
                    { "ObjectName", ex.ObjectName }
                }), ex, transactionId),
                InvalidOperationException ex => new ServiceException(ErrorCode.InvalidOperation, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidTimeZoneException ex => new ServiceException(ErrorCode.InvalidTimeZone, ex.Message, GetDictionary(ex), ex, transactionId),
                KeyNotFoundException ex => new ServiceException(ErrorCode.KeyNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                LockRecursionException ex => new ServiceException(ErrorCode.LockRecursionFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingMethodException ex => new ServiceException(ErrorCode.MissingMethod, ex.Message, GetDictionary(ex), ex, transactionId),
                MethodAccessException ex => new ServiceException(ErrorCode.MethodAccessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingFieldException ex => new ServiceException(ErrorCode.MissingField, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingMemberException ex => new ServiceException(ErrorCode.MissingMember, ex.Message, GetDictionary(ex), ex, transactionId),
                MemberAccessException ex => new ServiceException(ErrorCode.MemberAcessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                NotImplementedException ex => new ServiceException(ErrorCode.NotImplemented, ex.Message, GetDictionary(ex), ex, transactionId),
                PlatformNotSupportedException ex => new ServiceException(ErrorCode.PlatformNotSupported, ex.Message, GetDictionary(ex), ex, transactionId),
                NotSupportedException ex => new ServiceException(ErrorCode.NotSupported, ex.Message, GetDictionary(ex), ex, transactionId),
                NullReferenceException ex => new ServiceException(ErrorCode.NullReference, ex.Message, GetDictionary(ex), ex, transactionId),
                OperationCanceledException ex => new ServiceException(ErrorCode.OperationCanceled, ex.Message, GetDictionary(ex), ex, transactionId),
                OutOfMemoryException ex => new ServiceException(ErrorCode.OutOfMemory, ex.Message, GetDictionary(ex), ex, transactionId),
                PathTooLongException ex => new ServiceException(ErrorCode.PathTooLongFileName, ex.Message, GetDictionary(ex), ex, transactionId),
                RankException ex => new ServiceException(ErrorCode.RankArray, ex.Message, GetDictionary(ex), ex, transactionId),
                StackOverflowException ex => new ServiceException(ErrorCode.StackOverflow, ex.Message, GetDictionary(ex), ex, transactionId),
                TimeoutException ex => new ServiceException(ErrorCode.Timeout, ex.Message, GetDictionary(ex), ex, transactionId),
                TimeZoneNotFoundException ex => new ServiceException(ErrorCode.TimeZoneNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                TypeAccessException ex => new ServiceException(ErrorCode.TypeAccessInvalid, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeInitializationException ex => new ServiceException(ErrorCode.TypeInitializationFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeLoadException ex => new ServiceException(ErrorCode.TypeLoadFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeUnloadedException ex => new ServiceException(ErrorCode.TypeUnloaded, ex.Message, GetDictionary(ex), ex, transactionId),
                UnauthorizedAccessException ex => new ServiceException(ErrorCode.UnauthorizedAccess, ex.Message, GetDictionary(ex), ex, transactionId),
                ValidationException ex => new ServiceException(ErrorCode.ValidationFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "Value", ex.Value?.ToString() ?? "" },
                    { "AttributeErrorMessage", ex.ValidationAttribute?.ErrorMessage ?? "" },
                    { "AttributeErrorMessage", ex.ValidationAttribute?.ErrorMessageResourceName ?? "" },
                    { "ResultMemberNames", string.Join(", ", ex.ValidationResult.MemberNames) },
                }), ex, transactionId),
                var ex => new ServiceException(ErrorCode.None, ex.Message, GetDictionary(ex), ex, transactionId)
            };
        }

        /// <summary>
        /// Convert a <see cref="ServiceException"/> to an <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">ServiceException to convert.</param>
        /// <returns>An exception created from the input ServiceException.</returns>
        public static Exception ConvertFrom(ServiceException ex)
        {
            return ex.Error.Code switch
            {
                ErrorCode.AccessViolation => new AccessViolationException(ex.Error.Message, ex.InnerException),
                ErrorCode.AggregateFailure => new AggregateException(ex.Error.Message, ex.InnerException ?? new Exception()),
                ErrorCode.AppDomainUnloaded => new AppDomainUnloadedException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArgumentNull => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new ArgumentNullException(paramName, ex.Error.Message) : new ArgumentNullException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArgumentOutOfRange => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? ex.Error.Details.TryGetValue("Value", out string? value) ? new ArgumentOutOfRangeException(paramName, value, ex.Error.Message) : new ArgumentOutOfRangeException(paramName, ex.Error.Message) : new ArgumentOutOfRangeException(ex.Message, ex.InnerException),
                ErrorCode.DuplicateWaitObject => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new DuplicateWaitObjectException(paramName, ex.Error.Message) : new DuplicateWaitObjectException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArgumentInvalid => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new ArgumentException(ex.Error.Message, paramName, ex.InnerException) : new ArgumentException(ex.Error.Message, ex.InnerException),
                ErrorCode.DivideByZero => new DivideByZeroException(ex.Error.Message, ex.InnerException),
                ErrorCode.NotFiniteNumber => ex.Error.Details != null && ex.Error.Details.TryGetValue("OffendingNumber", out string? offendingNumber) ? new NotFiniteNumberException(ex.Error.Message, Convert.ToDouble(offendingNumber), ex.InnerException) : new NotFiniteNumberException(ex.Error.Message, ex.InnerException),
                ErrorCode.OverflowFailure => new OverflowException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArithmeticInvalid => new ArithmeticException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArrayTypeMismatch => new ArrayTypeMismatchException(ex.Error.Message, ex.InnerException),
                ErrorCode.BadImageFormat => ex.Error.Details != null && ex.Error.Details.TryGetValue("FileName", out string? fileName) ? new BadImageFormatException(ex.Error.Message, fileName, ex.InnerException) : new BadImageFormatException(ex.Error.Message, ex.InnerException),
                ErrorCode.CannotUnloadAppDomain => new CannotUnloadAppDomainException(ex.Error.Message, ex.InnerException),
                ErrorCode.ContextMashalFailure => new ContextMarshalException(ex.Error.Message, ex.InnerException),
                ErrorCode.DataMisaligned => new DataMisalignedException(ex.Error.Message, ex.InnerException),
                ErrorCode.DbUpdateConcurrency => new DbUpdateConcurrencyException(ex.Error.Message ?? ex.Message, ex.InnerException),
                ErrorCode.DbUpdateFailure => new DbUpdateException(ex.Error.Message ?? ex.Message, ex.InnerException),
                ErrorCode.DirectoryNotFound => new DirectoryNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.DllNotFound => new DllNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.EndOfStream => new EndOfStreamException(ex.Error.Message, ex.InnerException),
                ErrorCode.EntryPointNotFound => new EntryPointNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.FieldAccessInvalid => new FieldAccessException(ex.Error.Message, ex.InnerException),
                ErrorCode.FileNotFound => ex.Error.Details != null && ex.Error.Details.TryGetValue("FileName", out string? fileName) ? new FileNotFoundException(ex.Error.Message, fileName, ex.InnerException) : new FileNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.UriFormatException => new UriFormatException(ex.Error.Message, ex.InnerException),
                ErrorCode.FormatInvalid => new FormatException(ex.Error.Message, ex.InnerException),
                ErrorCode.HttpIOFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("HttpRequestError", out string? httpRequestError) ? new HttpIOException((HttpRequestError)Convert.ToInt32(httpRequestError), ex.Error.Message, ex.InnerException) : new IOException(ex.Error.Message, ex.InnerException),
                ErrorCode.IndexOutOfRange => new IndexOutOfRangeException(ex.Error.Message, ex.InnerException),
                ErrorCode.InsufficientExecutionStack => new InsufficientExecutionStackException(ex.Error.Message, ex.InnerException),
                ErrorCode.InsufficientMemory => new InsufficientMemoryException(ex.Error.Message, ex.InnerException),
                ErrorCode.InvalidCast => new InvalidCastException(ex.Error.Message, ex.InnerException),
                ErrorCode.InvalidData => new InvalidDataException(ex.Error.Message, ex.InnerException),
                ErrorCode.ObjectDisposed => ex.Error.Details != null && ex.Error.Details.TryGetValue("ObjectName", out string? objectName) ? new ObjectDisposedException(objectName, ex.Error.Message) : new ObjectDisposedException(ex.Error.Message, ex.InnerException),
                ErrorCode.InvalidOperation => new InvalidOperationException(ex.Error.Message, ex.InnerException),
                ErrorCode.InvalidTimeZone => new InvalidTimeZoneException(ex.Error.Message, ex.InnerException),
                ErrorCode.KeyNotFound => new KeyNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.LockRecursionFailure => new LockRecursionException(ex.Error.Message, ex.InnerException),
                ErrorCode.MissingMethod => new MissingMethodException(ex.Error.Message, ex.InnerException),
                ErrorCode.MethodAccessInvalid => new MethodAccessException(ex.Error.Message, ex.InnerException),
                ErrorCode.MissingField => new MissingFieldException(ex.Error.Message, ex.InnerException),
                ErrorCode.MissingMember => new MissingMemberException(ex.Error.Message, ex.InnerException),
                ErrorCode.MemberAcessInvalid => new MemberAccessException(ex.Error.Message, ex.InnerException),
                ErrorCode.NotImplemented => new NotImplementedException(ex.Error.Message, ex.InnerException),
                ErrorCode.PlatformNotSupported => new PlatformNotSupportedException(ex.Error.Message, ex.InnerException),
                ErrorCode.NotSupported => new NotSupportedException(ex.Error.Message, ex.InnerException),
                ErrorCode.NullReference => new NullReferenceException(ex.Error.Message, ex.InnerException),
                ErrorCode.OperationCanceled => new OperationCanceledException(ex.Error.Message, ex.InnerException),
                ErrorCode.OutOfMemory => new OutOfMemoryException(ex.Error.Message, ex.InnerException),
                ErrorCode.PathTooLongFileName => new PathTooLongException(ex.Error.Message, ex.InnerException),
                ErrorCode.RankArray => new RankException(ex.Error.Message, ex.InnerException),
                ErrorCode.StackOverflow => new StackOverflowException(ex.Error.Message, ex.InnerException),
                ErrorCode.Timeout => new TimeoutException(ex.Error.Message, ex.InnerException),
                ErrorCode.TimeZoneNotFound => new TimeZoneNotFoundException(ex.Error.Message, ex.InnerException),
                ErrorCode.TypeAccessInvalid => new TypeAccessException(ex.Error.Message, ex.InnerException),
                ErrorCode.TypeInitializationFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("TypeName", out string? typeName) ? new TypeInitializationException(typeName, ex.InnerException) : new TypeInitializationException(null, ex.InnerException),
                ErrorCode.TypeLoadFailure => new TypeLoadException(ex.Error.Message, ex.InnerException),
                ErrorCode.TypeUnloaded => new TypeUnloadedException(ex.Error.Message, ex.InnerException),
                ErrorCode.UnauthorizedAccess => new UnauthorizedAccessException(ex.Error.Message, ex.InnerException),
                ErrorCode.ValidationFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("Value", out string? value) && ex.Error.Details.TryGetValue("ResultMemberNames", out string? resultMemberNames) ?
                    new ValidationException(new ValidationResult(ex.Error.Message, resultMemberNames.Split(", ").ToList()), null, value) : ex.Error.Details != null && ex.Error.Details.TryGetValue("Value", out string? valueNoMember) ? new ValidationException(ex.Error.Message, null, valueNoMember) : new ValidationException(ex.Error.Message, ex.InnerException),
                ErrorCode.None => new Exception(ex.Error.Message, ex.InnerException),
                ErrorCode.BadRequest => new BadHttpRequestException(ex.Error.Message ?? ex.Message, ex.InnerException ?? new()),
                ErrorCode.SessionIdHeaderNotFound => new Exception("Session Id header (session-id) is missing from the headers.", ex.InnerException),
                ErrorCode.TransactionIdHeaderNotFound => new Exception("Transaction Id header (transaction-id) is missing from the headers.", ex.InnerException),
                ErrorCode.ChannelIdHeaderNotFound => new Exception("Channel Id header (channel-id) is missing from the headers.", ex.InnerException),
                ErrorCode.UserInfoHeaderNotFound => new Exception("User Info header (user-info) is missing from the headers.", ex.InnerException),
                _ => new Exception(ex.Error.Message)
            };
        }
    }
}
